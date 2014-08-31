using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.IO;

using ClassicWorld.NET;
using fNbt;

using Hypercube.Client;
using Hypercube.Core;
using Hypercube.Network;

namespace Hypercube.Map {
    /// <summary>
    /// ServerCore metadata structure for ServerCore maps. This plugs into ClassicWorld.net to load and save ServerCore-specific settings.
    /// </summary>
    public struct HypercubeMetadata : IMetadataStructure {
        public string BuildPerms;
        public string ShowPerms;
        public string JoinPerms;
        public bool Physics;
        public bool Building;
        public bool History;
        public string Motd;

        public NbtCompound Read(NbtCompound metadata) {
            var hcData = metadata.Get<NbtCompound>("ServerCore");

            if (hcData != null) {
                //try {
                    BuildPerms = hcData["BuildPerms"].StringValue;
                    ShowPerms = hcData["ShowPerms"].StringValue;
                    JoinPerms = hcData["JoinPerms"].StringValue;
                    Physics = Convert.ToBoolean(hcData["Physics"].ByteValue);
                    Building = Convert.ToBoolean(hcData["Building"].ByteValue);
                    History = Convert.ToBoolean(hcData["History"].ByteValue);

                    if (hcData["MOTD"] != null)
                        Motd = hcData["MOTD"].StringValue;
                //} catch (Exception) {

                //}

                metadata.Remove(hcData);
            }

            return metadata;
        }

        public NbtCompound Write() {
            var hcBase = new NbtCompound("ServerCore")
            {
                new NbtString("BuildPerms", BuildPerms),
                new NbtString("ShowPerms", ShowPerms),
                new NbtString("JoinPerms", JoinPerms),
                new NbtByte("Physics", Convert.ToByte(Physics)),
                new NbtByte("Building", Convert.ToByte(Building)),
                new NbtByte("History", Convert.ToByte(History))
            };

            if (Motd != null)
                hcBase.Add(new NbtString("MOTD", Motd));

            return hcBase;
        }
    }

    public class HypercubeMap {
        #region Variables
        public DateTime Lastclient;
        public Classicworld CWMap;
        public HypercubeMetadata HCSettings;
        public Thread BlockThread, PhysicsThread;
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
        public Entity[] EntitysList;
        public object ClientLock = new object();
        public object EntityLock = new object();

        public ConcurrentQueue<QueueItem> BlockchangeQueue = new ConcurrentQueue<QueueItem>();
        public ConcurrentQueue<QueueItem> PhysicsQueue = new ConcurrentQueue<QueueItem>();
        #endregion
        #region IDs
        public readonly Stack<sbyte> FreeIds = new Stack<sbyte>(127);
        #endregion
        #endregion

        /// <summary>
        /// Creates a new map.
        /// </summary>
        /// <param name="filename">Where to save the file</param>
        /// <param name="mapName">What to name the map</param>
        /// <param name="sizeX">Map's X size</param>
        /// <param name="sizeY">Map's Y size</param>
        /// <param name="sizeZ">Map's Z size</param>
        public HypercubeMap(string filename, string mapName, short sizeX, short sizeY, short sizeZ) {
            CWMap = new Classicworld(sizeX, sizeZ, sizeY);

            HCSettings = new HypercubeMetadata {Building = true, Physics = true, History = true}; // -- ServerCore specific settings, woo.

            var joinPerm = ServerCore.Permholder.GetPermission("map.joinmap");

            Joinperms.Add("map.joinmap", joinPerm);
            Showperms.Add("map.joinmap", joinPerm);
            Buildperms.Add("player.build", ServerCore.Permholder.GetPermission("player.build"));

            CWMap.MetadataParsers.Add("ServerCore", HCSettings);
            CWMap.MapName = mapName;
            CWMap.GeneratingSoftware = "ServerCore";
            CWMap.GeneratorName = "Blank";
            CWMap.CreatingService = "Classicube";
            CWMap.CreatingUsername = "[SERVER]";

            var cpeMeta = (CPEMetadata)CWMap.MetadataParsers["CPE"];

            if (cpeMeta.CustomBlocksFallback == null) {
                cpeMeta.CustomBlocksLevel = CPE.CustomBlocksSupportLevel;
                cpeMeta.CustomBlocksVersion = CPE.CustomBlocksVersion;
                cpeMeta.CustomBlocksFallback = new byte[256];

                for (var i = 50; i < 66; i++)
                    cpeMeta.CustomBlocksFallback[i] = (byte)ServerCore.Blockholder.GetBlock(i).CPEReplace;

                CWMap.MetadataParsers["CPE"] = cpeMeta;
            }

            cpeMeta.CustomBlocksFallback = null;

            Lastclient = DateTime.UtcNow;
            Clients = new Dictionary<short, NetworkClient>();
            Entities = new Dictionary<int, Entity>();
            CreateClientList();

            Path = filename;
            Save(Path);
            History = new MapHistory(this);

            for (sbyte i = 0; i < 127; i++) {
                FreeIds.Push(i);
            }
        }

