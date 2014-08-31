using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
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
        public Thread DataRunner;
        public ConcurrentQueue<IPacket> SendQueue;

        [NotNull] Dictionary<byte, Func<IPacket>> _packets;
        #endregion

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
        }

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

        public void Login() {
            if (!ServerCore.DB.ContainsPlayer(CS.LoginName))
                ServerCore.DB.CreatePlayer(CS.LoginName, CS.Ip);

            CS.LoginName = ServerCore.DB.GetPlayerName(CS.LoginName);

            if ((ServerCore.DB.GetDatabaseInt(CS.LoginName, "PlayerDB", "Banned") > 0)) { // -- Check if the player is banned
                KickPlayer("Banned: " + ServerCore.DB.GetDatabaseString(CS.LoginName, "PlayerDB", "BanMessage"));
                ServerCore.Logger.Log("Client", "Disconnecting player " + CS.LoginName + ": Player is banned.", LogType.Info);
                return;
            }

            LoadDB(); // -- Load the user's profile

            if (ServerCore.Nh.LoggedClients.ContainsKey(CS.LoginName)) {
                ServerCore.Nh.LoggedClients[CS.LoginName].KickNow("Logged in from another location");

                while (ServerCore.Nh.LoggedClients.ContainsKey(CS.LoginName)) {
                    Thread.Sleep(1);
                }
            }

            // -- Set the user as logged in.
            CS.LoggedIn = true;
            ServerCore.OnlinePlayers += 1;
            ServerCore.Nh.LoggedClients.Add(CS.LoginName, this);
            ServerCore.Nh.CreateShit();

            // -- Get the user logged in to the main map.
            CS.CurrentMap = ServerCore.Maps[ServerCore.MapIndex];
            CS.CurrentMap.Send(this);


            lock (CS.CurrentMap.ClientLock) {
                CS.CurrentMap.Clients.Add(CS.Id, this);
                CS.CurrentMap.CreateClientList();
            }

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

            ESpawn(CS.MyEntity.Name, CS.MyEntity.CreateStub());

            lock (CS.CurrentMap.EntityLock) {
                ServerCore.Logger.Log("Client", "Add Entity: " + CS.MyEntity.Id, LogType.Debug);
                CS.CurrentMap.Entities.Add(CS.MyEntity.Id, CS.MyEntity); // -- Add the entity to the map so that their location will be updated.
                CS.CurrentMap.CreateEntityList();
            }

            // -- CPE stuff
            CPE.SetupExtPlayerList(this);
        }

        public void SendHandshake(string motd = "") {
            var hs = new Handshake {
                Name = ServerCore.ServerName,
                ProtocolVersion = 7,
                Motd = motd == "" ? ServerCore.Motd : motd,
                Usertype = (byte) (CS.Op ? 100 : 0),
            };

            SendQueue.Enqueue(hs);
        }

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

        public void KickNow(string reason) {
            var dc = new Disconnect { Reason = reason };
            dc.Write(this);

            Thread.Sleep(100);

            if (DataRunner.IsAlive)
                DataRunner.Abort();

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
            ServerCore.Nh.CreateShit();

            var remove = new ExtRemovePlayerName { NameId = CS.NameId };
                
            foreach (var c in ServerCore.Nh.ClientList) {
                if (c.CS.CPEExtensions.ContainsKey("ExtPlayerList"))
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

        public void HandleBlockChange(short x, short y, short z, byte mode, byte block) {
            if (CS.Stopped) {
                Chat.SendClientChat(this, "§EYou are stopped, you cannot build.");
                CS.CurrentMap.SendBlock(this, x, y, z, CS.CurrentMap.GetBlock(x, y, z));
                return;
            }

            if ((0 > x || CS.CurrentMap.CWMap.SizeX <= x) || (0 > z || CS.CurrentMap.CWMap.SizeY <= z) || (0 > y || CS.CurrentMap.CWMap.SizeZ <= y)) // -- Out of bounds check.
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

                ServerCore.Luahandler.RunFunction(CS.MyEntity.BuildMode.Plugin, this, CS.CurrentMap, x, y, z, mode, myBlock.Id);
            } else 
                CS.CurrentMap.ClientChangeBlock(this, x, y, z, mode, myBlock);
            
        }

        public void Redo(int steps) {
            if (steps > (CS.UndoObjects.Count - CS.CurrentIndex))
                steps = (CS.UndoObjects.Count - CS.CurrentIndex);

            if (CS.CurrentIndex == CS.UndoObjects.Count - 1)
                return;

            if (CS.CurrentIndex == -1)
                CS.CurrentIndex = 0;

            for (var i = CS.CurrentIndex; i < (CS.CurrentIndex + steps); i++)
                CS.CurrentMap.BlockChange(CS.Id, CS.UndoObjects[i].X, CS.UndoObjects[i].Y, CS.UndoObjects[i].Z, CS.UndoObjects[i].NewBlock, CS.CurrentMap.GetBlock(CS.UndoObjects[i].X, CS.UndoObjects[i].Y, CS.UndoObjects[i].Z), false, false, true, 100);

            CS.CurrentIndex += (steps - 1);
        }

        public void Undo(int steps) {
            if (steps - 1 > (CS.CurrentIndex))
                steps = (CS.CurrentIndex + 1);

            if (CS.CurrentIndex == -1)
                return;

            for (var i = CS.CurrentIndex; i > (CS.CurrentIndex - steps); i--)
                CS.CurrentMap.BlockChange(CS.Id, CS.UndoObjects[i].X, CS.UndoObjects[i].Y, CS.UndoObjects[i].Z, CS.UndoObjects[i].OldBlock, CS.CurrentMap.GetBlock(CS.UndoObjects[i].X, CS.UndoObjects[i].Y, CS.UndoObjects[i].Z), false, false, true, 100);

            CS.CurrentIndex -= (steps - 1);
        }

        public void ChangeMap(HypercubeMap newMap) {
            if (newMap.FreeIds.Count == 0) {
                Chat.SendClientChat(this, "§EYou cannot join this map (It is full).");
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
        #region Entity Management
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

                if (!e.Spawned) { // -- Spawn an entity if it's not there yet.
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

                    if (e.Rot != csEnt.Rot || e.Look != csEnt.Look) {
                        csEnt.Rot = e.Rot;
                        csEnt.Look = e.Look;

                        if (!csEnt.Changed)
                            csEnt.Looked = true;
                    }
                }
            }

            if (CS.MyEntity.SendOwn)
                EFullMove(CS.MyEntity, true);
        }

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
            //Spawn.Write(this);
        }

        void EDelete(sbyte id) {
            var despawn = new DespawnPlayer {PlayerId = id};
            SendQueue.Enqueue(despawn);
        }

        void ELook(sbyte id, byte rot, byte look) {
            var oUp = new OrientationUpdate {PlayerId = id, Yaw = rot, Pitch = look};
            SendQueue.Enqueue(oUp);
        }

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

        void DataHandler() {
            while (BaseSocket.Connected) {
                if (BaseStream == null) {
                    ServerCore.Nh.HandleDisconnect(this);
                    return;
                }

                if (BaseStream.DataAvailable) {
                    var opCode = WSock.ReadByte();

                    if (!_packets.ContainsKey(opCode)) {
                        KickPlayer("Invalid packet received.");
                        ServerCore.Logger.Log("Client", "Invalid packet received: " + opCode, LogType.Warning);
                    }

                    CS.LastActive = DateTime.UtcNow;

                    var incoming = _packets[opCode]();
                    incoming.Read(this);

                    //try {
                        incoming.Handle(this);
                    //} catch (IOException e) {
                    //    ServerCore.Logger.Log("Client", e.Message, LogType.Error);
                    //    ServerCore.Logger.Log("Client", e.StackTrace, LogType.Debug);
                    //}
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
