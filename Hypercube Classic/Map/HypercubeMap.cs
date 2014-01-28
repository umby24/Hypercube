using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading;

using ClassicWorld_NET;
using fNbt;

using Hypercube_Classic.Client;

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
        public byte Clients;

        Thread ClientThread;
        DateTime LastClient;
        #endregion

        /// <summary>
        /// Creates a new blank map.
        /// </summary>
        public HypercubeMap(string Filename, string MapName, short SizeX, short SizeY, short SizeZ) {
            Map = new ClassicWorld(SizeX, SizeZ, SizeY);
            Map.MapName = MapName;
            Path = Filename;
            Map.Save(Path);

            LastClient = DateTime.UtcNow;
            Clients = 0;

            ClientThread = new Thread(MapMain);
            ClientThread.Start();
        }

        /// <summary>
        /// Loads a pre-existing map
        /// </summary>
        /// <param name="Filename">The path to the map.</param>
        public HypercubeMap(string Filename) {
            Path = Filename;
            Map = new ClassicWorld(Filename);
            HCSettings = new HypercubeMetadata(); // -- Create our HC Specific settings
            Map.MetadataParsers.Add("Hypercube", HCSettings); // -- Register it with the map loader
            Map.Load(); // -- Load the map
            LastClient = DateTime.UtcNow;
            Clients = 0;

            // -- Memory Conservation:
            if (Path.Substring(Path.Length - 1, 1) == "u") { // -- Unloads anything with a ".cwu" file extension. (ClassicWorld unloaded)
                Map.BlockData = null;
                GC.Collect();
                Loaded = false;
            }

            ClientThread = new Thread(MapMain);
            ClientThread.Start();
        }

        /// <summary>
        /// Shuts down the memory conservation thread.
        /// </summary>
        public void Shutdown() {
            if (ClientThread != null)
                ClientThread.Abort();
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
            if ((DateTime.UtcNow - LastClient).TotalSeconds > 300 && Clients == 0)
                UnloadMap();
            else if (Clients > 0)
                LastClient = DateTime.UtcNow;
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
