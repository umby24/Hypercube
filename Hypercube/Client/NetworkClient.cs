using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Net.Sockets;
using Hypercube.Core;
using Hypercube.Network;
using Hypercube.Map;
using JetBrains.Annotations;

namespace Hypercube.Client {
    public class NetworkClient {
        #region Variables
        public ClientSettings CS;

        public ClassicWrapped.ClassicWrapped WSock;
        public TcpClient BaseSocket;
        public NetworkStream BaseStream;
        public Thread DataRunner, EntityThread;
        public ConcurrentQueue<IPacket> SendQueue;

        [NotNull] Dictionary<byte, Func<IPacket>> _packets;
        #endregion

        /// <summary>
        /// Creates a new client, starts reading and parsing packets, and sets up some base settings for the client.
        /// </summary>
        /// <param name="baseSock"></param>
        /// <param name="ip"></param>
        public NetworkClient(TcpClient baseSock, string ip) {

            CS = new ClientSettings
            {
                LastActive = DateTime.UtcNow,
                Entities = new Dictionary<int, EntityStub>(),
                CPEExtensions = new Dictionary<string, int>(),
                SelectionCuboids = new List<byte>(),
                LoggedIn = false,
                CurrentIndex = 0,
                UndoObjects = new List<Undo>(),
                Ip = ip
            };

            BaseSocket = baseSock;
            BaseStream = BaseSocket.GetStream();

            WSock = new ClassicWrapped.ClassicWrapped {Stream = BaseStream};

            SendQueue = new ConcurrentQueue<IPacket>();
            Populate();

            DataRunner = new Thread(DataHandler);
            DataRunner.Start();

            EntityThread = new Thread(ExtrasHandler);
            EntityThread.Start();
        }

        /// <summary>
        /// Loads the client's settings from the database.
        /// </summary>
        public void LoadDB() {
            CS.Id = (short)ServerCore.DB.GetDatabaseInt(CS.LoginName, "PlayerDB", "Number");
            CS.Stopped = (ServerCore.DB.GetDatabaseInt(CS.LoginName, "PlayerDB", "Stopped") > 0);
            CS.Global = (ServerCore.DB.GetDatabaseInt(CS.LoginName, "PlayerDB", "Global") > 0);
            CS.MuteTime = ServerCore.DB.GetDatabaseInt(CS.LoginName, "PlayerDB", "Time_Muted");

            CS.PlayerRanks = RankContainer.SplitRanks(ServerCore.DB.GetDatabaseString(CS.LoginName, "PlayerDB", "Rank"));
            CS.RankSteps = RankContainer.SplitSteps(ServerCore.DB.GetDatabaseString(CS.LoginName, "PlayerDB", "RankStep"));
            CS.FormattedName = CS.PlayerRanks[CS.PlayerRanks.Count - 1].Prefix + CS.LoginName + CS.PlayerRanks[CS.PlayerRanks.Count - 1].Suffix;

            foreach (var r in CS.PlayerRanks) {
                if (!r.Op) continue;
                CS.Op = true;
                break;
            }

            ServerCore.DB.SetDatabase(CS.LoginName, "PlayerDB", "LoginCounter", (ServerCore.DB.GetDatabaseInt(CS.LoginName, "PlayerDB", "LoginCounter") + 1));
            ServerCore.DB.SetDatabase(CS.LoginName, "PlayerDB", "IP", CS.Ip);
        }