        /// <summary>
        /// Loads a pre-existing map.
        /// </summary>
        /// <param name="filename">File to load.</param>
        public HypercubeMap(string filename) {
            Path = filename;

            CWMap = new Classicworld(filename);
            HCSettings = new HypercubeMetadata();
            CWMap.MetadataParsers.Add("ServerCore", HCSettings);
            CWMap.Load();

            HCSettings = (HypercubeMetadata)CWMap.MetadataParsers["ServerCore"];

            if (HCSettings.BuildPerms == null) {
                Buildperms.Add("player.build", ServerCore.Permholder.GetPermission("player.build"));
            } else
                Buildperms = PermissionContainer.SplitPermissions(HCSettings.BuildPerms);
            

            if (HCSettings.ShowPerms == null) {
                Showperms.Add("map.joinmap", ServerCore.Permholder.GetPermission("map.joinmap"));
            } else 
                Showperms = PermissionContainer.SplitPermissions(HCSettings.ShowPerms);
            

            if (HCSettings.JoinPerms == null) {
                Joinperms.Add("map.joinmap", ServerCore.Permholder.GetPermission("map.joinmap"));

                HCSettings.Building = true;
                HCSettings.Physics = true;
                HCSettings.History = true;
            } else
                Joinperms = PermissionContainer.SplitPermissions(HCSettings.JoinPerms);
            

            var cpeMeta = (CPEMetadata)CWMap.MetadataParsers["CPE"];

            if (cpeMeta.CustomBlocksFallback == null) {
                cpeMeta.CustomBlocksLevel = CPE.CustomBlocksSupportLevel;
                cpeMeta.CustomBlocksVersion = CPE.CustomBlocksVersion;
                cpeMeta.CustomBlocksFallback = new byte[256];

                for (var i = 50; i < 66; i++)
                    cpeMeta.CustomBlocksFallback[i] = (byte)ServerCore.Blockholder.GetBlock(i).CPEReplace;

                CWMap.MetadataParsers["CPE"] = cpeMeta;
            }

            cpeMeta.CustomBlocksFallback = null;

            Lastclient = DateTime.UtcNow;
            Clients = new Dictionary<short, NetworkClient>();
            Entities = new Dictionary<int, Entity>();
            CreateClientList();

            // -- Memory Conservation
            if (Path.Substring(Path.Length - 1, 1) == "u") {
                CWMap.BlockData = null;
                CWMap.MetadataParsers.Clear();
                GC.Collect();
                Loaded = false;
            }

            if (HCSettings.History)
                History = new MapHistory(this);

            for (sbyte i = 0; i < 127; i++) {
                FreeIds.Push(i);
            }
        }

        public static void LoadMaps() {
            if (!Directory.Exists("Maps"))
                Directory.CreateDirectory("Maps");

            var files = Directory.GetFiles("Maps", "*.cw*", SearchOption.AllDirectories);

            foreach (var file in files) {
                try {
                    var newMap = new HypercubeMap(file);

                    foreach (var m in ServerCore.Maps) {
                        if (m.CWMap.MapName == newMap.CWMap.MapName) {
                            ServerCore.Logger.Log("Maps", "Failed to load " + file + ". Map with duplicate name already loaded. (" + m.Path + ")", LogType.Error);
                            newMap = null;
                            break;
                        }
                    }

                    if (newMap == null)
                        continue;

                    ServerCore.Maps.Add(newMap);
                    ServerCore.Logger.Log("Maps", "Loaded map '" + file + "'. (X=" + newMap.CWMap.SizeX + " Y=" + newMap.CWMap.SizeZ + " Z=" + newMap.CWMap.SizeY + ")", LogType.Info);
                } catch (Exception e) {
                    ServerCore.Logger.Log("Maps", "Failed to load map '" + file + "'.", LogType.Warning);
                    ServerCore.Logger.Log("Maps", e.Message, LogType.Error);
                    GC.Collect();
                }
            }
        }
        #region Map Functions
        public void Save(string filename = "") {
            if (!Loaded)
                return;

            HCSettings.BuildPerms = "";
            HCSettings.ShowPerms = "";
            HCSettings.JoinPerms = "";

            HCSettings.BuildPerms = PermissionContainer.PermissionsToString(Buildperms);
            HCSettings.JoinPerms = PermissionContainer.PermissionsToString(Joinperms);
            HCSettings.ShowPerms = PermissionContainer.PermissionsToString(Showperms);

            CWMap.MetadataParsers["ServerCore"] = HCSettings;

            CWMap.Save(filename != "" ? filename : Path);
        }

