using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading;

using ClassicWorld_NET;
using fNbt;

using Hypercube_Classic.Client;
using Hypercube_Classic.Core;

namespace Hypercube_Classic.Map {
    /// <summary>
    /// Hypercube metadata structure for Hypercube maps. This plugs into ClassicWorld.net to load and save Hypercube-specific settings.
    /// </summary>
    public struct HypercubeMetadata : IMetadataStructure {
        public short BuildRank;
        public short ShowRank;
        public short JoinRank;
        public bool Physics;
        public bool Building;
        public string MOTD;

        public NbtCompound Read(NbtCompound Metadata) {
            var HCData = Metadata.Get<NbtCompound>("Hypercube");

            if (HCData != null) {
                BuildRank = HCData["BuildRank"].ShortValue;
                ShowRank = HCData["ShowRank"].ShortValue;
                JoinRank = HCData["JoinRank"].ShortValue;
                Physics = Convert.ToBoolean(HCData["Physics"].ByteValue);
                Building = Convert.ToBoolean(HCData["Building"].ByteValue);

                if (HCData["MOTD"] != null)
                    MOTD = HCData["MOTD"].StringValue;

                Metadata.Remove(HCData);
            }

            return Metadata;
        }

        public NbtCompound Write() {
            var HCBase = new NbtCompound("Hypercube");

            HCBase.Add(new NbtShort("BuildRank", BuildRank));
            HCBase.Add(new NbtShort("ShowRank", ShowRank));
            HCBase.Add(new NbtShort("JoinRank", JoinRank));
            HCBase.Add(new NbtByte("Physics", Convert.ToByte(Physics)));
            HCBase.Add(new NbtByte("Building", Convert.ToByte(Building)));
            
            if (MOTD != null)
                HCBase.Add(new NbtString("MOTD", MOTD));

            return HCBase;
        }
    }

    public class HypercubeMap {
        #region Variables
        public ClassicWorld Map;
        public HypercubeMetadata HCSettings;
        public bool Loaded = true;
        public string Path;
        public List<NetworkClient> Clients;
        public List<Entity> Entities;
        public short FreeID = 0, NextID = 0; // -- For Client Entity IDs.
        public object EntityLocker = new object();

        Thread ClientThread;
        public Thread EntityThread;
        DateTime LastClient;
        Hypercube ServerCore;
        #endregion

        /// <summary>
        /// Creates a new blank map.
        /// </summary>
        public HypercubeMap(Hypercube Core, string Filename, string MapName, short SizeX, short SizeY, short SizeZ) {
            ServerCore = Core;
            Map = new ClassicWorld(SizeX, SizeZ, SizeY);
            Map.MapName = MapName;
            Path = Filename;
            Map.Save(Path);

            
            LastClient = DateTime.UtcNow;
            Clients = new List<NetworkClient>();
            Entities = new List<Entity>();

            ClientThread = new Thread(MapMain);
            ClientThread.Start();

            EntityThread = new Thread(MapEntities);
            EntityThread.Start();
        }

        /// <summary>
        /// Loads a pre-existing map
        /// </summary>
        /// <param name="Filename">The path to the map.</param>
        public HypercubeMap(Hypercube Core, string Filename) {
            ServerCore = Core;
            Path = Filename;
            Map = new ClassicWorld(Filename);
            HCSettings = new HypercubeMetadata(); // -- Create our HC Specific settings
            Map.MetadataParsers.Add("Hypercube", HCSettings); // -- Register it with the map loader
            Map.Load(); // -- Load the map
            
            LastClient = DateTime.UtcNow;
            Clients = new List<NetworkClient>();
            Entities = new List<Entity>();

            // -- Memory Conservation:
            if (Path.Substring(Path.Length - 1, 1) == "u") { // -- Unloads anything with a ".cwu" file extension. (ClassicWorld unloaded)
                Map.BlockData = null;
                GC.Collect();
                Loaded = false;
            }

            ClientThread = new Thread(MapMain);
            ClientThread.Start();

            EntityThread = new Thread(MapEntities);
            EntityThread.Start();
        }