        /// <summary>
        /// Performs functions to set the client as fully logged in.
        /// </summary>
        public void Login() {
            if (!ServerCore.DB.ContainsPlayer(CS.LoginName)) // -- If the client doesn't exist in the database
                ServerCore.DB.CreatePlayer(CS.LoginName, CS.Ip); // -- They are new! Create an entry.

            CS.LoginName = ServerCore.DB.GetPlayerName(CS.LoginName); // -- Get their case-correct username from the DB

            if ((ServerCore.DB.GetDatabaseInt(CS.LoginName, "PlayerDB", "Banned") > 0)) { // -- Check if the player is banned
                KickPlayer("Banned: " + ServerCore.DB.GetDatabaseString(CS.LoginName, "PlayerDB", "BanMessage"));
                ServerCore.Logger.Log("Client", "Disconnecting player " + CS.LoginName + ": Player is banned.", LogType.Info);
                return;
            }

            LoadDB(); // -- Load the user's profile

            // -- Check to ensure noone with the same name is already logged in
            NetworkClient client;

            if (ServerCore.Nh.LoggedClients.TryGetValue(CS.LoginName, out client)) {
                client.KickNow("Logged in from another location");

                while (ServerCore.Nh.LoggedClients.TryGetValue(CS.LoginName, out client))
                    Thread.Sleep(1);
            }

            // -- Set the user as logged in.
            CS.LoggedIn = true;
            ServerCore.Nh.LoggedClients.Add(CS.LoginName, this);
            ServerCore.Nh.IntLoggedClients.Add(CS.Id, this);
            ServerCore.Nh.CreateLists();
            CS.NameId = ServerCore.FreeIds.Pop();
            ServerCore.OnlinePlayers += 1;

            // -- Get the user logged in to the main map.
            CS.CurrentMap = ServerCore.Maps[ServerCore.MapMain];
            CS.CurrentMap.Send(this);

            lock (CS.CurrentMap.ClientLock) {
                CS.CurrentMap.Clients.Add(CS.Id, this);
                CS.CurrentMap.CreateClientList();
            }

            // -- Notify everyone of the client's login
            ServerCore.Logger.Log("Client", "Player logged in. (Name = " + CS.LoginName + ")", LogType.Info);
            ServerCore.Luahandler.RunFunction("E_PlayerLogin", this);

            Chat.SendGlobalChat(ServerCore.TextFormats.SystemMessage + "Player " + CS.FormattedName + ServerCore.TextFormats.SystemMessage + " has joined.");
            Chat.SendClientChat(this, ServerCore.WelcomeMessage);

            // -- Create the user's entity.
            CS.MyEntity = new Entity(CS.CurrentMap, CS.LoginName, (short)(CS.CurrentMap.CWMap.SpawnX * 32), 
                (short)(CS.CurrentMap.CWMap.SpawnZ * 32), (short)((CS.CurrentMap.CWMap.SpawnY * 32) + 51), CS.CurrentMap.CWMap.SpawnRotation, CS.CurrentMap.CWMap.SpawnLook)
            {
                MyClient = this,
                Boundblock =
                    ServerCore.Blockholder.GetBlock(ServerCore.DB.GetDatabaseInt(CS.LoginName, "PlayerDB",
                        "BoundBlock"))
            };

            // -- Send the client the map's spawnpoint.
            ESpawn(CS.MyEntity.Name, CS.MyEntity.CreateStub());

            lock (CS.CurrentMap.EntityLock) {
                CS.CurrentMap.Entities.Add(CS.MyEntity.Id, CS.MyEntity); // -- Add the entity to the map so that their location will be updated.
                CS.CurrentMap.CreateEntityList();
            }

            
            // -- CPE stuff
            CPE.SetupExtPlayerList(this);
        }

        /// <summary>
        /// Sends a handshake to the client in preperation for map sending
        /// </summary>
        /// <param name="motd">Sets the visible MOTD on the client's screen</param>
        public void SendHandshake(string motd = "") {
            var hs = new Handshake {
                Name = ServerCore.ServerName,
                ProtocolVersion = 7,
                Motd = motd == "" ? ServerCore.Motd : motd,
                Usertype = (byte) (CS.Op ? 100 : 0),
            };

            SendQueue.Enqueue(hs);
        }