        public void Load() {
            if (Loaded)
                return;

            CWMap = new Classicworld(Path);
            HCSettings = new HypercubeMetadata();
            CWMap.MetadataParsers.Add("ServerCore", HCSettings);
            CWMap.Load();

            HCSettings = (HypercubeMetadata)CWMap.MetadataParsers["ServerCore"];
            Path = Path.Replace(".cwu", ".cw");
            Loaded = true;
            File.Move(Path + "u", Path);

            if (HCSettings.History && History != null)
                History.ReloadHistory();
            else if (HCSettings.History) 
                History = new MapHistory(this);
            

            ServerCore.Logger.Log("Map", CWMap.MapName + " reloaded.", LogType.Info);
            ServerCore.Luahandler.RunFunction("E_MapReloaded", this);
        }

        public void Load(string filename) {
            if (!File.Exists(filename)) {
                if (File.Exists(filename + "u"))
                    File.Move(filename + "u", filename);
                else
                    return;
            }

            Path = filename;
            CWMap = new Classicworld(filename);
            HCSettings = new HypercubeMetadata();
            CWMap.MetadataParsers.Add("ServerCore", HCSettings);
            CWMap.Load();

            HCSettings = (HypercubeMetadata)CWMap.MetadataParsers["ServerCore"];

            if (HCSettings.BuildPerms == null) {
                Buildperms.Add("player.build", ServerCore.Permholder.GetPermission("player.build"));
            } else
                Buildperms = PermissionContainer.SplitPermissions(HCSettings.BuildPerms);


            if (HCSettings.ShowPerms == null) {
                Showperms.Add("map.joinmap", ServerCore.Permholder.GetPermission("map.joinmap"));
            } else
                Showperms = PermissionContainer.SplitPermissions(HCSettings.ShowPerms);


            if (HCSettings.JoinPerms == null) {
                Joinperms.Add("map.joinmap", ServerCore.Permholder.GetPermission("map.joinmap"));

                HCSettings.Building = true;
                HCSettings.Physics = true;
                HCSettings.History = true;
            } else
                Joinperms = PermissionContainer.SplitPermissions(HCSettings.JoinPerms);


            var cpeMeta = (CPEMetadata)CWMap.MetadataParsers["CPE"];

            if (cpeMeta.CustomBlocksFallback == null) {
                cpeMeta.CustomBlocksLevel = CPE.CustomBlocksSupportLevel;
                cpeMeta.CustomBlocksVersion = CPE.CustomBlocksVersion;
                cpeMeta.CustomBlocksFallback = new byte[256];

                for (var i = 50; i < 66; i++)
                    cpeMeta.CustomBlocksFallback[i] = (byte)ServerCore.Blockholder.GetBlock(i).CPEReplace;

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
            ServerCore.Logger.Log("Map", CWMap.MapName + " unloaded.", LogType.Info);
            ServerCore.Luahandler.RunFunction("E_MapUnloaded", this);
        }

        public void Resize(short x, short y, short z) {
            var unloadMap = false;

            if (!Loaded) {
                Load();
                unloadMap = true;
            }

            CWMap.SizeX = x;
            CWMap.SizeY = z;
            CWMap.SizeZ = y;

            var temp = new byte[x * y * z];

            Buffer.BlockCopy(CWMap.BlockData, 0, temp, 0,
                temp.Length > CWMap.BlockData.Length ? CWMap.BlockData.Length : temp.Length);

            CWMap.BlockData = temp;

            if (unloadMap)
                Unload();
            else
                Resend();
                
        }

        public byte GetBlockId(short x, short z, short y) {
            if (!BlockInBounds(x, z, y))
                return 254;

            if (!Loaded)
                return 254;

            var index = (y * CWMap.SizeZ + z) * CWMap.SizeX + x;
            return CWMap.BlockData[index];
        }

        public Block GetBlock(short x, short z, short y) {
            return ServerCore.Blockholder.GetBlock(GetBlockId(x, z, y));
        }

        public void SetBlockId(short x, short z, short y, byte type, int clientId) {
            if (!BlockInBounds(x, z, y))
                return;

            if (!Loaded)
                return;

            var index = (y * CWMap.SizeZ + z) * CWMap.SizeX + x;

            if (clientId != -1 && HCSettings.History && History != null)
                History.AddEntry(x, y, z, (ushort)clientId, (ushort)History.GetLastPlayer(x, y, z), type, CWMap.BlockData[index]);

            CWMap.BlockData[index] = type;
        }

        public bool BlockInBounds(short x, short y, short z) {
            return (0 < x || CWMap.SizeX >= x) || (0 < z || CWMap.SizeY >= z) || (0 < y || CWMap.SizeZ >= y);
        }
        /// <summary>
        /// Checks for clients. If clients have not been active for more than 30 seconds, the map will be unloaded.
        /// </summary>
        public void MapMain() {
            if ((DateTime.UtcNow - Lastclient).TotalSeconds > 5 && Clients.Count == 0 && Loaded)
                Unload();
            else if (Clients.Count > 0)
                Lastclient = DateTime.UtcNow;
        }

        public void Resend() {
            BlockchangeQueue = new ConcurrentQueue<QueueItem>();

            lock (ClientLock) {
                foreach (var c in Clients.Values) {
                    Send(c);
                    c.CS.Entities.Clear();
                }
            }
        }

        public void Send(NetworkClient client) {
            if (!Loaded)
                Load();

            if (HCSettings.Motd == null)
                client.SendHandshake();
            else
                client.SendHandshake(HCSettings.Motd);

            var temp = new byte[(CWMap.SizeX * CWMap.SizeY * CWMap.SizeZ) + 4];
            var lenBytes = BitConverter.GetBytes(temp.Length - 4);
            var offset = 4;
            Array.Reverse(lenBytes);

            Buffer.BlockCopy(lenBytes, 0, temp, 0, 4);

            for (var i = 0; i < CWMap.BlockData.Length - 1; i++) {
                var thisBlock = ServerCore.Blockholder.GetBlock(CWMap.BlockData[i]);

                if (thisBlock.CPELevel > client.CS.CustomBlocksLevel)
                    temp[offset] = (byte)thisBlock.CPEReplace;
                else
                    temp[offset] = thisBlock.OnClient;

                offset += 1;
            }

            temp = Libraries.GZip.Compress(temp);

            var init = new LevelInit();
            client.SendQueue.Enqueue(init);
            //init.Write(Client);

            offset = 0;

            while (offset != temp.Length) {
                if (temp.Length - offset > 1024) {
                    var send = new byte[1024];
                    Buffer.BlockCopy(temp, offset, send, 0, 1024);

                    var chunk = new LevelChunk
                    {
                        Length = 1024,
                        Data = send,
                        Percent = (byte) (((float) offset/temp.Length)*100)
                    };
                    client.SendQueue.Enqueue(chunk);
                    //Chunk.Write(Client);

                    offset += 1024;
                } else {
                    var send = new byte[1024];
                    Buffer.BlockCopy(temp, offset, send, 0, temp.Length - offset);

                    var chunk = new LevelChunk
                    {
                        Length = (short) ((temp.Length - offset)),
                        Data = send,
                        Percent = (byte) (((float) offset/temp.Length)*100)
                    };
                    client.SendQueue.Enqueue(chunk);

                    offset += chunk.Length;
                }
            }

            var final = new LevelFinalize {SizeX = CWMap.SizeX, SizeY = CWMap.SizeZ, SizeZ = CWMap.SizeY};
            client.SendQueue.Enqueue(final);

            GC.Collect();
        }

        public void Shutdown() {
            if (BlockThread != null)
                BlockThread.Abort();

            if (PhysicsThread != null)
                PhysicsThread.Abort();

            Save();

            if (!HCSettings.History || History == null) 
                return;

            History.UnloadHistory();
            History.DeFragment();
        }
        #endregion
        #region Entity Management
        public void CreateClientList() {
            ClientsList = Clients.Values.ToArray();
        }

        public void CreateEntityList() {
            EntitysList = Entities.Values.ToArray();
        }

        public void DeleteEntity(ref Entity toSpawn) {
            if (Entities.ContainsKey(toSpawn.Id)) {
                lock (EntityLock) {
                    Entities.Remove(toSpawn.Id);
                    CreateEntityList();
                }
            }

            if (toSpawn.MyClient != null && Clients.ContainsKey(toSpawn.MyClient.CS.Id)) {
                lock (ClientLock) {
                    Clients.Remove(toSpawn.MyClient.CS.Id);
                    CreateClientList();
                }
            }

            FreeIds.Push((sbyte)toSpawn.ClientId);
            ServerCore.Luahandler.RunFunction("E_EntityDeleted", this, toSpawn);
        }
        #endregion
        #region Block Management
        public void ClientChangeBlock(NetworkClient client, short x, short y, short z, byte mode, Block newBlock) {
            if (!BlockInBounds(x, y, z)) 
                return;

            var mapBlock = GetBlock(x, y, z);

            if (mode == 0)
                newBlock = ServerCore.Blockholder.GetBlock(0);

            if (newBlock == mapBlock && newBlock != ServerCore.Blockholder.GetBlock(0))
                return;

            var canbuild = false;

            foreach(var r in client.CS.PlayerRanks) {
                if (PermissionContainer.RankMatchesPermissions(r, Buildperms.Values.ToList(), true)) {
                    canbuild = true;
                    break;
                }
            }

            if (!canbuild) {
                Chat.SendClientChat(client, "§EYou are not allowed to build here.");
                SendBlock(client, x, y, z, mapBlock);
                return;
            }

            if (!RankContainer.RankListContains(mapBlock.RanksDelete, client.CS.PlayerRanks) && mode == 0) {
                Chat.SendClientChat(client, "§EYou are not allowed to delete this block type.");
                SendBlock(client, x, y, z, mapBlock);
                return;
            }

            if (!RankContainer.RankListContains(newBlock.RanksPlace, client.CS.PlayerRanks) && mode > 0) {
                Chat.SendClientChat(client, "§EYou are not allowed to place this block type.");
                SendBlock(client, x, y, z, mapBlock);
                return;
            }

            ServerCore.Luahandler.RunFunction("E_BlockChange", client, x, y, z, newBlock);
            BlockChange(client.CS.Id, x, y, z, newBlock, mapBlock, true, true, true, 250);
        }

        public void BlockChange(short clientId, short x, short y, short z, Block type, Block lastType, bool undo, bool physics, bool send, short priority) {
            SetBlockId(x, y, z, (byte)(type.Id), clientId);

            if (undo) {
                NetworkClient client;

                ServerCore.Nh.IntLoggeClients.TryGetValue(clientId, out client);

                if (client != null) {
                    if (client.CS.CurrentIndex == -1)
                        client.CS.CurrentIndex = 0;

                    if (client.CS.CurrentIndex != (client.CS.UndoObjects.Count - 1)) {
                        for (var i = client.CS.CurrentIndex; i < client.CS.UndoObjects.Count - 1; i++)
                            client.CS.UndoObjects.RemoveAt(i);
                    }

                    if (client.CS.UndoObjects.Count >= 50000)
                        client.CS.UndoObjects.RemoveAt(0);

                    var newUndo = new Undo {X = x, Y = y, Z = z, OldBlock = lastType, NewBlock = type};

                    client.CS.UndoObjects.Add(newUndo);
                    client.CS.CurrentIndex = client.CS.UndoObjects.Count - 1;
                }
            }

            if (physics) {
                var randomGen = new Random();

                for (short ix = -1; ix < 2; ix++) {
                    for (short iy = -1; iy < 2; iy++) {
                        for (short iz = -1; iz < 2; iz++) {

                            if (!BlockInBounds((short)(x + ix), (short)(y + iy), (short)(z + iz)))
                                continue;

                            var blockQueue = GetBlock((short)(x + ix), (short)(y + iy), (short)(z + iz));

                            if (blockQueue.Physics <= 0 && string.IsNullOrEmpty(blockQueue.PhysicsPlugin)) 
                                continue;
                            
                            PhysicsQueue.Enqueue(new QueueItem((short)(x + ix), (short)(y + iy), (short)(z + iz), DateTime.UtcNow.AddMilliseconds(blockQueue.PhysicsDelay + randomGen.Next(blockQueue.PhysicsRandom))));
                        }
                    }
                }
            }

            if (send)
                BlockchangeQueue.Enqueue(new QueueItem(x, y, z, priority));
        }

        public void MoveBlock(short x, short y, short z, short x2, short y2, short z2, bool undo, bool physics, short priority) {
            if ((0 > x || CWMap.SizeX <= x) || (0 > z || CWMap.SizeY <= z) || (0 > y || CWMap.SizeZ <= y) || (0 > x2 || CWMap.SizeX <= x2) || (0 > z2 || CWMap.SizeY <= z2) || (0 > y2 || CWMap.SizeZ <= y2))
                return;

            var block1 = GetBlock(x, y, z);
            var block2 = GetBlock(x2, y2, z2);

            SetBlockId(x, y, z, 0, -1);
            SetBlockId(x2, y2, z2, (byte)(block1.Id), History.GetLastPlayer(x, z, y));

            if (undo) {
                var lastPlayer = History.GetLastPlayer(x, z, y);
                NetworkClient client;

                ServerCore.Nh.IntLoggeClients.TryGetValue(lastPlayer, out client);

                if (client != null) {
                    if (client.CS.CurrentIndex == -1)
                        client.CS.CurrentIndex = 0;

                    if (client.CS.CurrentIndex != (client.CS.UndoObjects.Count - 1)) {
                        for (var i = client.CS.CurrentIndex; i < client.CS.UndoObjects.Count - 1; i++)
                            client.CS.UndoObjects.RemoveAt(i);
                    }

                    if (client.CS.UndoObjects.Count >= 50000)
                        client.CS.UndoObjects.RemoveAt(0);

                    var newUndo = new Undo {
                        X = x,
                        Y = y,
                        Z = z,
                        OldBlock = block1,
                        NewBlock = ServerCore.Blockholder.GetBlock(0)
                    };
                    var newUndo2 = new Undo {X = x2, Y = y2, Z = z2, OldBlock = block2, NewBlock = block1};

                    client.CS.UndoObjects.Add(newUndo);
                    client.CS.UndoObjects.Add(newUndo2);
                    client.CS.CurrentIndex = client.CS.UndoObjects.Count - 1;
                }
            }

            BlockchangeQueue.Enqueue(new QueueItem(x, y, z, priority));
            BlockchangeQueue.Enqueue(new QueueItem(x2, y2, z2, priority));

            if (!physics) 
                return;

            var randomGen = new Random();

            for (short ix = -1; ix < 2; ix++) {
                for (short iy = -1; iy < 2; iy++) {
                    for (short iz = -1; iz < 2; iz++) {

                        if (!BlockInBounds((short)(x + ix), (short)(y + iy), (short)(z + iz)))
                            continue;

                        if (!BlockInBounds((short)(x2 + ix), (short)(y2 + iy), (short)(z2 + iz)))
                            continue;

                        var blockQueue = GetBlock((short)(x + ix), (short)(y + iy), (short)(z + iz));
                        var blockQueue2 = GetBlock((short)(x2 + ix), (short)(y2 + iy), (short)(z2 + iz));

                        if (blockQueue.Physics > 0 || !string.IsNullOrEmpty(blockQueue.PhysicsPlugin)) 
                            PhysicsQueue.Enqueue(new QueueItem((short)(x + ix), (short)(y + iy), (short)(z + iz), DateTime.UtcNow.AddMilliseconds(blockQueue.PhysicsDelay + randomGen.Next(blockQueue.PhysicsRandom))));
                            

                        if (blockQueue2.Physics <= 0 && string.IsNullOrEmpty(blockQueue2.PhysicsPlugin)) 
                            continue;

                        PhysicsQueue.Enqueue(new QueueItem((short)(x2 + ix), (short)(y2 + iy), (short)(z2 + iz), DateTime.UtcNow.AddMilliseconds(blockQueue2.PhysicsDelay + randomGen.Next(blockQueue2.PhysicsRandom))));
                    }
                }
            }
        }

        public void SendBlock(NetworkClient client, short x, short y, short z, Block type) {
            var setblock = new SetBlockServer {X = x, Y = y, Z = z};

            if (type.CPELevel > client.CS.CustomBlocksLevel)
                setblock.Block = (byte)type.CPEReplace;
            else
                setblock.Block = type.OnClient;

            client.SendQueue.Enqueue(setblock);
        }

        public void SendBlockToAll(short x, short y, short z, Block type) {
            foreach(var c in ClientsList)
                SendBlock(c, x, y, z, type);
        }

        public void BlockQueueLoop() {
            while (ServerCore.Running) {
                if (!HCSettings.Building) { // -- if the map has building disabled.
                    Thread.Sleep(10);
                    continue;
                }

                var changes = 0;

                while (changes <= ServerCore.MaxBlockChanges) {
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

                Thread.Sleep(1);
            }
        }

        public void PhysicsQueueLoop() {
            while (ServerCore.Running) {
                if (!HCSettings.Building && !HCSettings.Physics) {
                    Thread.Sleep(1);
                    continue;
                }

                while (true) {
                    QueueItem physItem;

                    if (!PhysicsQueue.TryDequeue(out physItem))
                        break;

                    if ((physItem.DoneTime - DateTime.UtcNow).Milliseconds > 0) {
                        PhysicsQueue.Enqueue(physItem);
                        continue;
                    }

                    var physicBlock = GetBlock(physItem.X, physItem.Y, physItem.Z);
                    short x = physItem.X, y = physItem.Y, z = physItem.Z;

                    switch (physicBlock.Physics) {
                        case 10:
                            PhysicsOriginalSand(x, y, z);
                            break;
                        case 11:
                            PhysicsD3Sand(x, y, z);
                            break;
                        case 20:
                            PhysicsInfiniteWater(physicBlock, x, y, z);
                            break;
                        case 21:
                            PhysicsFiniteWater(x, y, z);
                            break;
                        case 22:
                            PhysicsSnow(x, y, z);
                            break;
                    }
                }
            Thread.Sleep(1);
        }
    }

        #region Physics Functions
        void PhysicsOriginalSand(short x, short y, short z) {
            if (GetBlockId(x, y, (short)(z - 1)) == 0)
                MoveBlock(x, y, z, x, y, (short)(z - 1), true, true, 1);
        }

        void PhysicsD3Sand(short x, short y, short z) {
            if (GetBlockId(x, y, (short)(z - 1)) == 0)
                MoveBlock(x, y, z, x, y, (short)(z - 1), true, true, 1);
            else if (GetBlockId((short)(x + 1), y, (short)(z - 1)) == 0 && GetBlockId((short)(x + 1), y, z) == 0)
                MoveBlock(x, y, z, (short)(x + 1), y, (short)(z - 1), true, true, 900);
            else if (GetBlockId((short)(x - 1), y, (short)(z - 1)) == 0 && GetBlockId((short)(x - 1), y, z) == 0)
                MoveBlock(x, y, z, (short)(x - 1), y, (short)(z - 1), true, true, 900);
            else if (GetBlockId(x, (short)(y + 1), (short)(z - 1)) == 0 && GetBlockId(x, (short)(y + 1), z) == 0)
                MoveBlock(x, y, z, x, (short)(y + 1), (short)(z - 1), true, true, 900);
            else if (GetBlockId(x, (short)(y - 1), (short)(z - 1)) == 0 && GetBlockId(x, (short)(y - 1), z) == 0)
                MoveBlock(x, y, z, x, (short)(y - 1), (short)(z - 1), true, true, 900);
        }

        void PhysicsInfiniteWater(Block physicBlock, short x, short y, short z) {
            short playerId = -1;

            if (HCSettings.History)
                playerId = History.GetLastPlayer(x, y, z);

            if (GetBlockId(x, y, (short)(z - 1)) == 0)
                BlockChange(playerId, x, y, (short)(z - 1), physicBlock, GetBlock(x, y, (short)(z - 1)), true, true, true, 1);
            else if (GetBlockId((short)(x + 1), y, z) == 0)
                BlockChange(playerId, (short)(x + 1), y, z, physicBlock, GetBlock((short)(x + 1), y, z), true, true, true, 1);
            else if (GetBlockId((short)(x - 1), y, z) == 0)
                BlockChange(playerId, (short)(x - 1), y, z, physicBlock, GetBlock((short)(x - 1), y, z), true, true, true, 1);
            else if (GetBlockId(x, (short)(y + 1), z) == 0)
                BlockChange(playerId, x, (short)(y + 1), z, physicBlock, GetBlock(x, (short)(y + 1), z), true, true, true, 1);
            else if (GetBlockId(x, (short)(y - 1), z) == 0)
                BlockChange(playerId, x, (short)(y - 1), z, physicBlock, GetBlock(x, (short)(y - 1), z), true, true, true, 1);
        }

        void PhysicsFiniteWater(short x, short y, short z) {
            if (GetBlock(x, y, (short)(z - 1)).Name == "Air")
                MoveBlock(x, y, z, x, y, (short)(z - 1), true, true, 2);
            else {
                var fillArray = new int[1024, 1024];
                var fill = new ConcurrentQueue<QueueItem>();
                var found = false;

                fill.Enqueue(new QueueItem(x, y, z, 1));

                while (true) {
                    QueueItem working;

                    if (!fill.TryDequeue(out working))
                        break;

                    if (GetBlockId(working.X, working.Y, (short)(working.Z - 1)) == 0) {
                        MoveBlock(x, y, z, working.X, working.Y, (short)(working.Z - 1), true, true, 2);
                        found = true;
                    } else {
                        if (GetBlockId((short)(working.X + 1), working.Y, working.Z) == 0 && fillArray[working.X + 1, working.Y] == 0) {
                            fillArray[working.X + 1, working.Y] = 1;
                            fill.Enqueue(new QueueItem((short)(working.X + 1), working.Y, working.Z, 1));
                        }

                        if (working.X != 0 && GetBlockId((short)(working.X - 1), working.Y, working.Z) == 0 && fillArray[working.X - 1, working.Y] == 0) {
                            fillArray[working.X * 1, working.Y] = 1;
                            fill.Enqueue(new QueueItem((short)(working.X - 1), working.Y, working.Z, 1));
                        }

                        if (GetBlockId(working.X, (short)(working.Y + 1), working.Z) == 0 && fillArray[working.X, working.Y + 1] == 0) {
                            fillArray[working.X, working.Y + 1] = 1;
                            fill.Enqueue(new QueueItem(working.X, (short)(working.Y + 1), working.Z, 1));
                        }

                        if (working.Y != 0 && GetBlockId(working.X, (short)(working.Y - 1), working.Z) == 0 && fillArray[working.X, working.Y - 1] == 0) {
                            fillArray[working.X, working.Y - 1] = 1;
                            fill.Enqueue(new QueueItem(working.X, (short)(working.Y - 1), working.Z, 1));
                        }

                    }

                    if (fill.Count > 50000 || found)
                        fill = new ConcurrentQueue<QueueItem>();
                }
            }
        }

        void PhysicsSnow(short x, short y, short z) {
            if (GetBlockId(x, y, (short)(z - 1)) == 0 || GetBlockId(x, y, (short)(z - 1)) == 53)
                MoveBlock(x, y, z, x, y, (short)(z - 1), true, true, 1);
        }
        #endregion
        #endregion
        #region FillFunctions
        public void BuildBox(NetworkClient client, short x, short y, short z, short x2, short y2, short z2, Block material, Block replaceMaterial, bool hollow, short priority, bool undo, bool physics) {
            if (x > x2) {
                var temp = x;
                x = x2;
                x2 = temp;
            }
            if (y > y2) {
                var temp = y;
                y = y2;
                y2 = temp;
            }
            if (z > z2) {
                var temp = z;
                z = z2;
                z2 = temp;
            }

            for (var ix = x; ix < x2 + 1; ix++) {
                for (var iy = y; iy < y2 + 1; iy++) {
                    for (var iz = z; iz < z2 + 1; iz++) {
                        if (replaceMaterial.Id == 99 || replaceMaterial == GetBlock(ix, iy, iz)) {
                            if (ix == x || ix == x2 || iy == y || iy == y2 || iz == z || iz == z2)
                                BlockChange(client.CS.Id, ix, iy, iz, material, GetBlock(ix, iy, iz), undo, physics, true, priority);
                            else if (hollow == false)
                                BlockChange(client.CS.Id, ix, iy, iz, material, GetBlock(ix, iy, iz), undo, physics, true, priority);
                        }
                    }
                }
            }

        }

        public void BuildLine(NetworkClient client, short x, short y, short z, short x2, short y2, short z2, Block material, short priority, bool undo, bool physics) {
            var dx = x2 - x;
            var dy = y2 - y;
            var dz = z2 - z;

            var blocks = 1;

            if (blocks < Math.Abs(dx))
                blocks = Math.Abs(dx);

            if (blocks < Math.Abs(dy))
                blocks = Math.Abs(dy);

            if (blocks < Math.Abs(dz))
                blocks = Math.Abs(dz);

            var mx = dx / (float) blocks;
            var my = dy / (float)blocks;
            var mz = dz / (float)blocks;

            for (var i = 0; i < blocks; i++)
                BlockChange(client.CS.Id, (short)(x + mx * i), (short)(y + my * i), (short)(z + mz * i), material, GetBlock((short)(x + mx * i), (short)(y + my * i), (short)(z + mz * i)), undo, physics, true, priority);
        }

        public void BuildSphere(NetworkClient client, short x, short y, short z, float radius, Block material, Block replaceMaterial, bool hollow, short priority, bool undo, bool physics) {
            var rounded = (int)Math.Round(radius, 1);
            var power = (float)Math.Pow(radius, 2);

            for (var ix = -rounded; ix < rounded; ix++) {
                for (var iy = -rounded; iy < rounded; iy++) {
                    for (var iz = -rounded; iz < rounded; iz++) {
                        var squareDistance = (int)(Math.Pow(ix, 2) + Math.Pow(iy, 2) + Math.Pow(iz, 2));

                        if (squareDistance <= power) {
                            var allowed = false;
                            if (hollow) {
                                if (Math.Pow(ix + 1, 2) + Math.Pow(iy, 2) + Math.Pow(iz, 2) > power)
                                    allowed = true;

                                if (Math.Pow(ix - 1, 2) + Math.Pow(iy, 2) + Math.Pow(iz, 2) > power)
                                    allowed = true;

                                if (Math.Pow(ix, 2) + Math.Pow(iy + 1, 2) + Math.Pow(iz, 2) > power)
                                    allowed = true;

                                if (Math.Pow(ix, 2) + Math.Pow(iy - 1, 2) + Math.Pow(iz, 2) > power)
                                    allowed = true;

                                if (Math.Pow(ix, 2) + Math.Pow(iy, 2) + Math.Pow(iz + 1, 2) > power)
                                    allowed = true;

                                if (Math.Pow(ix, 2) + Math.Pow(iy, 2) + Math.Pow(iz - 1, 2) > power)
                                    allowed = true;
                            } else {
                                allowed = true;
                            }

                            if (allowed) {
                                if (replaceMaterial.Id == 99 || replaceMaterial == GetBlock((short)(x + ix), (short)(y + iy), (short)(z + iz)))
                                    BlockChange(client.CS.Id, (short)(x + ix), (short)(y + iy), (short)(z + iz), material, GetBlock((short)(x + ix), (short)(y + iy), (short)(z + iz)), undo, physics, true, priority);
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
