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
                try {
                    BuildRanks = HCData["BuildRanks"].StringValue;
                    ShowRanks = HCData["ShowRanks"].StringValue;
                    JoinRanks = HCData["JoinRanks"].StringValue;
                    Physics = Convert.ToBoolean(HCData["Physics"].ByteValue);
                    Building = Convert.ToBoolean(HCData["Building"].ByteValue);
                    History = HCData["History"].IntArrayValue;

                    if (HCData["MOTD"] != null)
                        MOTD = HCData["MOTD"].StringValue;
                } catch {

                }

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

        public Thread ClientThread, BlockChangeThread, PhysicsThread;
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
            
            // -- Creates HC Metadata if it does not exist.
            if (HCSettings.BuildRanks == null) {
                foreach (Rank r in Core.Rankholder.Ranks)  // -- Allow all ranks to access this map by default.
                    BuildRanks.Add(r);
            }

            if (HCSettings.ShowRanks == null) {
                foreach (Rank r in Core.Rankholder.Ranks)  // -- Allow all ranks to access this map by default.
                    ShowRanks.Add(r);
            }

            if (HCSettings.JoinRanks == null) {
                foreach (Rank r in Core.Rankholder.Ranks)  // -- Allow all ranks to access this map by default.
                    JoinRanks.Add(r);

                HCSettings.Building = true;
                HCSettings.Physics = true;
            }

            HCSettings.Building = true;
            HCSettings.Physics = true;

            if (HCSettings.History == null)
                HCSettings.History = new int[Map.BlockData.Length];

            LastClient = DateTime.UtcNow;
            Clients = new List<NetworkClient>();
            Entities = new List<Entity>();

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

            if (PhysicsThread != null)
                PhysicsThread.Abort();

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
            System.IO.File.Move(Path + "u", Path);
        }

        /// <summary>
        /// Unload a map to conserve memory.
        /// </summary>
        public void UnloadMap() {
            // -- Unloads the map data to conserve memory.
            System.IO.File.Delete(Path);

            if (Path.Substring(Path.Length - 1, 1) != "u") 
                Path += "u";
            

            SaveMap();

            Map.BlockData = null; // -- Remove the block data (a lot of memory)
            GC.Collect(); // -- Let the GC collect it and free our memory
            Loaded = false; // -- Make sure the server knows the map is no longer loaded.
        }

        /// <summary>
        /// Saves this map.
        /// </summary>
        /// <param name="Filename"></param>
        public void SaveMap(string Filename = "") {
            if (!Loaded)
                return;

            HCSettings.BuildRanks = "";

            foreach (Rank r in BuildRanks) 
                HCSettings.BuildRanks += r.ID + ",";
            foreach (Rank r in JoinRanks)
                HCSettings.JoinRanks += r.ID + ",";
            foreach (Rank r in ShowRanks)
                HCSettings.ShowRanks += r.ID + ",";

            HCSettings.BuildRanks = HCSettings.BuildRanks.TrimEnd(',');
            HCSettings.ShowRanks = HCSettings.ShowRanks.TrimEnd(',');
            HCSettings.JoinRanks = HCSettings.JoinRanks.TrimEnd(',');

            Map.MetadataParsers["Hypercube"] = HCSettings;

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
            int index = (z * Map.SizeY + y) * Map.SizeX + x;
            return Map.BlockData[index];
        }

        public Block GetBlock(short x, short y, short z) {
            int index = (z * Map.SizeY + y) * Map.SizeX + x;
            return ServerCore.Blockholder.GetBlock(Map.BlockData[index]);
        }

        public void SetBlockID(short x, short y, short z, byte Type, int ClientID) {
            int index = (z * Map.SizeY + y) * Map.SizeX + x;
            Map.BlockData[index] = Type;
            HCSettings.History[index] = ClientID;
        }

        /// <summary>
        /// Checks for clients. If clients have not been active for more than 30 seconds, the map will be unloaded.
        /// </summary>
        public void MapMain() {
            while (ServerCore.Running) {
               
                if ((DateTime.UtcNow - LastClient).TotalSeconds > 1 && Clients.Count == 0 && Loaded == true)
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

                for (int i = 0; i < Entities.Count; i++ ) {
                    if (Entities[i].Changed) {
                        var TeleportPacket = new Packets.PlayerTeleport();
                        TeleportPacket.PlayerID = (sbyte)Entities[i].ClientID;
                        TeleportPacket.X = Entities[i].X;
                        TeleportPacket.Y = Entities[i].Y;
                        TeleportPacket.Z = Entities[i].Z;
                        TeleportPacket.yaw = Entities[i].Rot;
                        TeleportPacket.pitch = Entities[i].Look;

                        foreach (NetworkClient c in Clients) {
                            if (Entities[i].MyClient != null && Entities[i].MyClient != c)
                                TeleportPacket.Write(c);
                            else if (Entities[i].MyClient == c && Entities[i].SendOwn == true) {
                                TeleportPacket.PlayerID = (sbyte)-1;
                                TeleportPacket.Write(c);
                                Entities[i].SendOwn = false;
                            }
                        }
                        Entities[i].Changed = false;
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
            var ChangeBlock = ServerCore.Blockholder.GetBlock(Type);

            if (Mode == 0)
                Type = 0;

            if (Type == (MapBlock.ID - 1))
                return;

            if (!BuildRanks.Contains(Client.CS.PlayerRank)) {
                Chat.SendClientChat(Client, "&4Error:&f You are not allowed to build here.");
                SendBlockToClient(X, Y, Z, MapBlock, Client);
                return;
            }

            if (!MapBlock.RanksDelete.Contains(Client.CS.PlayerRank) && Mode == 0) {
                Chat.SendClientChat(Client, "&4Error:&f You are not allowed to delete this block type.");
                SendBlockToClient(X, Y, Z, MapBlock, Client);
                return;
            }

            if (!ChangeBlock.RanksPlace.Contains(Client.CS.PlayerRank) && Mode > 0) {
                Chat.SendClientChat(Client, "&4Error:&f You are not allowed to place this block type.");
                SendBlockToClient(X, Y, Z, MapBlock, Client);
                return;
            }

            if (!(X >= 0 && X < Map.SizeX) || !(Y >= 0 && Y < Map.SizeY) || !(Z >= 0 && Z < Map.SizeZ)) {
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
                            var BlockQueue = GetBlock((short)(X + ix), (short)(Y + iy), (short)(Z + iz));

                            if (BlockQueue.Physics > 0 || (BlockQueue.PhysicsPlugin != "" && BlockQueue.PhysicsPlugin != null)) {
                                if (!PhysicsQueue.Contains(new QueueItem((short)(X + ix), (short)(Y + iy), (short)(Z + iz), 1), new QueueComparator())) 
                                    PhysicsQueue.Add(new QueueItem((short)(X + ix), (short)(Y + iy), (short)(Z + iz), 250));
                                
                            }
                        }
                    }
                }
            }

            if (Send) 
                BlockchangeQueue.Add(new QueueItem(X, Y, Z, Priority));
            
        }

        public void MoveBlock(short X, short Y, short Z, short X2, short Y2, short Z2, bool undo, bool physics, short priority) {
            if (!(X >= 0 && X < Map.SizeX) || !(Y >= 0 && Y < Map.SizeY) || !(Z >= 0 && Z < Map.SizeZ) || !(X2 >= 0 && X2 < Map.SizeX) || !(Y2 >= 0 && Y2 < Map.SizeY) || !(Z2 >= 0 && Z2 < Map.SizeZ)) 
                return;

            var Block1 = GetBlock(X, Y, Z);
            var Block2 = GetBlock(X2, Y2, Z2);

            SetBlockID(X, Y, Z, 0, -1);
            SetBlockID(X2, Y2, Z2, (byte)(Block1.ID - 1), -1); //TODO: Get client.

            if (undo) {
                //TODO: Undo
            }

            //if (Block1.ID - 1 != 0)
                BlockchangeQueue.Add(new QueueItem(X, Y, Z, priority));

            //if (Block2.ID != Block1.ID)
                BlockchangeQueue.Add(new QueueItem(X2, Y2, Z2, priority));

            if (physics) {
                for (short ix = -1; ix < 2; ix++) {
                    for (short iy = -1; iy < 2; iy++) {
                        for (short iz = -1; iz < 2; iz++) {
                            var BlockQueue = GetBlock((short)(X + ix), (short)(Y + iy), (short)(Z + iz));
                            var BlockQueue2 = GetBlock((short)(X2 + ix), (short)(Y2 + iy), (short)(Z2 + iz));

                            if (BlockQueue.Physics > 0 || (BlockQueue.PhysicsPlugin != "" && BlockQueue.PhysicsPlugin != null)) {
                                if (!PhysicsQueue.Contains(new QueueItem((short)(X + ix), (short)(Y + iy), (short)(Z + iz), 250), new QueueComparator())) 
                                    PhysicsQueue.Add(new QueueItem((short)(X + ix), (short)(Y + iy), (short)(Z + iz), 1));
                            }
                            if (BlockQueue2.Physics > 0 || (BlockQueue2.PhysicsPlugin != "" && BlockQueue2.PhysicsPlugin != null)) {
                                if (!PhysicsQueue.Contains(new QueueItem((short)(X2 + ix), (short)(Y2 + iy), (short)(Z2 + iz), 250), new QueueComparator()))
                                    PhysicsQueue.Add(new QueueItem((short)(X2 + ix), (short)(Y2 + iy), (short)(Z2 + iz), 250));
                            }
                        }
                    }
                }
            }

        }

        public void Blockchanger() {
            while (ServerCore.Running) {
                if (HCSettings.Building) {
                    int Changes = 0;

                    while (BlockchangeQueue.Count > 0 && (Changes <= ServerCore.MaxBlockChanges)) {
                        for (int i = 0; i < BlockchangeQueue.Count; i++) {
                            if (BlockchangeQueue[i].Priority == 0) {
                                SendBlockToClients(BlockchangeQueue[i].X, BlockchangeQueue[i].Y, BlockchangeQueue[i].Z, GetBlock(BlockchangeQueue[i].X, BlockchangeQueue[i].Y, BlockchangeQueue[i].Z));
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
            while (ServerCore.Running) {
                if (HCSettings.Building) {
                    while (PhysicsQueue.Count > 0) {
                        for (int i = 0; i < PhysicsQueue.Count; i++) {
                            var physicBlock = GetBlock(PhysicsQueue[i].X, PhysicsQueue[i].Y, PhysicsQueue[i].Z);
                            short X = PhysicsQueue[i].X, Y = PhysicsQueue[i].Y, Z = PhysicsQueue[i].Z;
                            PhysicsQueue.RemoveAt(i);

                            switch (physicBlock.Physics) {
                                case 10:
                                    PhysicsOriginalSand(physicBlock, X, Y, Z);
                                    break;
                                case 11:
                                    PhysicsD3Sand(physicBlock, X, Y, Z);
                                    break;
                                case 20:
                                    break;
                                case 21:
                                    break;
                            }
                            
                        }
                    }
                }
                Thread.Sleep(100);
            }
        }

        #region Physic Computations
        void PhysicsOriginalSand(Block physicBlock, short X, short Y, short Z) {
            if (GetBlockID(X, Y, (short)(Z - 1)) == 0)
                MoveBlock(X, Y, Z, X, Y, (short)(Z - 1), true, true, 1);
        }

        void PhysicsD3Sand(Block physicBlock, short X, short Y, short Z) {
            if (GetBlockID(X, Y, (short)(Z - 1)) == 0)
                MoveBlock(X, Y, Z, X, Y, (short)(Z - 1), true, true, 1);
            else if (GetBlockID((short)(X + 1), Y, (short)(Z - 1)) == 0 && GetBlockID((short)(X + 1), Y, Z) == 0)
                MoveBlock(X, Y, Z, (short)(X + 1), Y, (short)(Z - 1), true, true, 900);
            else if (GetBlockID((short)(X - 1), Y, (short)(Z - 1)) == 0 && GetBlockID((short)(X - 1), Y, Z) == 0)
                MoveBlock(X, Y, Z, (short)(X - 1), Y, (short)(Z - 1), true, true, 900);
            else if (GetBlockID(X, (short)(Y + 1), (short)(Z - 1)) == 0 && GetBlockID(X, (short)(Y + 1), Z) == 0)
                MoveBlock(X, Y, Z, X, (short)(Y + 1), (short)(Z - 1), true, true, 900);
            else if (GetBlockID(X, (short)(Y - 1), (short)(Z - 1)) == 0 && GetBlockID(X, (short)(Y - 1), Z) == 0)
                MoveBlock(X, Y, Z, X, (short)(Y - 1), (short)(Z - 1), true, true, 900);
        }
        void PhysicsInfiniteWater() {

        }
        void PhysicsFiniteWater() {

        }
        #endregion
        public void SendBlockToClient(short X, short Y, short Z, Block Type, NetworkClient c) {
            var BlockchangePacket = new Packets.SetBlockServer();

            if (Type.CPELevel > c.CS.CustomBlocksLevel)
                BlockchangePacket.Block = (byte)Type.CPEReplace;
            else
                BlockchangePacket.Block = Type.OnClient;

            BlockchangePacket.X = X;
            BlockchangePacket.Y = Y;
            BlockchangePacket.Z = Z;
            BlockchangePacket.Write(c);
        }

        public void SendBlockToClients(short X, short Y, short Z, Block Type) {
            var BlockchangePacket = new Packets.SetBlockServer();
            BlockchangePacket.X = X;
            BlockchangePacket.Y = Y;
            BlockchangePacket.Z = Z;

            foreach (NetworkClient c in Clients) {
                if (Type.CPELevel > c.CS.CustomBlocksLevel)
                    BlockchangePacket.Block = (byte)Type.CPEReplace;
                else
                    BlockchangePacket.Block = Type.OnClient;

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