        /// <summary>
        /// Kicks the player from the server.
        /// </summary>
        /// <param name="reason">The reason the player is being kicked.</param>
        /// <param name="log">if true, sends a message notifing everyone of the kick, and adds it to the player's kick counter.</param>
        public void KickPlayer(string reason, bool log = false) {
            var dc = new Disconnect {Reason = reason};
            SendQueue.Enqueue(dc);

            Thread.Sleep(100);

            if (BaseSocket.Connected)
                BaseSocket.Close();

            if (CS.LoggedIn && log) {
                ServerCore.Logger.Log("Client", CS.LoginName + " has been kicked. (" + reason + ")", LogType.Info);
                ServerCore.Luahandler.RunFunction("E_PlayerKicked", CS.LoginName, reason);
                Chat.SendGlobalChat(CS.FormattedName + ServerCore.TextFormats.SystemMessage + " has been kicked. (" + reason + ")");

                var values = new Dictionary<string, string>
                {
                    {
                        "KickCounter",
                        (ServerCore.DB.GetDatabaseInt(CS.LoginName, "PlayerDB", "KickCounter") + 1).ToString()
                    },
                    {"KickMessage", reason}
                };

                ServerCore.DB.Update("PlayerDB", values, "Name='" + CS.LoginName + "'");
            }

            ServerCore.Nh.HandleDisconnect(this);
        }

        /// <summary>
        /// Blocking kick function that also performs all disconnection functions on the client before returning.
        /// </summary>
        /// <param name="reason">The reason for the player being kicked.</param>
        public void KickNow(string reason) {
            ServerCore.Logger.Log("Client", "Played kicked: " + reason, LogType.Info);
            var dc = new Disconnect { Reason = reason };
            dc.Write(this);

            Thread.Sleep(100);

            if (DataRunner.IsAlive)
                DataRunner.Abort();

            if (EntityThread.IsAlive)
                EntityThread.Abort();

            if (BaseSocket.Connected)
                BaseSocket.Close();

            if (!CS.LoggedIn) 
                return;

            lock (CS.CurrentMap.ClientLock) {
                CS.CurrentMap.Clients.Remove(CS.Id);
                CS.CurrentMap.CreateClientList();
            }

            if (CS.MyEntity != null) {
                CS.CurrentMap.DeleteEntity(ref CS.MyEntity);
                ServerCore.FreeEids.Push((short)CS.MyEntity.Id);
            }

            ServerCore.OnlinePlayers --;
            ServerCore.FreeIds.Push(CS.NameId);
            ServerCore.Nh.LoggedClients.Remove((CS.LoginName));
            ServerCore.Nh.IntLoggedClients.Remove(CS.Id);
            ServerCore.Nh.CreateLists();

            var remove = new ExtRemovePlayerName { NameId = CS.NameId };

            foreach (var c in ServerCore.Nh.ClientList) {
                int extVer;
                if (c.CS.CPEExtensions.TryGetValue("ExtPlayerList", out extVer))
                    c.SendQueue.Enqueue(remove);
            }

            ServerCore.Logger.Log("Network", "Player " + CS.LoginName + " has disconnected.", LogType.Info); // -- Notify of their disconnection.
            ServerCore.Luahandler.RunFunction("E_PlayerDisconnect", CS.LoginName);
            Chat.SendGlobalChat(ServerCore.TextFormats.SystemMessage + "Player " + CS.FormattedName + ServerCore.TextFormats.SystemMessage + " left.");
            CS.LoggedIn = false;

            lock (ServerCore.Nh.ClientLock) {
                ServerCore.Nh.Clients.Remove(this);
            }
        }

