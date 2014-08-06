using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

using ClassicWorld_NET;
using fNbt;

using Hypercube.Client;
using Hypercube.Core;
using Hypercube.Network;

namespace Hypercube.Map {
    /// <summary>
    /// Hypercube metadata structure for Hypercube maps. This plugs into ClassicWorld.net to load and save Hypercube-specific settings.
    /// </summary>
    public struct HypercubeMetadata : IMetadataStructure {
        public string BuildPerms;
        public string ShowPerms;
        public string JoinPerms;
        public bool Physics;
        public bool Building;
        public bool History;
        public string MOTD;

        public NbtCompound Read(NbtCompound Metadata) {
            var HCData = Metadata.Get<NbtCompound>("Hypercube");

            if (HCData != null) {
                try {
                    BuildPerms = HCData["BuildPerms"].StringValue;
                    ShowPerms = HCData["ShowPerms"].StringValue;
                    JoinPerms = HCData["JoinPerms"].StringValue;
                    Physics = Convert.ToBoolean(HCData["Physics"].ByteValue);
                    Building = Convert.ToBoolean(HCData["Building"].ByteValue);
                    History = Convert.ToBoolean(HCData["History"].ByteValue);

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

            HCBase.Add(new NbtString("BuildPerms", BuildPerms));
            HCBase.Add(new NbtString("ShowPerms", ShowPerms));
            HCBase.Add(new NbtString("JoinPerms", JoinPerms));
            HCBase.Add(new NbtByte("Physics", Convert.ToByte(Physics)));
            HCBase.Add(new NbtByte("Building", Convert.ToByte(Building)));
            HCBase.Add(new NbtByte("History", Convert.ToByte(History)));

            if (MOTD != null)
                HCBase.Add(new NbtString("MOTD", MOTD));

            return HCBase;
        }
    }

    public class HypercubeMap {
        #region Variables
        public DateTime Lastclient;
        public ClassicWorld CWMap;
        public HypercubeMetadata HCSettings;
        public Thread ClientThread, BlockThread, PhysicsThread;
        public MapHistory History;

        public string Path;
        public bool Loaded = true;

        #region Lists
        public SortedDictionary<string, Permission> Joinperms = new SortedDictionary<string, Permission>(StringComparer.InvariantCultureIgnoreCase);
        public SortedDictionary<string, Permission> Buildperms = new SortedDictionary<string, Permission>(StringComparer.InvariantCultureIgnoreCase);
        public SortedDictionary<string, Permission> Showperms = new SortedDictionary<string, Permission>(StringComparer.InvariantCultureIgnoreCase);

        public Dictionary<int, Entity> Entities;
        public Dictionary<short, NetworkClient> Clients;
        public NetworkClient[] ClientsList;
        public object ClientLock = new object();
        public object EntityLock = new object();

        public ConcurrentQueue<QueueItem> BlockchangeQueue = new ConcurrentQueue<QueueItem>();
        public List<QueueItem> PhysicsQueue = new List<QueueItem>(); //TODO: Update this to something faster
        #endregion
        #region IDs
        public short FreeID = 0, NextID = 0;
        #endregion
        
        public Hypercube Servercore;
        #endregion

        /// <summary>
        /// Creates a new map.
        /// </summary>
        /// <param name="Core">Server core</param>
        /// <param name="filename">Where to save the file</param>
        /// <param name="mapName">What to name the map</param>
        /// <param name="sizeX">Map's X size</param>
        /// <param name="sizeY">Map's Y size</param>
        /// <param name="sizeZ">Map's Z size</param>
        public HypercubeMap(Hypercube Core, string filename, string mapName, short sizeX, short sizeY, short sizeZ) {
            Servercore = Core;
            CWMap = new ClassicWorld(sizeX, sizeZ, sizeY);

            HCSettings = new HypercubeMetadata(); // -- Hypercube specific settings, woo.
            HCSettings.Building = true; // -- Enable building, history and physics by default.
            HCSettings.Physics = true;
            HCSettings.History = true;

            var joinPerm = Core.Permholder.GetPermission("map.joinmap");

            Joinperms.Add("map.joinmap", joinPerm);
            Showperms.Add("map.joinmap", joinPerm);
            Buildperms.Add("player.build", Core.Permholder.GetPermission("player.build"));

            CWMap.MetadataParsers.Add("Hypercube", HCSettings);
            CWMap.MapName = mapName;
            CWMap.GeneratingSoftware = "Hypercube";
            CWMap.GeneratorName = "Blank";
            CWMap.CreatingService = "Classicube";
            CWMap.CreatingUsername = "[SERVER]";

            var cpeMeta = (CPEMetadata)CWMap.MetadataParsers["CPE"];

            if (cpeMeta.CustomBlocksFallback == null) {
                cpeMeta.CustomBlocksLevel = CPE.CustomBlocksSupportLevel;
                cpeMeta.CustomBlocksVersion = CPE.CustomBlocksVersion;
                cpeMeta.CustomBlocksFallback = new byte[256];

                for (int i = 50; i < 66; i++)
                    cpeMeta.CustomBlocksFallback[i] = (byte)Core.Blockholder.GetBlock(i).CPEReplace;

                CWMap.MetadataParsers["CPE"] = cpeMeta;
            }

            cpeMeta.CustomBlocksFallback = null;

            Lastclient = DateTime.UtcNow;
            Clients = new Dictionary<short, NetworkClient>();
            Entities = new Dictionary<int, Entity>();
            CreateList();

            Path = filename;
            Save(Path);
            History = new MapHistory(this);
        }

        /// <summary>
        /// Loads a pre-existing map.
        /// </summary>
        /// <param name="Core">Server core</param>
        /// <param name="filename">File to load.</param>
        public HypercubeMap(Hypercube Core, string filename) {
            Servercore = Core;
            Path = filename;

            CWMap = new ClassicWorld(filename);
            HCSettings = new HypercubeMetadata();
            CWMap.MetadataParsers.Add("Hypercube", HCSettings);
            CWMap.Load();

            HCSettings = (HypercubeMetadata)CWMap.MetadataParsers["Hypercube"];

            if (HCSettings.BuildPerms == null) {
                Buildperms.Add("player.build", Core.Permholder.GetPermission("player.build"));
            } else
                Buildperms = PermissionContainer.SplitPermissions(Core, HCSettings.BuildPerms);
            

            if (HCSettings.ShowPerms == null) {
                Showperms.Add("map.joinmap", Core.Permholder.GetPermission("map.joinmap"));
            } else 
                Showperms = PermissionContainer.SplitPermissions(Core, HCSettings.ShowPerms);
            

            if (HCSettings.JoinPerms == null) {
                Joinperms.Add("map.joinmap", Core.Permholder.GetPermission("map.joinmap"));

                HCSettings.Building = true;
                HCSettings.Physics = true;
                HCSettings.History = true;
            } else
                Joinperms = PermissionContainer.SplitPermissions(Core, HCSettings.JoinPerms);
            

            var cpeMeta = (CPEMetadata)CWMap.MetadataParsers["CPE"];

            if (cpeMeta.CustomBlocksFallback == null) {
                cpeMeta.CustomBlocksLevel = CPE.CustomBlocksSupportLevel;
                cpeMeta.CustomBlocksVersion = CPE.CustomBlocksVersion;
                cpeMeta.CustomBlocksFallback = new byte[256];

                for (int i = 50; i < 66; i++)
                    cpeMeta.CustomBlocksFallback[i] = (byte)Core.Blockholder.GetBlock(i).CPEReplace;

                CWMap.MetadataParsers["CPE"] = cpeMeta;
            }

            cpeMeta.CustomBlocksFallback = null;

            Lastclient = DateTime.UtcNow;
            Clients = new Dictionary<short, NetworkClient>();
            Entities = new Dictionary<int, Entity>();
            CreateList();

            // -- Memory Conservation
            if (Path.Substring(Path.Length - 1, 1) == "u") {
                CWMap.BlockData = null;
                CWMap.MetadataParsers.Clear();
                GC.Collect();
                Loaded = false;
            }

            if (HCSettings.History)
                History = new MapHistory(this);
        }

        public static void LoadMaps(Hypercube Core) {
            if (!Directory.Exists("Maps"))
                Directory.CreateDirectory("Maps");

            string[] files = Directory.GetFiles("Maps", "*.cw*", SearchOption.AllDirectories);

            foreach (string file in files) {
                try {
                    var NewMap = new HypercubeMap(Core, file);

                    foreach (HypercubeMap m in Core.Maps) {
                        if (m.CWMap.MapName == NewMap.CWMap.MapName) {
                            Core.Logger.Log("Maps", "Failed to load " + file + ". Map with duplicate name already loaded. (" + m.Path + ")", LogType.Error);
                            NewMap = null;
                            break;
                        }
                    }

                    if (NewMap == null)
                        continue;

                    Core.Maps.Add(NewMap);
                    Core.Logger.Log("Maps", "Loaded map '" + file + "'. (X=" + NewMap.CWMap.SizeX + " Y=" + NewMap.CWMap.SizeZ + " Z=" + NewMap.CWMap.SizeY + ")", LogType.Info);
                } catch (Exception e) {
                    Core.Logger.Log("Maps", "Failed to load map '" + file + "'.", LogType.Warning);
                    Core.Logger.Log("Maps", e.Message, LogType.Error);
                    GC.Collect();
                }
            }
        }

        #region Map Functions
        public void Save(string Filename = "") {
            if (!Loaded)
                return;

            HCSettings.BuildPerms = "";
            HCSettings.ShowPerms = "";
            HCSettings.JoinPerms = "";

            HCSettings.BuildPerms = PermissionContainer.PermissionsToString(Buildperms);
            HCSettings.JoinPerms = PermissionContainer.PermissionsToString(Joinperms);
            HCSettings.ShowPerms = PermissionContainer.PermissionsToString(Showperms);

            CWMap.MetadataParsers["Hypercube"] = HCSettings;

            if (Filename != "")
                CWMap.Save(Filename);
            else
                CWMap.Save(Path);
        }

        public void Load() {
            if (Loaded)
                return;

            CWMap = new ClassicWorld(Path);
            HCSettings = new HypercubeMetadata();
            CWMap.MetadataParsers.Add("Hypercube", HCSettings);
            CWMap.Load();

            HCSettings = (HypercubeMetadata)CWMap.MetadataParsers["Hypercube"];
            Path = Path.Replace(".cwu", ".cw");
            Loaded = true;
            File.Move(Path + "u", Path);

            if (HCSettings.History && History != null)
                History.ReloadHistory();
            else if (HCSettings.History) 
                History = new MapHistory(this);
            

            Servercore.Logger.Log("Map", CWMap.MapName + " reloaded.", LogType.Info);
            Servercore.Luahandler.RunFunction("E_MapReloaded", this);
        }

        public void Load(string filename) {
            if (!File.Exists(filename)) {
                if (File.Exists(filename + "u"))
                    File.Move(filename + "u", filename);
                else
                    return;
            }

            Path = filename;
            CWMap = new ClassicWorld(filename);
            HCSettings = new HypercubeMetadata();
            CWMap.MetadataParsers.Add("Hypercube", HCSettings);
            CWMap.Load();

            HCSettings = (HypercubeMetadata)CWMap.MetadataParsers["Hypercube"];

            if (HCSettings.BuildPerms == null) {
                Buildperms.Add("player.build", Servercore.Permholder.GetPermission("player.build"));
            } else
                Buildperms = PermissionContainer.SplitPermissions(Servercore, HCSettings.BuildPerms);


            if (HCSettings.ShowPerms == null) {
                Showperms.Add("map.joinmap", Servercore.Permholder.GetPermission("map.joinmap"));
            } else
                Showperms = PermissionContainer.SplitPermissions(Servercore, HCSettings.ShowPerms);


            if (HCSettings.JoinPerms == null) {
                Joinperms.Add("map.joinmap", Servercore.Permholder.GetPermission("map.joinmap"));

                HCSettings.Building = true;
                HCSettings.Physics = true;
                HCSettings.History = true;
            } else
                Joinperms = PermissionContainer.SplitPermissions(Servercore, HCSettings.JoinPerms);


            var cpeMeta = (CPEMetadata)CWMap.MetadataParsers["CPE"];

            if (cpeMeta.CustomBlocksFallback == null) {
                cpeMeta.CustomBlocksLevel = CPE.CustomBlocksSupportLevel;
                cpeMeta.CustomBlocksVersion = CPE.CustomBlocksVersion;
                cpeMeta.CustomBlocksFallback = new byte[256];

                for (int i = 50; i < 66; i++)
                    cpeMeta.CustomBlocksFallback[i] = (byte)Servercore.Blockholder.GetBlock(i).CPEReplace;

                CWMap.MetadataParsers["CPE"] = cpeMeta;
            }

            cpeMeta.CustomBlocksFallback = null;

            if (HCSettings.History)
                History = new MapHistory(this);
        }

        public void Unload() {
            if (Path.Substring(Path.Length - 1, 1) != "u") {
                File.Move(Path, Path + "u");
                Path += "u";
            }

            Save();

            if (HCSettings.History && History != null)
                History.UnloadHistory();

            CWMap.BlockData = null;
            CWMap.MetadataParsers.Clear();
            GC.Collect();
            Loaded = false;
            Servercore.Logger.Log("Map", CWMap.MapName + " unloaded.", LogType.Info);
            Servercore.Luahandler.RunFunction("E_MapUnloaded", this);
        }

        public void Resize(short x, short y, short z) {
            bool UnloadMap = false;

            if (!Loaded) {
                Load();
                UnloadMap = true;
            }

            CWMap.SizeX = x;
            CWMap.SizeY = z;
            CWMap.SizeZ = y;

            var Temp = new byte[x * y * z];

            if (Temp.Length > CWMap.BlockData.Length)
                Buffer.BlockCopy(CWMap.BlockData, 0, Temp, 0, CWMap.BlockData.Length);
            else
                Buffer.BlockCopy(CWMap.BlockData, 0, Temp, 0, Temp.Length);

            CWMap.BlockData = Temp;
            Temp = null;

            if (UnloadMap)
                Unload();
            else
                Resend();
                
        }

        public byte GetBlockID(short x, short z, short y) {
            if ((0 > x || CWMap.SizeX <= x) || (0 > z || CWMap.SizeY <= z) || (0 > y || CWMap.SizeZ <= y))
                return 255;

            if (!Loaded)
                return 255;

            int index = (y * CWMap.SizeZ + z) * CWMap.SizeX + x;
            return CWMap.BlockData[index];
        }

        public Block GetBlock(short x, short z, short y) {
            return Servercore.Blockholder.GetBlock(GetBlockID(x, z, y));
        }

        public void SetBlockID(short x, short z, short y, byte type, int clientId) {
            if (!Loaded)
                return;

            int index = (y * CWMap.SizeZ + z) * CWMap.SizeX + x;

            if (clientId != -1 && HCSettings.History && History != null)
                History.AddEntry(x, y, z, (ushort)clientId, (ushort)History.GetLastPlayer(x, y, z), type, CWMap.BlockData[index]);

            CWMap.BlockData[index] = type;
        }

        /// <summary>
        /// Checks for clients. If clients have not been active for more than 30 seconds, the map will be unloaded.
        /// </summary>
        public void MapMain() {
            while (Servercore.Running) {
                if ((DateTime.UtcNow - Lastclient).TotalSeconds > 5 && Clients.Count == 0 && Loaded == true)
                    Unload();
                else if (Clients.Count > 0)
                    Lastclient = DateTime.UtcNow;

                Thread.Sleep(30000);
            }
        }

        public void Resend() {
            BlockchangeQueue = new ConcurrentQueue<QueueItem>();

            lock (ClientLock) {
                foreach (NetworkClient c in Clients.Values) {
                    Send(c);
                    c.CS.Entities.Clear();
                }
            }
        }

        public void Send(NetworkClient Client) {
            if (!Loaded)
                Load();

            if (HCSettings.MOTD == null)
                Client.SendHandshake();
            else
                Client.SendHandshake(HCSettings.MOTD);

            byte[] Temp = new byte[(CWMap.SizeX * CWMap.SizeY * CWMap.SizeZ) + 4];
            byte[] LenBytes = BitConverter.GetBytes(Temp.Length - 4);
            int Offset = 4;
            Array.Reverse(LenBytes);

            Buffer.BlockCopy(LenBytes, 0, Temp, 0, 4);

            for (int i = 0; i < CWMap.BlockData.Length - 1; i++) {
                var ThisBlock = Servercore.Blockholder.GetBlock(CWMap.BlockData[i]);

                if (ThisBlock.CPELevel > Client.CS.CustomBlocksLevel)
                    Temp[Offset] = (byte)ThisBlock.CPEReplace;
                else
                    Temp[Offset] = ThisBlock.OnClient;

                Offset += 1;
            }

            Temp = Libraries.GZip.Compress(Temp);

            var init = new LevelInit();
            init.Write(Client);

            Offset = 0;

            while (Offset != Temp.Length) {
                if (Temp.Length - Offset > 1024) {
                    byte[] Send = new byte[1024];
                    Buffer.BlockCopy(Temp, Offset, Send, 0, 1024);

                    var Chunk = new LevelChunk();
                    Chunk.Length = 1024;
                    Chunk.Data = Send;
                    Chunk.Percent = (byte)(((float)Offset / Temp.Length) * 100);
                    Chunk.Write(Client);

                    Offset += 1024;
                } else {
                    byte[] Send = new byte[1024];
                    Buffer.BlockCopy(Temp, Offset, Send, 0, Temp.Length - Offset);

                    var Chunk = new LevelChunk();
                    Chunk.Length = (short)((Temp.Length - Offset));
                    Chunk.Data = Send;
                    Chunk.Percent = (byte)(((float)Offset / Temp.Length) * 100);
                    Chunk.Write(Client);

                    Offset += Chunk.Length;
                }
            }

            var Final = new LevelFinalize();
            Final.SizeX = CWMap.SizeX;
            Final.SizeY = CWMap.SizeZ;
            Final.SizeZ = CWMap.SizeY;
            Final.Write(Client);

            Temp = null;
            GC.Collect();
        }

        public void Shutdown() {
            if (ClientThread != null)
                ClientThread.Abort();

            if (BlockThread != null)
                BlockThread.Abort();

            if (PhysicsThread != null)
                PhysicsThread.Abort();

            Save();

            if (HCSettings.History && History != null) {
                History.UnloadHistory();
                History.DeFragment();
            }
        }
        #endregion
        #region Entity Management
        public void CreateList() {
            ClientsList = Clients.Values.ToArray();
        }

        public void DeleteEntity(ref Entity ToSpawn) {
            if (Entities.ContainsKey(ToSpawn.ID)) {
                lock (EntityLock) {
                    Entities.Remove(ToSpawn.ID);
                }
            }

            if (ToSpawn.MyClient != null && Clients.ContainsKey(ToSpawn.MyClient.CS.ID) != false) {
                lock (ClientLock) {
                    Clients.Remove(ToSpawn.MyClient.CS.ID);
                    CreateList();
                }
            }

            FreeID = ToSpawn.ClientID;
            Servercore.Luahandler.RunFunction("E_EntityDeleted", this, ToSpawn);
        }
        #endregion
        #region Block Management
        public void ClientChangeBlock(NetworkClient Client, short X, short Y, short Z, byte Mode, Block NewBlock) {
            if ((0 > X || CWMap.SizeX <= X) || (0 > Z || CWMap.SizeY <= Z) || (0 > Y || CWMap.SizeZ <= Y)) {
                Chat.SendClientChat(Client, "§EThat block is outside the bounds of the map.");
                return;
            }

            var MapBlock = GetBlock(X, Y, Z);

            if (Mode == 0)
                NewBlock = Servercore.Blockholder.GetBlock(0);

            if (NewBlock == MapBlock && NewBlock != Servercore.Blockholder.GetBlock(0))
                return;

            bool Canbuild = false;

            foreach(Rank r in Client.CS.PlayerRanks) {
                if (PermissionContainer.RankMatchesPermissions(r, Buildperms.Values.ToList(), true)) {
                    Canbuild = true;
                    break;
                }
            }

            if (!Canbuild) {
                Chat.SendClientChat(Client, "§EYou are not allowed to build here.");
                SendBlock(Client, X, Y, Z, MapBlock);
                return;
            }

            if (!RankContainer.RankListContains(MapBlock.RanksDelete, Client.CS.PlayerRanks) && Mode == 0) {
                Chat.SendClientChat(Client, "§EYou are not allowed to delete this block type.");
                SendBlock(Client, X, Y, Z, MapBlock);
                return;
            }

            if (!RankContainer.RankListContains(NewBlock.RanksPlace, Client.CS.PlayerRanks) && Mode > 0) {
                Chat.SendClientChat(Client, "§EYou are not allowed to place this block type.");
                SendBlock(Client, X, Y, Z, MapBlock);
                return;
            }

            Servercore.Luahandler.RunFunction("E_BlockChange", Client, X, Y, Z, NewBlock);
            BlockChange(Client.CS.ID, X, Y, Z, NewBlock, MapBlock, true, true, true, 250);
        }

        public void BlockChange(short ClientID, short X, short Y, short Z, Block Type, Block LastType, bool Undo, bool Physics, bool Send, short Priority) {
            SetBlockID(X, Y, Z, (byte)(Type.ID), ClientID);

            //if (Undo) {
            //    NetworkClient Client = null;

            //    if (ServerCore.Clients.ContainsKey(ClientID))
            //        Client = Clients[ClientID];
            //    else
            //        return;

            //    if (Client != null) {
            //        if (Client.CS.CurrentIndex == -1)
            //            Client.CS.CurrentIndex = 0;

            //        if (Client.CS.CurrentIndex != (Client.CS.UndoObjects.Count - 1)) {
            //            for (int i = Client.CS.CurrentIndex; i < Client.CS.UndoObjects.Count - 1; i++)
            //                Client.CS.UndoObjects.RemoveAt(i);
            //        }

            //        if (Client.CS.UndoObjects.Count >= 50000)
            //            Client.CS.UndoObjects.RemoveAt(0);

            //        var newUndo = new Undo();
            //        newUndo.x = X;
            //        newUndo.y = Y;
            //        newUndo.z = Z;
            //        newUndo.OldBlock = LastType;
            //        newUndo.NewBlock = Type;

            //        Client.CS.UndoObjects.Add(newUndo);
            //        Client.CS.CurrentIndex = Client.CS.UndoObjects.Count - 1;
            //    }
            //}

            if (Physics) {
                for (short ix = -1; ix < 2; ix++) {
                    for (short iy = -1; iy < 2; iy++) {
                        for (short iz = -1; iz < 2; iz++) {

                            if ((0 > (X + ix) || CWMap.SizeX <= (X + ix)) || (0 > (Z + iz) || CWMap.SizeY <= (Z + iz)) || (0 > (Y + iy) || CWMap.SizeZ <= (Y + iy)))
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
                BlockchangeQueue.Enqueue(new QueueItem(X, Y, Z, Priority));
        }

        public void MoveBlock(short X, short Y, short Z, short X2, short Y2, short Z2, bool undo, bool physics, short priority) {
            if ((0 > X || CWMap.SizeX <= X) || (0 > Z || CWMap.SizeY <= Z) || (0 > Y || CWMap.SizeZ <= Y) || (0 > X2 || CWMap.SizeX <= X2) || (0 > Z2 || CWMap.SizeY <= Z2) || (0 > Y2 || CWMap.SizeZ <= Y2))
                return;

            var Block1 = GetBlock(X, Y, Z);
            var Block2 = GetBlock(X2, Y2, Z2);

            SetBlockID(X, Y, Z, 0, -1);
            SetBlockID(X2, Y2, Z2, (byte)(Block1.ID), History.GetLastPlayer(X, Z, Y));

            //if (undo) {
            //    var lastPlayer = History.GetLastPlayer(X, Z, Y);
            //    NetworkClient Client = null;

            //    lock (ClientLock) {
            //        Client = Clients[lastPlayer];
            //    }

            //    if (Client != null) {
            //        if (Client.CS.CurrentIndex == -1)
            //            Client.CS.CurrentIndex = 0;

            //        if (Client.CS.CurrentIndex != (Client.CS.UndoObjects.Count - 1)) {
            //            for (int i = Client.CS.CurrentIndex; i < Client.CS.UndoObjects.Count - 1; i++)
            //                Client.CS.UndoObjects.RemoveAt(i);
            //        }

            //        if (Client.CS.UndoObjects.Count >= 50000)
            //            Client.CS.UndoObjects.RemoveAt(0);

            //        var newUndo = new Undo();
            //        newUndo.x = X;
            //        newUndo.y = Y;
            //        newUndo.z = Z;
            //        newUndo.OldBlock = Block1;
            //        newUndo.NewBlock = Servercore.Blockholder.GetBlock(0);

            //        Client.CS.UndoObjects.Add(newUndo);

            //        var newUndo2 = new Undo();
            //        newUndo2.x = X2;
            //        newUndo2.y = Y2;
            //        newUndo2.z = Z2;
            //        newUndo2.OldBlock = Block2;
            //        newUndo2.NewBlock = Block1;

            //        Client.CS.UndoObjects.Add(newUndo2);

            //        Client.CS.CurrentIndex = Client.CS.UndoObjects.Count - 1;
            //    }
            //}

            BlockchangeQueue.Enqueue(new QueueItem(X, Y, Z, priority));
            BlockchangeQueue.Enqueue(new QueueItem(X2, Y2, Z2, priority));

            if (physics) {
                for (short ix = -1; ix < 2; ix++) {
                    for (short iy = -1; iy < 2; iy++) {
                        for (short iz = -1; iz < 2; iz++) {

                            if ((0 > (X + ix) || CWMap.SizeX <= (X + ix)) || (0 > (Z + iz) || CWMap.SizeY <= (Z + iz)) || (0 > (Y + iy) || CWMap.SizeZ <= (Y + iy)))
                                continue;

                            if ((0 > (X2 + ix) || CWMap.SizeX <= (X2 + ix)) || (0 > (Z2 + iz) || CWMap.SizeY <= (Z2 + iz)) || (0 > (Y2 + iy) || CWMap.SizeZ <= (Y2 + iy)))
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

        public void SendBlock(NetworkClient client, short x, short y, short z, Block type) {
            var Setblock = new SetBlockServer();
            Setblock.X = x;
            Setblock.Y = y;
            Setblock.Z = z;

            if (type.CPELevel > client.CS.CustomBlocksLevel)
                Setblock.Block = (byte)type.CPEReplace;
            else
                Setblock.Block = type.OnClient;

            Setblock.Write(client);
        }

        public void SendBlockToAll(short x, short y, short z, Block type) {
            foreach(NetworkClient c in ClientsList)
                SendBlock(c, x, y, z, type);
            
        }

        public void BlockQueueLoop() {
            while (Servercore.Running) {
                if (!HCSettings.Building) { // -- if the map has building disabled.
                    Thread.Sleep(10);
                    continue;
                }

                int changes = 0;

                while (changes <= Servercore.MaxBlockChanges) {
                    QueueItem output;

                    if (!BlockchangeQueue.TryDequeue(out output))
                        break;

                    if (output.Priority == 0) { // -- Our block changes feature prioritized changes ;P
                        changes += 1;
                        SendBlockToAll(output.X, output.Y, output.Z, GetBlock(output.X, output.Y, output.Z));
                    } else {
                        output.Priority -= 1; // -- Every tick of the loop, the item will get bumped closer to being changed.
                        BlockchangeQueue.Enqueue(output);
                    }
                }

                Thread.Sleep(100);
            }
        }

        public void PhysicsQueueLoop() {
            while (Servercore.Running) {
                if (HCSettings.Building && HCSettings.Physics) {
                    while (PhysicsQueue.Count > 0) {
                        for (int i = 0; i < PhysicsQueue.Count; i++) {
                            try {
                                if ((PhysicsQueue[i].DoneTime - DateTime.UtcNow).Milliseconds > 0)
                                    continue;
                            } catch {
                                continue;
                            }

                            var physicBlock = GetBlock(PhysicsQueue[i].X, PhysicsQueue[i].Y, PhysicsQueue[i].Z);
                            short X = PhysicsQueue[i].X, Y = PhysicsQueue[i].Y, Z = PhysicsQueue[i].Z;

                            PhysicsQueue.RemoveAt(i);

                            if (i != 0)
                                i -= 1;

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
                                    PhysicsFiniteWater(physicBlock, X, Y, Z);
                                    break;
                                case 22:
                                    PhysicsSnow(physicBlock, X, Y, Z);
                                    break;
                            }

                        }
                    }
                }
                Thread.Sleep(10);
            }
        }

        #region Physics Functions
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
            short PlayerID = -1;

            if (HCSettings.History)
                PlayerID = History.GetLastPlayer(X, Y, Z);

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
            if (GetBlock(X, Y, (short)(Z - 1)).Name == "Air")
                MoveBlock(X, Y, Z, X, Y, (short)(Z - 1), true, true, 2);
            else {
                int[,] FillArray = new int[1024, 1024];
                var Fill = new ConcurrentQueue<QueueItem>();
                bool found = false;

                Fill.Enqueue(new QueueItem(X, Y, Z, 1));

                while (true) {
                    QueueItem working;

                    if (!Fill.TryDequeue(out working))
                        break;

                    if (GetBlockID(working.X, working.Y, (short)(working.Z - 1)) == 0) {
                        MoveBlock(X, Y, Z, working.X, working.Y, (short)(working.Z - 1), true, true, 2);
                        found = true;
                    } else {
                        if (GetBlockID((short)(working.X + 1), working.Y, working.Z) == 0 && FillArray[working.X + 1, working.Y] == 0) {
                            FillArray[working.X + 1, working.Y] = 1;
                            Fill.Enqueue(new QueueItem((short)(working.X + 1), working.Y, working.Z, 1));
                        }

                        if (GetBlockID((short)(working.X - 1), working.Y, working.Z) == 0 && FillArray[working.X - 1, working.Y] == 0) {
                            FillArray[working.X * 1, working.Y] = 1;
                            Fill.Enqueue(new QueueItem((short)(working.X - 1), working.Y, working.Z, 1));
                        }

                        if (GetBlockID(working.X, (short)(working.Y + 1), working.Z) == 0 && FillArray[working.X, working.Y + 1] == 0) {
                            FillArray[working.X, working.Y + 1] = 1;
                            Fill.Enqueue(new QueueItem(working.X, (short)(working.Y + 1), working.Z, 1));
                        }

                        if (GetBlockID(working.X, (short)(working.Y - 1), working.Z) == 0 && FillArray[working.X, working.Y - 1] == 0) {
                            FillArray[working.X, working.Y - 1] = 1;
                            Fill.Enqueue(new QueueItem(working.X, (short)(working.Y - 1), working.Z, 1));
                        }

                    }

                    if (Fill.Count > 50000 || found)
                        Fill = new ConcurrentQueue<QueueItem>();
                }
            }
        }

        void PhysicsSnow(Block physicBlock, short X, short Y, short Z) {
            if (GetBlockID(X, Y, (short)(Z - 1)) == 0 || GetBlockID(X, Y, (short)(Z - 1)) == 53)
                MoveBlock(X, Y, Z, X, Y, (short)(Z - 1), true, true, 1);
        }
        #endregion
        #endregion
        #region FillFunctions
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
                        if (ReplaceMaterial.ID == 99 || ReplaceMaterial == GetBlock(ix, iy, iz)) {
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

            float Mx = Dx / (float) Blocks;
            float My = Dy / (float)Blocks;
            float Mz = Dz / (float)Blocks;

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
                                if (ReplaceMaterial.ID == 99 || ReplaceMaterial == GetBlock((short)(X + ix), (short)(Y + iy), (short)(Z + iz)))
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