        /// <summary>
        /// Shuts down the Threads.
        /// </summary>
        public void Shutdown() {
            if (ClientThread != null)
                ClientThread.Abort();

            if (EntityThread != null)
                EntityThread.Abort();

            SaveMap();
        }

        /// <summary>
        /// Reloads a map that was unloaded for memory conservation.
        /// </summary>
        public void LoadMap() {
            Map = new ClassicWorld(Path);
            HCSettings = new HypercubeMetadata(); // -- Create our HC Specific settings
            Map.MetadataParsers.Add("Hypercube", HCSettings); // -- Register it with the map loader
            Map.Load(); // -- Load the map

            Path = Path.Replace(".cwu", ".cw");
            Loaded = true;
        }

        /// <summary>
        /// Unload a map to conserve memory.
        /// </summary>
        public void UnloadMap() {
            // -- Unloads the map data to conserve memory.
            if (Path.Substring(Path.Length - 1, 1) != "u") {
                Path += "u";
            }

            Map.BlockData = null; // -- Remove the block data (a lot of memory)
            GC.Collect(); // -- Let the GC collect it and free our memory
            Loaded = false; // -- Make sure the server knows the map is no longer loaded.
        }

        /// <summary>
        /// Saves this map.
        /// </summary>
        /// <param name="Filename"></param>
        public void SaveMap(string Filename = "") {
            if (Filename != "")
                Map.Save(Filename);
            else
                Map.Save(Path);
        }

        /// <summary>
        /// Gets the block at (x, y, z) from the ClassicWorld Map.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public byte GetBlockID(short x, short y, short z) { // (Y * Size_Z + Z) * Size_X + X
            int index = (y * Map.SizeZ + z) * Map.SizeX + x;
            return Map.BlockData[index];
        }

        /// <summary>
        /// Checks for clients. If clients have not been active for more than 30 seconds, the map will be unloaded.
        /// </summary>
        public void MapMain() {
            while (ServerCore.Running) {
                if ((DateTime.UtcNow - LastClient).TotalSeconds > 300 && Clients.Count == 0)
                    UnloadMap();
                else if (Clients.Count > 0)
                    LastClient = DateTime.UtcNow;

                Thread.Sleep(30000);
            }
        }

        public void MapEntities() {
            while (ServerCore.Running) {
                
                foreach (Entity E in Entities) {
                    if (E.Changed) {
                        var TeleportPacket = new Packets.PlayerTeleport();
                        TeleportPacket.PlayerID = (sbyte)E.ClientID;
                        TeleportPacket.X = E.X;
                        TeleportPacket.Y = E.Y;
                        TeleportPacket.Z = E.Z;
                        TeleportPacket.yaw = E.Rot;
                        TeleportPacket.pitch = E.Look;
                        
                        foreach (NetworkClient c in Clients) {
                            if (E.MyClient != null && E.MyClient != c)
                                TeleportPacket.Write(c);
                            else if (E.MyClient == c && E.SendOwn == true) {
                                TeleportPacket.PlayerID = (sbyte)-1;
                                TeleportPacket.Write(c);
                                E.SendOwn = false;
                            }
                        }
                        E.Changed = false;
                    }
                }
                Thread.Sleep(10); // -- This gives the rest of the program time to make modifications to the Entitys and Clients list. 
            }
        }

        public void SpawnEntity(Entity ToSpawn) {
            var ESpawn = new Packets.SpawnPlayer();
            ESpawn.PlayerName = ToSpawn.Name;
            ESpawn.PlayerID = (sbyte)ToSpawn.ClientID;
            ESpawn.X = ToSpawn.X;
            ESpawn.Y = ToSpawn.Y;
            ESpawn.Z = ToSpawn.Z;
            ESpawn.Yaw = ToSpawn.Rot;
            ESpawn.Pitch = ToSpawn.Look;
            
            foreach (NetworkClient c in Clients) {
                if (c != ToSpawn.MyClient) 
                    ESpawn.Write(c);
                 else {
                    ESpawn.PlayerID = (sbyte)-1;
                    ESpawn.Write(c);
                    ESpawn.PlayerID = (sbyte)ToSpawn.ClientID;
                }
            }
        }