        /// <summary>
        /// Handles an incoming block change from this client.
        /// </summary>
        /// <param name="x">X location of block change</param>
        /// <param name="y">Y location of block change</param>
        /// <param name="z">Z location of block change</param>
        /// <param name="mode">Breaking, or placing.</param>
        /// <param name="block">The block being placed</param>
        public void HandleBlockChange(short x, short y, short z, byte mode, byte block) {
            if (CS.Stopped) {
                Chat.SendClientChat(this, "§EYou are stopped, you cannot build.");
                CS.CurrentMap.SendBlock(this, x, y, z, CS.CurrentMap.GetBlock(x, y, z));
                return;
            }

            if (!CS.CurrentMap.BlockInBounds(x, y, z)) // -- Out of bounds check.
                return;

            var myBlock = ServerCore.Blockholder.GetBlock(block);

            if (myBlock == CS.MyEntity.Boundblock && CS.MyEntity.BuildMaterial.Name != "Unknown") // -- If there is a bound block, change the material.
                myBlock = CS.MyEntity.BuildMaterial;

            CS.MyEntity.Lastmaterial = myBlock;

            if (CS.MyEntity.BuildMode.Name != "") {
                CS.MyEntity.ClientState.AddBlock(x, y, z);

                if (!ServerCore.BmContainer.Modes.ContainsValue(CS.MyEntity.BuildMode)) {
                    Chat.SendClientChat(this, "§EBuild mode '" + CS.MyEntity.BuildMode + "' not found.");
                    CS.MyEntity.BuildMode = new BmStruct {Name = ""};
                    CS.MyEntity.ClientState.ResendBlocks(this);
                    return;
                }

                if (CS.MyEntity.BuildMode.Plugin != "")
                    ServerCore.Luahandler.RunFunction(CS.MyEntity.BuildMode.Plugin, this, CS.CurrentMap, x, y, z, mode, myBlock.Id);
                else 
                    CS.MyEntity.BuildMode.Function(this, CS.CurrentMap, new Vector3S {X = x, Y = y, Z = z,}, mode, myBlock);
                
            } else 
                CS.CurrentMap.ClientChangeBlock(this, x, y, z, mode, myBlock);
            
        }

        /// <summary>
        /// Redoes previous block changes.
        /// </summary>
        /// <param name="steps">Number of blocks to redo</param>
        public void Redo(int steps) {
            if (CS.UndoObjects.Count == 0)
                return;

            if (steps > (CS.UndoObjects.Count - CS.CurrentIndex))
                steps = (CS.UndoObjects.Count - CS.CurrentIndex);

            if (CS.CurrentIndex == CS.UndoObjects.Count - 1)
                return;

            if (CS.CurrentIndex == -1)
                CS.CurrentIndex = 0;

            for (var i = CS.CurrentIndex; i < (CS.CurrentIndex + steps); i++) {
                var item = new BlockQueueItem {
                    Last = CS.CurrentMap.GetBlock(CS.UndoObjects[i].X, CS.UndoObjects[i].Y, CS.UndoObjects[i].Z),
                    Map = CS.CurrentMap,
                    Material = CS.UndoObjects[i].NewBlock,
                    Physics = false,
                    PlayerId = CS.Id,
                    Priority = 100,
                    Undo = false,
                    X = CS.UndoObjects[i].X,
                    Y = CS.UndoObjects[i].Y,
                    Z = CS.UndoObjects[i].Z,
                };

                HypercubeMap.ActionQueue.Enqueue(item);
            }
            CS.CurrentIndex += (steps - 1);
        }

        /// <summary>
        /// Undoes previous block changes.
        /// </summary>
        /// <param name="steps">Number of blocks to undo</param>
        public void Undo(int steps) {
            if (CS.UndoObjects.Count == 0)
                return;
            
            if (steps - 1 > (CS.CurrentIndex))
                steps = (CS.CurrentIndex + 1);

            if (CS.CurrentIndex == -1)
                return;

            for (var i = CS.CurrentIndex; i > (CS.CurrentIndex - steps); i--) {
                var item = new BlockQueueItem {
                    Last = CS.CurrentMap.GetBlock(CS.UndoObjects[i].X, CS.UndoObjects[i].Y, CS.UndoObjects[i].Z),
                    Map = CS.CurrentMap,
                    Material = CS.UndoObjects[i].OldBlock,
                    Physics = false,
                    PlayerId = CS.Id,
                    Priority = 100,
                    Undo = false,
                    X = CS.UndoObjects[i].X,
                    Y = CS.UndoObjects[i].Y,
                    Z = CS.UndoObjects[i].Z,
                };

                HypercubeMap.ActionQueue.Enqueue(item);
            }

            CS.CurrentIndex -= (steps - 1);
        }

