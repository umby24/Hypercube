using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
        public Thread DataRunner, TimeoutThread;
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
            CS.Id = (short)Hypercube.DB.GetDatabaseInt(CS.LoginName, "PlayerDB", "Number");
            CS.Stopped = (Hypercube.DB.GetDatabaseInt(CS.LoginName, "PlayerDB", "Stopped") > 0);
            CS.Global = (Hypercube.DB.GetDatabaseInt(CS.LoginName, "PlayerDB", "Global") > 0);
            CS.MuteTime = Hypercube.DB.GetDatabaseInt(CS.LoginName, "PlayerDB", "Time_Muted");

            CS.PlayerRanks = RankContainer.SplitRanks(Hypercube.DB.GetDatabaseString(CS.LoginName, "PlayerDB", "Rank"));
            CS.RankSteps = RankContainer.SplitSteps(Hypercube.DB.GetDatabaseString(CS.LoginName, "PlayerDB", "RankStep"));
            CS.FormattedName = CS.PlayerRanks[CS.PlayerRanks.Count - 1].Prefix + CS.LoginName + CS.PlayerRanks[CS.PlayerRanks.Count - 1].Suffix;

            foreach (var r in CS.PlayerRanks) {
                if (!r.Op) continue;
                CS.Op = true;
                break;
            }

            Hypercube.DB.SetDatabase(CS.LoginName, "PlayerDB", "LoginCounter", (Hypercube.DB.GetDatabaseInt(CS.LoginName, "PlayerDB", "LoginCounter") + 1));
            Hypercube.DB.SetDatabase(CS.LoginName, "PlayerDB", "IP", CS.Ip);
        }

        public void Login() {
            if (!Hypercube.DB.ContainsPlayer(CS.LoginName))
                Hypercube.DB.CreatePlayer(CS.LoginName, CS.Ip);

            CS.LoginName = Hypercube.DB.GetPlayerName(CS.LoginName);

            if ((Hypercube.DB.GetDatabaseInt(CS.LoginName, "PlayerDB", "Banned") > 0)) { // -- Check if the player is banned
                KickPlayer("Banned: " + Hypercube.DB.GetDatabaseString(CS.LoginName, "PlayerDB", "BanMessage"));
                Hypercube.Logger.Log("Client", "Disconnecting player " + CS.LoginName + ": Player is banned.", LogType.Info);
                return;
            }

            LoadDB(); // -- Load the user's profile

            if (Hypercube.Nh.LoggedClients.ContainsKey(CS.LoginName)) {
                //TODO: This
            }

            // -- Get the user logged in to the main map.
            CS.CurrentMap = Hypercube.Maps[Hypercube.MapIndex];
            CS.CurrentMap.Send(this);

            lock (CS.CurrentMap.ClientLock) {
                CS.CurrentMap.Clients.Add(CS.Id, this);
                CS.CurrentMap.CreateClientList();
            }

            Hypercube.Logger.Log("Client", "Player logged in. (Name = " + CS.LoginName + ")", LogType.Info);
            Hypercube.Luahandler.RunFunction("E_PlayerLogin", this);

            Chat.SendGlobalChat(Hypercube.TextFormats.SystemMessage + "Player " + CS.FormattedName + Hypercube.TextFormats.SystemMessage + " has joined.");
            Chat.SendClientChat(this, Hypercube.WelcomeMessage);

            // -- Create the user's entity.
            CS.MyEntity = new Entity(CS.CurrentMap, CS.LoginName, (short)(CS.CurrentMap.CWMap.SpawnX * 32), 
                (short)(CS.CurrentMap.CWMap.SpawnZ * 32), (short)((CS.CurrentMap.CWMap.SpawnY * 32) + 51), CS.CurrentMap.CWMap.SpawnRotation, CS.CurrentMap.CWMap.SpawnLook)
            {
                MyClient = this,
                Boundblock =
                    Hypercube.Blockholder.GetBlock(Hypercube.DB.GetDatabaseInt(CS.LoginName, "PlayerDB",
                        "BoundBlock"))
            };

            ESpawn(CS.MyEntity.Name, CS.MyEntity.CreateStub());

            lock (CS.CurrentMap.EntityLock) {
                CS.CurrentMap.Entities.Add(CS.MyEntity.Id, CS.MyEntity); // -- Add the entity to the map so that their location will be updated.
                CS.CurrentMap.CreateEntityList();
            }

            // -- CPE stuff
            CPE.SetupExtPlayerList(this);

            // -- Set the user as logged in.
            CS.LoggedIn = true;
            Hypercube.OnlinePlayers += 1;
            Hypercube.Nh.LoggedClients.Add(CS.LoginName, this);
            Hypercube.Nh.CreateShit();
        }

        public void SendHandshake(string motd = "") {
            var hs = new Handshake
            {
                Name = Hypercube.ServerName,
                ProtocolVersion = 7,
                Motd = motd == "" ? Hypercube.Motd : motd,
            };

            hs.Usertype = (byte)(CS.Op ? 100 : 0);

            SendQueue.Enqueue(hs);
        }

        public void KickPlayer(string reason, bool log = false) {
            var dc = new Disconnect {Reason = reason};
            SendQueue.Enqueue(dc);

            Thread.Sleep(100);

            if (BaseSocket.Connected)
                BaseSocket.Close();

            if (CS.LoggedIn && log) {
                Hypercube.Logger.Log("Client", CS.LoginName + " has been kicked. (" + reason + ")", LogType.Info);
                Hypercube.Luahandler.RunFunction("E_PlayerKicked", CS.LoginName, reason);
                Chat.SendGlobalChat(CS.FormattedName + Hypercube.TextFormats.SystemMessage + " has been kicked. (" + reason + ")");

                var values = new Dictionary<string, string>
                {
                    {
                        "KickCounter",
                        (Hypercube.DB.GetDatabaseInt(CS.LoginName, "PlayerDB", "KickCounter") + 1).ToString()
                    },
                    {"KickMessage", reason}
                };

                Hypercube.DB.Update("PlayerDB", values, "Name='" + CS.LoginName + "'");
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

            var myBlock = Hypercube.Blockholder.GetBlock(block);

            if (myBlock == CS.MyEntity.Boundblock && CS.MyEntity.BuildMaterial.Name != "Unknown") // -- If there is a bound block, change the material.
                myBlock = CS.MyEntity.BuildMaterial;

            CS.MyEntity.Lastmaterial = myBlock;

            if (CS.MyEntity.BuildMode.Name != "") {
                CS.MyEntity.ClientState.AddBlock(x, y, z);

                if (!Hypercube.BmContainer.Modes.ContainsValue(CS.MyEntity.BuildMode)) {
                    Chat.SendClientChat(this, "§EBuild mode '" + CS.MyEntity.BuildMode + "' not found.");
                    CS.MyEntity.BuildMode = new BmStruct {Name = ""};
                    CS.MyEntity.ClientState.ResendBlocks(this);
                    return;
                }

                Hypercube.Luahandler.RunFunction(CS.MyEntity.BuildMode.Plugin, this, CS.CurrentMap, x, y, z, mode, myBlock.Id);
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
            Chat.SendMapChat(newMap, "§SPlayer " + CS.FormattedName + " §Schanged to map &f" + newMap.CWMap.MapName + ".", 0, true);
            Chat.SendMapChat(CS.CurrentMap, "§SPlayer " + CS.FormattedName + " §Schanged to map &f" + newMap.CWMap.MapName + ".");
            Hypercube.Luahandler.RunFunction("E_MapChange", this, CS.CurrentMap, newMap);

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

            if (newMap.FreeId != 128) {
                CS.MyEntity.ClientId = (byte)newMap.FreeId;

                if (newMap.FreeId != newMap.NextId)
                    newMap.FreeId = newMap.NextId;
                else {
                    newMap.FreeId += 1;
                    newMap.NextId = newMap.FreeId;
                }
            }

            ESpawn(CS.MyEntity.Name, CS.MyEntity.CreateStub());

            lock (newMap.EntityLock) {
                newMap.Entities.Add(CS.Id, CS.MyEntity);
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

                if (!CS.CurrentMap.Entities.ContainsKey(e.Id)) { // -- Delete old entities.
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
                if (!CS.Entities.ContainsKey(e.Id)) {
                    if (e.Id != CS.MyEntity.Id)
                        CS.Entities.Add(e.Id, CS.CurrentMap.Entities[e.Id].CreateStub()); // -- If we do not have them yet, add them!
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
                if (BaseStream.DataAvailable) {
                    var opCode = WSock.ReadByte();

                    if (!_packets.ContainsKey(opCode)) {
                        KickPlayer("Invalid packet received.");
                        Hypercube.Logger.Log("Client", "Invalid packet received: " + opCode, LogType.Warning);
                    }

                    CS.LastActive = DateTime.UtcNow;

                    var incoming = _packets[opCode]();
                    incoming.Read(this);

                    try {
                        incoming.Handle(this);
                    } catch (Exception e) {
                        Hypercube.Logger.Log("Client", e.Message, LogType.Error);
                        Hypercube.Logger.Log("Client", e.StackTrace, LogType.Debug);
                    }
                }

                IPacket myPacket;

                while (SendQueue.TryDequeue(out myPacket)) {
                    myPacket.Write(this);
                }

                if ((DateTime.UtcNow - CS.LastActive).Seconds > 500 && (DateTime.UtcNow - CS.LastActive).Seconds < 1000) {
                    var myPing = new Ping();
                    myPing.Write(this);
                } else if ((DateTime.UtcNow - CS.LastActive).Seconds > 1000) {
                    Hypercube.Logger.Log("Timeout", "Player " + CS.Ip + " timed out.", LogType.Info);
                    KickPlayer("Timed out");
                    return;
                }

                if (CS.LoggedIn)
                    EntityPositions();

                Thread.Sleep(1);
            }

            Hypercube.Nh.HandleDisconnect(this);
        }
        #endregion
    }
}