        public void SendAllEntities(NetworkClient Client) {
            var ESpawn = new Packets.SpawnPlayer();

            foreach (Entity e in Entities) {
                ESpawn.PlayerName = e.Name;
                ESpawn.PlayerID = (sbyte)e.ClientID;
                ESpawn.X = e.X;
                ESpawn.Y = e.Y;
                ESpawn.Z = e.Z;
                ESpawn.Yaw = e.Rot;
                ESpawn.Pitch = e.Look;

                if (e.MyClient != Client)
                    ESpawn.Write(Client);
                else {
                    ESpawn.PlayerID = (sbyte)-1;
                    ESpawn.Write(Client);
                }
            }
        }

        public void DeleteEntity(ref Entity ToSpawn) {
            if (Entities.Contains(ToSpawn))
                Entities.Remove(ToSpawn);

            if (ToSpawn.MyClient != null && Clients.Contains(ToSpawn.MyClient))
                Clients.Remove(ToSpawn.MyClient);

            var Despawn = new Packets.DespawnPlayer();
            Despawn.PlayerID = (sbyte)ToSpawn.ClientID;

            foreach (NetworkClient c in Clients) {
                Despawn.Write(c);
            }
        }

        /// <summary>
        /// Sends the map to the given client.
        /// </summary>
        /// <param name="Client"></param>
        public void SendMap(NetworkClient Client) {
            if (Loaded == false)
                LoadMap();

            Client.SendHandshake();

            byte[] Temp = new byte[(Map.SizeX * Map.SizeY * Map.SizeZ) + 4];
            byte[] LenBytes = BitConverter.GetBytes(Temp.Length - 4);
            int Offset = 4;
            Array.Reverse(LenBytes);

            Buffer.BlockCopy(LenBytes, 0, Temp, 0, 4);
            Buffer.BlockCopy(Map.BlockData, 0, Temp, 4, Temp.Length - 4);

            //TODO: Imeplement this properly.

            //for (int i = 0; i < Temp.Length - 5; i++) {
            //    // TODO: Convert once Block.cs is implemented.
            //    Temp[Offset] = Map.BlockData[i];
            //}

            Temp = Libraries.GZip.Compress(Temp);

            //int CompressedSize = Temp.Length;
            //CompressedSize += (1024 - (CompressedSize % 1024));

            var Init = new Packets.LevelInit();
            Init.Write(Client);

            Offset = 0;

            while (Offset != Temp.Length) {
                if (Temp.Length - Offset > 1024) {
                    byte[] Send = new byte[1024];
                    Buffer.BlockCopy(Temp, Offset, Send, 0, 1024);

                    var Chunk = new Packets.LevelChunk();
                    Chunk.Length = 1024;
                    Chunk.Data = Send;
                    Chunk.Percent = (byte)((Offset / Temp.Length) * 100);
                    Chunk.Write(Client);

                    Offset += 1024;
                } else {
                    byte[] Send = new byte[1024];
                    Buffer.BlockCopy(Temp, Offset, Send, 0, Temp.Length - Offset);

                    var Chunk = new Packets.LevelChunk();
                    Chunk.Length = (short)((Temp.Length - Offset));
                    Chunk.Data = Send;
                    Chunk.Percent = (byte)((Offset / Temp.Length) * 100);
                    Chunk.Write(Client);

                    Offset += Chunk.Length;
                }
            }

            var Final = new Packets.LevelFinalize();
            Final.SizeX = Map.SizeX;
            Final.SizeY = Map.SizeZ;
            Final.SizeZ = Map.SizeY;
            Final.Write(Client);

        }
    }
}