        /// <summary>
        /// Changes the map the client is on.
        /// </summary>
        /// <param name="newMap">The map to send the client to.</param>
        public void ChangeMap(HypercubeMap newMap) {
            if (newMap.FreeIds.Count == 0) {
                Chat.SendClientChat(this, "§EYou cannot join this map (It is full).");
                return;
            }

            if (!HasAllPermissions(newMap.Joinperms.Values.ToList())) {
                if (!HasAllPermissions(newMap.Showperms.Values.ToList())) {
                    Chat.SendClientChat(this, "§EMap '" + newMap.CWMap.MapName + "' not found.");
                    return;
                }

                Chat.SendClientChat(this, "§EYou do not have permission to join this map.");
                return;
            }

            Chat.SendMapChat(newMap, "§SPlayer " + CS.FormattedName + " §Schanged to map &f" + newMap.CWMap.MapName + ".", 0, true);
            Chat.SendMapChat(CS.CurrentMap, "§SPlayer " + CS.FormattedName + " §Schanged to map &f" + newMap.CWMap.MapName + ".");
            ServerCore.Luahandler.RunFunction("E_MapChange", this, CS.CurrentMap, newMap);

            lock (CS.CurrentMap.ClientLock) {
                CS.CurrentMap.Clients.Remove(CS.Id);
                CS.CurrentMap.CreateClientList();
            }

            CS.CurrentMap.DeleteEntity(ref CS.MyEntity);
            CS.CurrentMap = newMap;

            newMap.Send(this);

            lock (newMap.ClientLock) {
                newMap.Clients.Add(CS.Id, this);
                newMap.CreateClientList();
            }

            CS.MyEntity.X = (short)(newMap.CWMap.SpawnX * 32);
            CS.MyEntity.Y = (short)(newMap.CWMap.SpawnZ * 32);
            CS.MyEntity.Z = (short)((newMap.CWMap.SpawnY * 32) + 51);
            CS.MyEntity.Rot = newMap.CWMap.SpawnRotation;
            CS.MyEntity.Look = newMap.CWMap.SpawnLook;
            CS.MyEntity.Map = newMap;
            CS.MyEntity.ClientId = (byte)newMap.FreeIds.Pop();

            ESpawn(CS.MyEntity.Name, CS.MyEntity.CreateStub());

            lock (newMap.EntityLock) {
                newMap.Entities.Add(CS.MyEntity.Id, CS.MyEntity);
                newMap.CreateEntityList();
            }

            CPE.UpdateExtPlayerList(this);
        }

        /// <summary>
        /// Determines if this client has a given permission
        /// </summary>
        /// <param name="permission">The full permission name ex. map.joinmap</param>
        /// <returns>True if client has permission, false if not.</returns>
        public bool HasPermission(string permission) {
            return CS.PlayerRanks.Any(rank => rank.HasPermission(permission));
        }

        /// <summary>
        /// Determines if this client has all the permissions in the list
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public bool HasAllPermissions(List<Permission> permissions) {
            return permissions.All(permission => HasPermission(permission.Fullname));
        }

        /// <summary>
        /// An overload of the main method providing support for sorted dictionaries
        /// </summary>
        /// <param name="prms"></param>
        /// <returns></returns>
        public bool HasAllPermissions(SortedDictionary<string, Permission> prms) {
            return prms.All(perm => HasPermission(perm.Key));
        }

