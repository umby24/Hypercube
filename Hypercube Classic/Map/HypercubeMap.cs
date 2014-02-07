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
        public string BuildRanks;
        public string ShowRanks;
        public string JoinRanks;
        public bool Physics;
        public bool Building;
        public string MOTD;
        public int[] History;

        public NbtCompound Read(NbtCompound Metadata) {
            var HCData = Metadata.Get<NbtCompound>("Hypercube");

            if (HCData != null) {
                BuildRanks = HCData["BuildRanks"].StringValue;
                ShowRanks = HCData["ShowRanks"].StringValue;
                JoinRanks = HCData["JoinRanks"].StringValue;
                Physics = Convert.ToBoolean(HCData["Physics"].ByteValue);
                Building = Convert.ToBoolean(HCData["Building"].ByteValue);
                History = HCData["History"].IntArrayValue;

                if (HCData["MOTD"] != null)
                    MOTD = HCData["MOTD"].StringValue;

                Metadata.Remove(HCData);
            }

            return Metadata;
        }

        public NbtCompound Write() {
            var HCBase = new NbtCompound("Hypercube");

            HCBase.Add(new NbtString("BuildRanks", BuildRanks));
            HCBase.Add(new NbtString("ShowRanks", ShowRanks));
            HCBase.Add(new NbtString("JoinRanks", JoinRanks));
            HCBase.Add(new NbtByte("Physics", Convert.ToByte(Physics)));
            HCBase.Add(new NbtByte("Building", Convert.ToByte(Building)));
            HCBase.Add(new NbtIntArray("History", History));

            if (MOTD != null)
                HCBase.Add(new NbtString("MOTD", MOTD));

            return HCBase;
        }
    }

    public struct QueueComparator : IEqualityComparer<QueueItem> {
        public bool Equals(QueueItem Item1, QueueItem Item2) {
            if (Item1.X == Item2.X && Item1.Y == Item2.Y && Item1.Z == Item2.Z)
                return true;
            else
                return false;
        }

        public int GetHashCode(QueueItem Item) {
            int hCode = Item.X ^ Item.Y ^ Item.Z;
            return hCode.GetHashCode();
        }
    }

    public class QueueItem {
        public short X, Y, Z, Priority;

        public QueueItem(short _X, short _Y, short _Z, short _Priority) {
            X = _X;
            Y = _Y;
            Z = _Z;
            Priority = _Priority;
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
        public List<Rank> ShowRanks = new List<Rank>();
        public List<Rank> BuildRanks = new List<Rank>();
        public List<Rank> JoinRanks = new List<Rank>();

        public List<QueueItem> BlockchangeQueue = new List<QueueItem>();
        public List<QueueItem> PhysicsQueue = new List<QueueItem>();

        public short FreeID = 0, NextID = 0; // -- For Client Entity IDs.
        public object EntityLocker = new object();

        Thread ClientThread, BlockChangeThread, PhysicsThread;
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

            HCSettings = new HypercubeMetadata(); // -- Hypercube specific settings, woo.
            HCSettings.Building = true; // -- Enable building and physics by default.
            HCSettings.Physics = true;
            HCSettings.History = new int[Map.BlockData.Length];

            foreach (Rank r in Core.Rankholder.Ranks) { // -- Allow all ranks to access this map by default.
                JoinRanks.Add(r);
                BuildRanks.Add(r);
                ShowRanks.Add(r);
                HCSettings.JoinRanks += r.ID.ToString();
                HCSettings.BuildRanks += r.ID.ToString();
                HCSettings.ShowRanks += r.ID.ToString();
            }

            Map.MetadataParsers.Add("Hypercube", HCSettings); // -- Add the parser so it will save with the map :)

            Map.MapName = MapName;
            Path = Filename;

            Map.GeneratingSoftware = "Hypercube";
            Map.GeneratorName = "Blank";
            Map.CreatingService = "Classicube";
            Map.CreatingUsername = "[SERVER]";

            var myRef = (CPEMetadata)Map.MetadataParsers["CPE"];

            if (myRef.CustomBlocksFallback == null) {
                myRef.CustomBlocksLevel = 1;
                myRef.CustomBlocksVersion = 1;
                myRef.CustomBlocksFallback = new byte[256];

                for (int i = 50; i < 66; i++) {
                    myRef.CustomBlocksFallback[i] = (byte)Core.Blockholder.GetBlock(i).CPEReplace;
                }

                Map.MetadataParsers["CPE"] = myRef;
            }

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
            
            //TODO: Add checking of HC Metadata here.

            LastClient = DateTime.UtcNow;
            Clients = new List<NetworkClient>();
            Entities = new List<Entity>();

            foreach (Rank r in Core.Rankholder.Ranks) { // -- Allow all ranks to access this map by default.
                JoinRanks.Add(r);
                BuildRanks.Add(r);
                ShowRanks.Add(r);
                HCSettings.JoinRanks += r.ID.ToString();
                HCSettings.BuildRanks += r.ID.ToString();
                HCSettings.ShowRanks += r.ID.ToString();
            }

            // -- Set CPE Information...
            var myRef = (CPEMetadata)Map.MetadataParsers["CPE"];

            if (myRef.CustomBlocksFallback == null) {
                myRef.CustomBlocksLevel = 1;
                myRef.CustomBlocksVersion = 1;
                myRef.CustomBlocksFallback = new byte[256];

                for (int i = 50; i < 66; i++) {
                    myRef.CustomBlocksFallback[i] = (byte)Core.Blockholder.GetBlock(i).CPEReplace;
                }

                Map.MetadataParsers["CPE"] = myRef;
            }

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

            BlockChangeThread = new Thread(Blockchanger);
            BlockChangeThread.Start();
        }

        #region Map Functions
        /// <summary>
        /// Shuts down the Threads.
        /// </summary>
        public void Shutdown() {
            if (ClientThread != null)
                ClientThread.Abort();

            if (EntityThread != null)
                EntityThread.Abort();

            if (BlockChangeThread != null)
                BlockChangeThread.Abort();

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

        public Block GetBlock(short x, short y, short z) {
            int index = (y * Map.SizeZ + z) * Map.SizeX + x;
            return ServerCore.Blockholder.GetBlock(Map.BlockData[index]);
        }

        public void SetBlockID(short x, short y, short z, byte Type, int ClientID) {
            int index = (y * Map.SizeZ + z) * Map.SizeX + x;
            Map.BlockData[index] = Type;
            HCSettings.History[index] = ClientID;
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

            for (int i = 0; i < Map.BlockData.Length - 1; i++) {
                var ThisBlock = ServerCore.Blockholder.GetBlock(Map.BlockData[i]);

                if (ThisBlock.CPELevel > Client.CS.CustomBlocksLevel)
                    Temp[Offset] = (byte)ThisBlock.CPEReplace;
                else
                    Temp[Offset] = ThisBlock.OnClient;

                Offset += 1;
            }

            Temp = Libraries.GZip.Compress(Temp);

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
            Final.SizeY = Map.SizeY;
            Final.SizeZ = Map.SizeZ;
            Final.Write(Client);

        }
        #endregion
        #region Entity Management
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
        #endregion
        #region Blockchanging
        public void ClientChangeBlock(NetworkClient Client, short X, short Y, short Z, byte Mode, byte Type) {
            var MapBlock = GetBlock(X, Y, Z);

            if (Mode == 0)
                Type = 0;

            if (Type == MapBlock.ID)
                return;

            if (!BuildRanks.Contains(Client.CS.PlayerRank)) {
                Chat.SendClientChat(Client, "&4Error:&f You are not allowed to build here.");
                SendBlockToClient(X, Y, Z, GetBlockID(X, Y, Z), Client);
                return;
            }

            if (MapBlock.RanksDelete.Contains(Client.CS.PlayerRank) && Mode > 0) {
                Chat.SendClientChat(Client, "&4Error:&f You are not allowed to delete this block type.");
                SendBlockToClient(X, Y, Z, GetBlockID(X, Y, Z), Client);
                return;
            }

            if (MapBlock.RanksPlace.Contains(Client.CS.PlayerRank) && Mode == 0) {
                Chat.SendClientChat(Client, "&4Error:&f You are not allowed to place this block type.");
                SendBlockToClient(X, Y, Z, GetBlockID(X, Y, Z), Client);
                return;
            }

            if (X > Map.SizeX || Y > Map.SizeZ || Z > Map.SizeY) {
                Chat.SendClientChat(Client, "&4Error: &fThat block is outside the bounds of the map.");
                return;
            }

            BlockChange(Client.CS.ID, X, Y, Z, Type, true, true, true, 250);

        }

        public void BlockChange(int ClientID, short X, short Y, short Z, byte Type, bool Undo, bool Physics, bool Send, short Priority) {
            SetBlockID(X, Y, Z, Type, ClientID);

            if (Undo) {
                //TODO: Undo.
            }

            if (Physics) {
                for (short ix = -1; ix < 2; ix++) {
                    for (short iy = -1; iy < 2; iy++) {
                        for (short iz = -1; iz < 2; iz++) {
                            //TODO: AddPhysicsQueue(X, Y, Z);
                            var BlockQueue = GetBlock((short)(X + ix), (short)(Y + iy), (short)(Z + iz));

                            if (BlockQueue.Physics > 0 || (BlockQueue.PhysicsPlugin != "" && BlockQueue.PhysicsPlugin != null)) {
                                if (!BlockchangeQueue.Contains(new QueueItem((short)(X + ix), (short)(Y + iy), (short)(Z + iz), 1), new QueueComparator())) {
                                    PhysicsQueue.Add(new QueueItem((short)(X + ix), (short)(Y + iy), (short)(Z + iz), 1));
                                }
                            }
                        }
                    }
                }
            }

            if (Send) 
                BlockchangeQueue.Add(new QueueItem(X, Y, Z, Priority));
            
        }

        public void Blockchanger() {
            while (ServerCore.Running) {
                if (HCSettings.Building) {
                    int Changes = 0;

                    while (BlockchangeQueue.Count > 0 && (Changes <= ServerCore.MaxBlockChanges)) {
                        for (int i = 0; i < BlockchangeQueue.Count - 1; i++) {
                            if (BlockchangeQueue[i].Priority == 0) {
                                SendBlockToClients(BlockchangeQueue[i].X, BlockchangeQueue[i].Y, BlockchangeQueue[i].Z, GetBlockID(BlockchangeQueue[i].X, BlockchangeQueue[i].Y, BlockchangeQueue[i].Z));
                                Changes += 1;

                                if (Changes == ServerCore.MaxBlockChanges) {
                                    break;
                                }

                                BlockchangeQueue.RemoveAt(i);
                            } else {
                                BlockchangeQueue[i].Priority -= 1;
                            }
                        }
                    }
                }
                Thread.Sleep(100);
            }
        }

        public void PhysicCompleter() {
            if (HCSettings.Building) {
                
            }
        }

        public void SendBlockToClient(short X, short Y, short Z, byte Type, NetworkClient c) {
            var BlockchangePacket = new Packets.SetBlockServer();
            BlockchangePacket.Block = Type;
            BlockchangePacket.X = X;
            BlockchangePacket.Y = Y;
            BlockchangePacket.Z = Z;
            BlockchangePacket.Write(c);
        }

        public void SendBlockToClients(short X, short Y, short Z, byte Type) {
            var BlockchangePacket = new Packets.SetBlockServer();
            BlockchangePacket.Block = Type;
            BlockchangePacket.X = X;
            BlockchangePacket.Y = Y;
            BlockchangePacket.Z = Z;

            foreach (NetworkClient c in Clients) {
                BlockchangePacket.Write(c);
            }
        }

        #endregion
        #region Physics
        public void CalculateSandPhysics() {

        }

        public void CalculateNewSandPhysics() {

        }

        public void CalculateInfiniteLiquid() { 

        }

        public void CalculateFiniteLiquid() {

        }
        #endregion
    }
}
