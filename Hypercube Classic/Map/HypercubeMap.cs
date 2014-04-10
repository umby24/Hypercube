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
        public bool History;
        public string MOTD;

        public NbtCompound Read(NbtCompound Metadata) {
            var HCData = Metadata.Get<NbtCompound>("Hypercube");

            if (HCData != null) {
                try {
                    BuildRanks = HCData["BuildRanks"].StringValue;
                    ShowRanks = HCData["ShowRanks"].StringValue;
                    JoinRanks = HCData["JoinRanks"].StringValue;
                    Physics = Convert.ToBoolean(HCData["Physics"].ByteValue);
                    Building = Convert.ToBoolean(HCData["Building"].ByteValue);

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

    public struct Vector3S {
        public short X;
        public short Y;
        public short Z;
    }

    public class QueueItem {
        public short X, Y, Z, Priority;
        public DateTime DoneTime;

        public QueueItem(short _X, short _Y, short _Z, short _Priority) {
            X = _X;
            Y = _Y;
            Z = _Z;
            Priority = _Priority;
        }
        public QueueItem(short _X, short _Y, short _Z, DateTime _DoneTime) {
            X = _X;
            Y = _Y;
            Z = _Z;
            DoneTime = _DoneTime;
        }
    }

    public class HypercubeMap {
        #region Variables
        public ClassicWorld Map;
        public HypercubeMetadata HCSettings;
        public bool Loaded = true;
        public string Path;
        public MapHistory ThisHistory;
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
        public Hypercube ServerCore;
        #endregion

        /// <summary>
        /// Creates a new blank map.
        /// </summary>
        public HypercubeMap(Hypercube Core, string Filename, string MapName, short SizeX, short SizeY, short SizeZ) {
            ServerCore = Core;
            Map = new ClassicWorld(SizeX, SizeZ, SizeY);

            HCSettings = new HypercubeMetadata(); // -- Hypercube specific settings, woo.
            HCSettings.Building = true; // -- Enable building, history and physics by default.
            HCSettings.Physics = true;
            HCSettings.History = true;

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

            SaveMap(Path);
            
            LastClient = DateTime.UtcNow;
            Clients = new List<NetworkClient>();
            Entities = new List<Entity>();

            if (HCSettings.History)
                ThisHistory = new MapHistory(this);
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

            HCSettings = (HypercubeMetadata)Map.MetadataParsers["Hypercube"];
            HCSettings.JoinRanks = null;
            HCSettings.ShowRanks = null;
            HCSettings.BuildRanks = null;

            // -- Creates HC Metadata if it does not exist.
            if (HCSettings.BuildRanks == null) {
                foreach (Rank r in Core.Rankholder.Ranks)  // -- Allow all ranks to access this map by default.
                    BuildRanks.Add(r);
            } else {
                BuildRanks = RankContainer.SplitRanks(Core, HCSettings.BuildRanks);
            }

            if (HCSettings.ShowRanks == null) {
                foreach (Rank r in Core.Rankholder.Ranks)  // -- Allow all ranks to access this map by default.
                    ShowRanks.Add(r);
            } else {
                ShowRanks = RankContainer.SplitRanks(Core, HCSettings.ShowRanks);
            }

            if (HCSettings.JoinRanks == null) {
                foreach (Rank r in Core.Rankholder.Ranks)  // -- Allow all ranks to access this map by default.
                    JoinRanks.Add(r);

                HCSettings.Building = true;
                HCSettings.Physics = true;
                HCSettings.History = true;
            } else {
                JoinRanks = RankContainer.SplitRanks(Core, HCSettings.JoinRanks);
            }

            LastClient = DateTime.UtcNow;
            Clients = new List<NetworkClient>();
            Entities = new List<Entity>();

            // -- Set CPE Information...
            var myRef = (CPEMetadata)Map.MetadataParsers["CPE"];

            if (myRef.CustomBlocksFallback == null) {
                myRef.CustomBlocksLevel = 1;
                myRef.CustomBlocksVersion = 1;
                myRef.CustomBlocksFallback = new byte[256];

                for (int i = 50; i < 66; i++) 
                    myRef.CustomBlocksFallback[i] = (byte)Core.Blockholder.GetBlock(i).CPEReplace;

                Map.MetadataParsers["CPE"] = myRef;
            }

            // -- Memory Conservation:
            if (Path.Substring(Path.Length - 1, 1) == "u") { // -- Unloads anything with a ".cwu" file extension. (ClassicWorld unloaded)
                Map.BlockData = null;
                GC.Collect();
                Loaded = false;
            }

            ThisHistory = new MapHistory(this);
        }

        #region Map Functions
        public void LoadNewFile(string Filename) {
            if (!System.IO.File.Exists(Filename))
                return;

            Path = Filename;

            Map = new ClassicWorld(Filename);
            HCSettings = new HypercubeMetadata(); // -- Create our HC Specific settings
            Map.MetadataParsers.Add("Hypercube", HCSettings); // -- Register it with the map loader
            Map.Load(); // -- Load the map

            // -- Creates HC Metadata if it does not exist.
            if (HCSettings.BuildRanks == null) {
                foreach (Rank r in ServerCore.Rankholder.Ranks)  // -- Allow all ranks to access this map by default.
                    BuildRanks.Add(r);
            }

            if (HCSettings.ShowRanks == null) {
                foreach (Rank r in ServerCore.Rankholder.Ranks)  // -- Allow all ranks to access this map by default.
                    ShowRanks.Add(r);
            }

            if (HCSettings.JoinRanks == null) {
                foreach (Rank r in ServerCore.Rankholder.Ranks)  // -- Allow all ranks to access this map by default.
                    JoinRanks.Add(r);

                HCSettings.Building = true;
                HCSettings.Physics = true;
                HCSettings.History = true;
            }

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
                    myRef.CustomBlocksFallback[i] = (byte)ServerCore.Blockholder.GetBlock(i).CPEReplace;
                }

                Map.MetadataParsers["CPE"] = myRef;
            }
        }
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
            ThisHistory.UnloadHistory();

            ServerCore.Logger._Log("Info", "Map", "Defragmenting history of " + Path);
            ThisHistory.DeFragment();
            ServerCore.Logger._Log("Info", "Map", "Done.");
        }

        /// <summary>
        /// Reloads a map that was unloaded for memory conservation.
        /// </summary>
        public void LoadMap() {
            Map = new ClassicWorld(Path);
            HCSettings = new HypercubeMetadata(); // -- Create our HC Specific settings
            Map.MetadataParsers.Add("Hypercube", HCSettings); // -- Register it with the map loader
            Map.Load(); // -- Load the map
            HCSettings = (HypercubeMetadata)Map.MetadataParsers["Hypercube"];

            ThisHistory.ReloadHistory();

            Path = Path.Replace(".cwu", ".cw");
            Loaded = true;
            System.IO.File.Move(Path + "u", Path);
        }

        /// <summary>
        /// Unload a map to conserve memory.
        /// </summary>
        public void UnloadMap() {
            // -- Unloads the map data to conserve memory.
            if (Path.Substring(Path.Length - 1, 1) != "u") {
                System.IO.File.Move(Path, Path + "u");
                Path += "u";
            }

            HCSettings.Building = true;

            SaveMap();
            ThisHistory.UnloadHistory();

            Map.BlockData = null; // -- Remove the block data (a lot of memory)
            GC.Collect(); // -- Let the GC collect it and free our memory
            Loaded = false; // -- Make sure the server knows the map is no longer loaded.
        }

        /// <summary>
        /// Resizes the map.
        /// </summary>
        /// <param name="x">New X size of the map.</param>
        /// <param name="y">New Y Size of the map.</param>
        /// <param name="z">New Z Size of the map.</param>
        public void ResizeMap(short x, short y, short z) {
            if (Loaded == false)
                LoadMap();

            Map.SizeX = x;
            Map.SizeZ = y;
            Map.SizeY = z;

            var Temp = new Byte[x * y * z];

            if (Temp.Length > Map.BlockData.Length)
                Buffer.BlockCopy(Map.BlockData, 0, Temp, 0, Map.BlockData.Length);
            else
                Buffer.BlockCopy(Map.BlockData, 0, Temp, 0, Temp.Length);

            Map.BlockData = Temp;
            Temp = null;

            if (Loaded == false)
                UnloadMap();

            ResendMap();
        }

        /// <summary>
        /// Saves this map.
        /// </summary>
        /// <param name="Filename"></param>
        public void SaveMap(string Filename = "") {
            if (!Loaded)
                return;

            HCSettings.BuildRanks = "";
            HCSettings.ShowRanks = "";
            HCSettings.JoinRanks = "";

            foreach (Rank r in BuildRanks) 
                HCSettings.BuildRanks += r.ID + ",";

            foreach (Rank r in JoinRanks)
                HCSettings.JoinRanks += r.ID + ",";

            foreach (Rank r in ShowRanks)
                HCSettings.ShowRanks += r.ID + ",";

            HCSettings.BuildRanks = HCSettings.BuildRanks.TrimEnd(',');
            HCSettings.ShowRanks = HCSettings.ShowRanks.TrimEnd(',');
            HCSettings.JoinRanks = HCSettings.JoinRanks.TrimEnd(',');
            HCSettings.Building = true;
            HCSettings.History = true;
            HCSettings.Physics = true;
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
        public byte GetBlockID(short x, short z, short y) { // (Y * Size_Z + Z) * Size_X + X
            if ((0 > x || Map.SizeX <= x) || (0 > z || Map.SizeY <= z) || (0 > y || Map.SizeZ <= y))
                return 255;

            int index = (y * Map.SizeZ + z) * Map.SizeX + x;
            return Map.BlockData[index];
        }

        /// <summary>
        /// Gets the block object for the block at (x,y,z) from the map.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Block GetBlock(short x, short z, short y) {
            int index = (y * Map.SizeZ + z) * Map.SizeX + x;
            return ServerCore.Blockholder.GetBlock(Map.BlockData[index]);
        }

        /// <summary>
        /// Sets the block at (x, y, z) to the new type, with the given clientID in history. If ClientID is -1, no history will be saved.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="y"></param>
        /// <param name="Type"></param>
        /// <param name="ClientID"></param>
        public void SetBlockID(short x, short z, short y, byte Type, int ClientID) {
            int index = (y * Map.SizeZ + z) * Map.SizeX + x;
            
            if (ClientID != -1 && HCSettings.History)
                ThisHistory.AddEntry(x, y, z, (ushort)ClientID, 0, Type, Map.BlockData[index]);

            Map.BlockData[index] = Type;
            
        }

        /// <summary>
        /// Checks for clients. If clients have not been active for more than 30 seconds, the map will be unloaded.
        /// </summary>
        public void MapMain() {
            while (ServerCore.Running) {
                if ((DateTime.UtcNow - LastClient).TotalSeconds > 5 && Clients.Count == 0 && Loaded == true)
                    UnloadMap();
                else if (Clients.Count > 0)
                    LastClient = DateTime.UtcNow;

                Thread.Sleep(30000);
            }
        }

        /// <summary>
        /// Resends the map to all clients.
        /// </summary>
        public void ResendMap() {
            foreach (NetworkClient c in Clients) 
                SendMap(c);
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
            Final.SizeY = Map.SizeZ;
            Final.SizeZ = Map.SizeY;
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

                        for (int z = 0; z < Clients.Count; z++) {
                            if (Entities[i].MyClient != null && Entities[i].MyClient != Clients[z])
                                TeleportPacket.Write(Clients[z]);
                            else if (Entities[i].MyClient == Clients[z] && Entities[i].SendOwn == true) {
                                TeleportPacket.PlayerID = (sbyte)-1;
                                TeleportPacket.Write(Clients[z]);
                                Entities[i].SendOwn = false;
                            }
                        }

                        Entities[i].Changed = false;
                    }
                }
                Thread.Sleep(10);
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
            }
        }

        public void DeleteEntity(ref Entity ToSpawn) {
            var Despawn = new Packets.DespawnPlayer();

            if (Entities.Contains(ToSpawn))
                Entities.Remove(ToSpawn);

            if (ToSpawn.MyClient != null && Clients.Contains(ToSpawn.MyClient)) {
                Clients.Remove(ToSpawn.MyClient);

                foreach (Entity e in Entities) {
                    Despawn.PlayerID = (sbyte)e.ClientID;
                    Despawn.Write(ToSpawn.MyClient);
                }
            }

            Despawn.PlayerID = (sbyte)ToSpawn.ClientID;

            foreach (NetworkClient c in Clients) 
                Despawn.Write(c);

            FreeID = ToSpawn.ClientID;
        }

        #endregion
        #region Blockchanging
        public void ClientChangeBlock(NetworkClient Client, short X, short Y, short Z, byte Mode, Block NewBlock) {
            if ((0 > X || Map.SizeX <= X) || (0 > Z || Map.SizeY <= Z) || (0 > Y || Map.SizeZ <= Y)) {
                Chat.SendClientChat(Client, "&4Error: &fThat block is outside the bounds of the map.");
                return;
            }

            var MapBlock = GetBlock(X, Y, Z);

            if (Mode == 0)
                NewBlock = ServerCore.Blockholder.GetBlock(0);

            if (NewBlock == MapBlock)
                return;

            if (!RankContainer.RankListContains(BuildRanks, Client.CS.PlayerRanks)) {
                Chat.SendClientChat(Client, "&4Error:&f You are not allowed to build here.");
                SendBlockToClient(X, Y, Z, MapBlock, Client);
                return;
            }

            if (!RankContainer.RankListContains(MapBlock.RanksDelete, Client.CS.PlayerRanks) && Mode == 0) {
                Chat.SendClientChat(Client, "&4Error:&f You are not allowed to delete this block type.");
                SendBlockToClient(X, Y, Z, MapBlock, Client);
                return;
            }

            if (!RankContainer.RankListContains(NewBlock.RanksPlace, Client.CS.PlayerRanks) && Mode > 0) {
                Chat.SendClientChat(Client, "&4Error:&f You are not allowed to place this block type.");
                SendBlockToClient(X, Y, Z, MapBlock, Client);
                return;
            }

            BlockChange(Client.CS.ID, X, Y, Z, NewBlock, MapBlock, true, true, true, 250);

        }

        public void BlockChange(int ClientID, short X, short Y, short Z, Block Type, Block LastType, bool Undo, bool Physics, bool Send, short Priority) {
            SetBlockID(X, Y, Z, (byte)(Type.ID - 1), ClientID);

            if (Undo) {
                //TODO: Undo.
                NetworkClient Client = null;

                foreach (NetworkClient c in Clients) {
                    if (c.CS.ID == ClientID) {
                        Client = c;
                        break;
                    }
                }

                if (Client != null) {
                    if (Client.CS.CurrentIndex == -1)
                        Client.CS.CurrentIndex = 0;

                    if (Client.CS.CurrentIndex != (Client.CS.UndoObjects.Count - 1)) {
                        for (int i = Client.CS.CurrentIndex; i < Client.CS.UndoObjects.Count - 1; i++)
                            Client.CS.UndoObjects.RemoveAt(i);
                    }

                    if (Client.CS.UndoObjects.Count >= 50000)
                        Client.CS.UndoObjects.RemoveAt(0);

                    var newUndo = new Undo();
                    newUndo.x = X;
                    newUndo.y = Y;
                    newUndo.z = Z;
                    newUndo.OldBlock = LastType;

                    Client.CS.UndoObjects.Add(newUndo);
                    Client.CS.CurrentIndex = Client.CS.UndoObjects.Count - 1;
                }
            }

            if (Physics) {
                for (short ix = -1; ix < 2; ix++) {
                    for (short iy = -1; iy < 2; iy++) {
                        for (short iz = -1; iz < 2; iz++) {
                            
                            if ((0 > (X + ix) || Map.SizeX <= (X + ix)) || (0 > (Z + iz) || Map.SizeY <= (Z + iz)) || (0 > (Y + iy) || Map.SizeZ <= (Y + iy))) 
                                continue;

                            var BlockQueue = GetBlock((short)(X + ix), (short)(Y + iy), (short)(Z + iz));

                            if (BlockQueue.Physics > 0 || (BlockQueue.PhysicsPlugin != "" && BlockQueue.PhysicsPlugin != null)) {
                                if (!PhysicsQueue.Contains(new QueueItem((short)(X + ix), (short)(Y + iy), (short)(Z + iz), 1), new QueueComparator())) {
                                    var randomGen = new Random();
                                    PhysicsQueue.Add(new QueueItem((short)(X + ix), (short)(Y + iy), (short)(Z + iz), DateTime.UtcNow.AddMilliseconds(Type.PhysicsDelay + randomGen.Next(Type.PhysicsRandom))));
                                }
                            }
                            
                        }
                    }
                }
            }

            if (Send) 
                BlockchangeQueue.Add(new QueueItem(X, Y, Z, Priority));
        }

        public void MoveBlock(short X, short Y, short Z, short X2, short Y2, short Z2, bool undo, bool physics, short priority) {
            if ((0 > X || Map.SizeX <= X) || (0 > Z || Map.SizeY <= Z) || (0 > Y || Map.SizeZ <= Y) || (0 > X2 || Map.SizeX <= X2) || (0 > Z2 || Map.SizeY <= Z2) || (0 > Y2 || Map.SizeZ <= Y2))
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

                            if ((0 > (X + ix) || Map.SizeX <= (X + ix)) || (0 > (Z + iz) || Map.SizeY <= (Z + iz)) || (0 > (Y + iy) || Map.SizeZ <= (Y + iy)))
                                continue;

                            if ((0 > (X2 + ix) || Map.SizeX <= (X2 + ix)) || (0 > (Z2 + iz) || Map.SizeY <= (Z2 + iz)) || (0 > (Y2 + iy) || Map.SizeZ <= (Y2 + iy))) 
                                continue;

                            var BlockQueue = GetBlock((short)(X + ix), (short)(Y + iy), (short)(Z + iz));
                            var BlockQueue2 = GetBlock((short)(X2 + ix), (short)(Y2 + iy), (short)(Z2 + iz));

                            if (BlockQueue.Physics > 0 || (BlockQueue.PhysicsPlugin != "" && BlockQueue.PhysicsPlugin != null)) {
                                if (!PhysicsQueue.Contains(new QueueItem((short)(X + ix), (short)(Y + iy), (short)(Z + iz), 1), new QueueComparator())) {
                                    var randomGen = new Random();
                                    PhysicsQueue.Add(new QueueItem((short)(X + ix), (short)(Y + iy), (short)(Z + iz), DateTime.UtcNow.AddMilliseconds(Block1.PhysicsDelay + randomGen.Next(Block1.PhysicsRandom))));
                                }
                            }

                            if (BlockQueue2.Physics > 0 || (BlockQueue2.PhysicsPlugin != "" && BlockQueue2.PhysicsPlugin != null)) {
                                if (!PhysicsQueue.Contains(new QueueItem((short)(X2 + ix), (short)(Y2 + iy), (short)(Z2 + iz), 250), new QueueComparator())) {
                                    var randomGen = new Random();
                                    PhysicsQueue.Add(new QueueItem((short)(X2 + ix), (short)(Y2 + iy), (short)(Z2 + iz), DateTime.UtcNow.AddMilliseconds(Block2.PhysicsDelay + randomGen.Next(Block2.PhysicsRandom))));
                                }
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

                                if (Changes == ServerCore.MaxBlockChanges) 
                                    break;

                                BlockchangeQueue.RemoveAt(i);
                            } else {
                                BlockchangeQueue[i].Priority -= 1;
                            }
                        }
                    }
                }
                Thread.Sleep(10);
            }
        }

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
        public void PhysicCompleter() {
            while (ServerCore.Running) {
                if (HCSettings.Building) {
                    while (PhysicsQueue.Count > 0) {
                        for (int i = 0; i < PhysicsQueue.Count; i++) {
                            if ((PhysicsQueue[i].DoneTime - DateTime.UtcNow).Milliseconds > 0)
                                continue;
                            
                            var physicBlock = GetBlock(PhysicsQueue[i].X, PhysicsQueue[i].Y, PhysicsQueue[i].Z);
                            var randomGen = new Random();
                            short X = PhysicsQueue[i].X, Y = PhysicsQueue[i].Y, Z = PhysicsQueue[i].Z;
                            PhysicsQueue.RemoveAt(i);
                            
                            //Thread.Sleep(physicBlock.PhysicsDelay + randomGen.Next(physicBlock.PhysicsRandom));

                            switch (physicBlock.Physics) {
                                case 10:
                                    PhysicsOriginalSand(physicBlock, X, Y, Z);
                                    break;
                                case 11:
                                    PhysicsD3Sand(physicBlock, X, Y, Z);
                                    break;
                                case 20:
                                    PhysicsInfiniteWater(physicBlock, X, Y, Z);
                                    break;
                                case 21:
                                    break;
                            }

                        }
                    }
                }
                Thread.Sleep(5);
            }
        }

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

        void PhysicsInfiniteWater(Block physicBlock, short X, short Y, short Z) {
            int PlayerID = -1;

            if (HCSettings.History)
                PlayerID = ThisHistory.Lookup(X, Y, Z)[0].Player;

            if (GetBlockID(X, Y, (short)(Z - 1)) == 0)
                BlockChange(PlayerID, X, Y, (short)(Z - 1), physicBlock, GetBlock(X, Y, (short)(Z - 1)), true, true, true, 1);
            else if (GetBlockID((short)(X + 1), Y, Z) == 0)
                BlockChange(PlayerID, (short)(X + 1), Y, Z, physicBlock, GetBlock((short)(X + 1), Y, Z), true, true, true, 1);
            else if (GetBlockID((short)(X - 1), Y, Z) == 0)
                BlockChange(PlayerID, (short)(X - 1), Y, Z, physicBlock, GetBlock((short)(X - 1), Y, Z), true, true, true, 1);
            else if (GetBlockID(X, (short)(Y + 1), Z) == 0)
                BlockChange(PlayerID, X, (short)(Y + 1), Z, physicBlock, GetBlock(X, (short)(Y + 1), Z), true, true, true, 1);
            else if (GetBlockID(X, (short)(Y - 1), Z) == 0)
                BlockChange(PlayerID, X, (short)(Y - 1), Z, physicBlock, GetBlock(X, (short)(Y - 1), Z), true, true, true, 1);
        }

        void PhysicsFiniteWater(Block physicBlock, short X, short Y, short Z) {
            
        }
        #endregion
        #region Build Functions
        public void BuildBox(NetworkClient Client, short X, short Y, short Z, short X2, short Y2, short Z2, Block Material, Block ReplaceMaterial, bool Hollow, short Priority, bool undo, bool physics) {
            if (X > X2) {
                var temp = X;
                X = X2;
                X2 = temp;
            }
            if (Y > Y2) {
                var temp = Y;
                Y = Y2;
                Y2 = temp;
            }
            if (Z > Z2) {
                var temp = Z;
                Z = Z2;
                Z2 = temp;
            }

            for (short ix = X; ix < X2 + 1; ix++) {
                for (short iy = Y; iy < Y2 + 1; iy++) {
                    for (short iz = Z; iz < Z2 + 1; iz++) {
                        if (ReplaceMaterial.ID == 1 || ReplaceMaterial == GetBlock(ix, iy, iz)) {
                            if (ix == X || ix == X2 || iy == Y || iy == Y2 || iz == Z || iz == Z2)
                                BlockChange(Client.CS.ID, ix, iy, iz, Material, GetBlock(ix, iy, iz), undo, physics, true, Priority);
                             else if (Hollow == false)
                                BlockChange(Client.CS.ID, ix, iy, iz, Material, GetBlock(ix, iy, iz), undo, physics, true, Priority);
                        }
                    }
                }
            }

        }

        public void BuildLine(NetworkClient Client, short X, short Y, short Z, short X2, short Y2, short Z2, Block Material, short Priority, bool undo, bool physics) {
            var Dx = X2 - X;
            var Dy = Y2 - Y;
            var Dz = Z2 - Z;

            int Blocks = 1;

            if (Blocks < Math.Abs(Dx))
                Blocks = Math.Abs(Dx);

            if (Blocks < Math.Abs(Dy))
                Blocks = Math.Abs(Dy);

            if (Blocks < Math.Abs(Dz))
                Blocks = Math.Abs(Dz);

            float Mx = Dx / Blocks;
            float My = Dy / Blocks;
            float Mz = Dz / Blocks;

            for (int i = 0; i < Blocks; i++)
                BlockChange(Client.CS.ID, (short)(X + Mx * i), (short)(Y + My * i), (short)(Z + Mz * i), Material, GetBlock((short)(X + Mx * i), (short)(Y + My * i), (short)(Z + Mz * i)), undo, physics, true, Priority);
        }

        public void BuildSphere(NetworkClient Client, short X, short Y, short Z, float Radius, Block Material, Block ReplaceMaterial, bool Hollow, short Priority, bool Undo, bool Physics) {
            int Rounded = (int)Math.Round(Radius, 1);
            float Power = (float)Math.Pow((double)Radius, 2);

            for (int ix = -Rounded; ix < Rounded; ix++) {
                for (int iy = -Rounded; iy < Rounded; iy++) {
                    for (int iz = -Rounded; iz < Rounded; iz++) {
                        int SquareDistance = (int)(Math.Pow(ix, 2) + Math.Pow(iy, 2) + Math.Pow(iz, 2));
                        bool allowed = false;

                        if (SquareDistance <= Power) {
                            if (Hollow) {
                                allowed = false;

                                if (Math.Pow(ix + 1, 2) + Math.Pow(iy, 2) + Math.Pow(iz, 2) > Power)
                                    allowed = true;

                                if (Math.Pow(ix - 1, 2) + Math.Pow(iy, 2) + Math.Pow(iz, 2) > Power)
                                    allowed = true;

                                if (Math.Pow(ix, 2) + Math.Pow(iy + 1, 2) + Math.Pow(iz, 2) > Power)
                                    allowed = true;

                                if (Math.Pow(ix, 2) + Math.Pow(iy - 1, 2) + Math.Pow(iz, 2) > Power)
                                    allowed = true;

                                if (Math.Pow(ix, 2) + Math.Pow(iy, 2) + Math.Pow(iz + 1, 2) > Power)
                                    allowed = true;

                                if (Math.Pow(ix, 2) + Math.Pow(iy, 2) + Math.Pow(iz - 1, 2) > Power)
                                    allowed = true;
                            } else {
                                allowed = true;
                            }

                            if (allowed) {
                                if (ReplaceMaterial.ID == 1 || ReplaceMaterial == GetBlock((short)(X + ix), (short)(Y + iy), (short)(Z + iz)))
                                    BlockChange(Client.CS.ID, (short)(X + ix), (short)(Y + iy), (short)(Z + iz), Material, GetBlock((short)(X + ix), (short)(Y + iy), (short)(Z + iz)), Undo, Physics, true, Priority);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