        #region Entity Management
        /// <summary>
        /// Updates entity positions on this client.
        /// </summary>
        void EntityPositions() {
            var delete = new List<int>();

            foreach (var e in CS.Entities.Values) {
                if (e.Map != CS.CurrentMap) {
                    // -- Delete the entity.
                    EDelete((sbyte)e.ClientId);
                    delete.Add(e.Id);
                    continue;
                }

                if (e.Id == CS.MyEntity.Id) {
                    // -- Delete yourself
                    EDelete((sbyte)e.ClientId);
                    delete.Add(e.Id);
                    continue;
                }

                Entity p;
                if (!CS.CurrentMap.Entities.TryGetValue(e.Id, out p)) {
                    EDelete((sbyte)e.ClientId);
                    delete.Add(e.Id);
                    continue;
                }

                if (!e.Visible) {
                    EDelete((sbyte)e.ClientId);
                    delete.Add(e.Id);
                }

                if (!e.Spawned && e.Visible) { // -- Spawn an entity if it's not there yet.
                    ESpawn(CS.CurrentMap.Entities[e.Id].Name, e);
                    e.Spawned = true;
                }

                if (e.Looked) { // -- If the player looked, send them just an orient update.
                    ELook((sbyte)e.ClientId, CS.CurrentMap.Entities[e.Id].Rot, CS.CurrentMap.Entities[e.Id].Look);
                    e.Looked = false;
                }

                if (e.Changed) { // -- If they moved, send them both.
                    EFullMove(CS.CurrentMap.Entities[e.Id]);
                    e.Changed = false;
                }

                if (p.Model == e.Model) 
                    continue;

                e.Model = p.Model;
                EModelChange(e.ClientId, e.Model);
            }

            foreach (var i in delete) 
                CS.Entities.Remove(i); // -- If anyone needs to be removed, remove them. (Avoids collection modification)

            foreach (var e in CS.CurrentMap.EntitysList) {
                EntityStub p;
                if (!CS.Entities.TryGetValue(e.Id, out p)) {
                    if (e.Id != CS.MyEntity.Id)
                        CS.Entities.Add(e.Id, e.CreateStub()); // -- If we do not have them yet, add them!
                    
                } else {
                    var csEnt = CS.Entities[e.Id];

                    if (e.X != csEnt.X || e.Y != csEnt.Y || e.Z != csEnt.Z) {
                        csEnt.X = e.X;
                        csEnt.Y = e.Y;
                        csEnt.Z = e.Z;
                        csEnt.Changed = true;
                    }

                    if (e.Rot == csEnt.Rot && e.Look == csEnt.Look) 
                        continue;
                    csEnt.Rot = e.Rot;
                    csEnt.Look = e.Look;

                    if (!csEnt.Changed)
                        csEnt.Looked = true;
                }
            }

            if (CS.MyEntity.SendOwn) {
                EFullMove(CS.MyEntity, true);
                CS.MyEntity.SendOwn = false;
            }
        }

        /// <summary>
        /// Checks if this client is within a teleporter, or needs to be killed by a kill block.
        /// </summary>
        void CheckPosition() {
            var myLoc = CS.MyEntity.GetBlockLocation();
            var tele = CS.CurrentMap.Teleporters.FindTeleporter(myLoc);

            if (tele != null) {
                if (tele.DestinationMap != CS.CurrentMap)
                    ChangeMap(tele.DestinationMap);

                CS.MyEntity.X = (short)(tele.Dest.X * 32);
                CS.MyEntity.Y = (short)(tele.Dest.Y * 32);
                CS.MyEntity.Z = (short)((tele.Dest.Z * 32) + 51);
                CS.MyEntity.Look = tele.DestLook;
                CS.MyEntity.Rot = tele.DestRot;
                CS.MyEntity.SendOwn = true;
            }

            //TODO: Kill blocks.
        }

        /// <summary>
        /// Spawns a new entity on the client.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="entity"></param>
        void ESpawn(string name, EntityStub entity) {
            var spawn = new SpawnPlayer
            {
                PlayerName = name,
                X = entity.X,
                Y = entity.Y,
                Z = entity.Z,
                Yaw = entity.Rot,
                Pitch = entity.Look
            };

            if (entity.Id == CS.MyEntity.Id)
                spawn.PlayerId = -1;
            else
                spawn.PlayerId = (sbyte)entity.ClientId;
            
            SendQueue.Enqueue(spawn);

            int extVer;

            if (!CS.CPEExtensions.TryGetValue("ChangeModel", out extVer) || entity.Model == "default")
                return;

            var modelPack = new ChangeModel {
                EntityId = entity.ClientId,
                ModelName = entity.Model,
            };

            SendQueue.Enqueue(modelPack);
        }

        void EModelChange(byte id, string model) {
            int extVer;

            if (!CS.CPEExtensions.TryGetValue("ChangeModel", out extVer))
                return;

            var modelPack = new ChangeModel {
                EntityId = id,
                ModelName = model,
            };

            SendQueue.Enqueue(modelPack);
        }

        /// <summary>
        /// Deletes an entity that was spawned on the client.
        /// </summary>
        /// <param name="id"></param>
        void EDelete(sbyte id) {
            var despawn = new DespawnPlayer {PlayerId = id};
            SendQueue.Enqueue(despawn);
        }

        /// <summary>
        /// Updates the rotation and pitch of an entity.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rot"></param>
        /// <param name="look"></param>
        void ELook(sbyte id, byte rot, byte look) {
            var oUp = new OrientationUpdate {PlayerId = id, Yaw = rot, Pitch = look};
            SendQueue.Enqueue(oUp);
        }

        /// <summary>
        /// Updates the location (absolute) and look of an entity rendered on this client.
        /// </summary>
        /// <param name="fullEntity"></param>
        /// <param name="own"></param>
        void EFullMove(Entity fullEntity, bool own = false) {
            var move = new PlayerTeleport
            {
                X = fullEntity.X,
                Y = fullEntity.Y,
                Z = fullEntity.Z,
                Yaw = fullEntity.Rot,
                Pitch = fullEntity.Look
            };

            if (own)
                move.PlayerId = -1;
            else
                move.PlayerId = (sbyte)fullEntity.ClientId;


            SendQueue.Enqueue(move);
        }
        #endregion
        #region Network functions
        /// <summary>
        /// Populates the list of accetpable packets from the client. Anything other than these will be rejected.
        /// </summary>
        void Populate() {
            _packets = new Dictionary<byte, Func<IPacket>> {
                {0, () => new Handshake()},
                {1, () => new Ping()},
                {5, () => new SetBlock()},
                {8, () => new PlayerTeleport()},
                {13, () => new Message()},
                {16, () => new ExtInfo()},
                {17, () => new ExtEntry()},
                {19, () => new CustomBlockSupportLevel()}
            };
        }

        void ExtrasHandler() {
            while (BaseSocket.Connected) {
                if (!CS.LoggedIn || CS.MyEntity == null) {
                    Thread.Sleep(10);
                    continue;
                }

                CheckPosition();
                //EntityPositions();
                Thread.Sleep(10);
            }
        }

        void DataHandler() {
            while (BaseSocket.Connected) {
                if (BaseStream == null) {
                    ServerCore.Nh.HandleDisconnect(this);
                    return;
                }

                if (BaseStream.DataAvailable) {
                    var opCode = WSock.ReadByte();

                    Func<IPacket> packet;

                    if (!_packets.TryGetValue(opCode, out packet)) {
                        KickPlayer("Invalid packet received.");
                        ServerCore.Logger.Log("Client", "Invalid packet received: " + opCode, LogType.Warning);
                        return;
                    }

                    CS.LastActive = DateTime.UtcNow;

                    var incoming = packet();
                    incoming.Read(this);
                    incoming.Handle(this);
                }

                try {
                    IPacket myPacket;
                    while (SendQueue.TryDequeue(out myPacket)) {
                        myPacket.Write(this);
                    }
                }
                catch (IOException) {
                    ServerCore.Nh.HandleDisconnect(this);
                    break;
                }

                if ((DateTime.UtcNow - CS.LastActive).Seconds > 5 && (DateTime.UtcNow - CS.LastActive).Seconds < 10) {
                    var myPing = new Ping();
                    myPing.Write(this);
                } else if ((DateTime.UtcNow - CS.LastActive).Seconds > 10) {
                    ServerCore.Logger.Log("Timeout", "Player " + CS.Ip + " timed out.", LogType.Info);
                    KickPlayer("Timed out");
                    return;
                }

                if (CS.LoggedIn)
                    EntityPositions();

                Thread.Sleep(1);
            }

            ServerCore.Nh.HandleDisconnect(this);
        }
        #endregion
    }
}
