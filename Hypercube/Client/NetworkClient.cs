using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data;
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
            // -- Creates an object for holding all of the settings for this client.
            // -- This is to declutter the main client class of tons of variables.
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

            BaseSocket = baseSock; // -- Sets up the socket
            BaseStream = BaseSocket.GetStream();

            WSock = new ClassicWrapped.ClassicWrapped {Stream = BaseStream};

            SendQueue = new ConcurrentQueue<IPacket>(); // -- Send queue for outgoing packets.
            Populate(); // -- Populates reconizable client -> server packets.

            DataRunner = new Thread(DataHandler); // -- Thread for handling incoming data, sending data from the send queue, and updating entity positions.
            DataRunner.Start();

            EntityThread = new Thread(ExtrasHandler); // -- Thread for checking position for teleporters and kill blocks.
            EntityThread.Start();
        }

        /// <summary>
        /// Loads the client's settings from the database.
        /// </summary>
        public void LoadDB(DataTable playerObj) {
            CS.Id = Convert.ToInt16(playerObj.Rows[0]["Number"]);
            CS.Stopped = (Convert.ToInt32(playerObj.Rows[0]["Stopped"]) > 0); // -- If stopped, player cannot build.
            CS.Global = (Convert.ToInt32(playerObj.Rows[0]["Global"]) > 0); // -- Global chat or map only chat.

            if (playerObj.Rows[0]["Time_Muted"].GetType() != typeof(DBNull))
                CS.MuteTime = Convert.ToInt32(playerObj.Rows[0]["Time_Muted"]);

            CS.PlayerRanks = RankContainer.SplitRanks((string)playerObj.Rows[0]["Rank"]); // -- The ranks the player has
            CS.RankSteps = RankContainer.SplitSteps((string)playerObj.Rows[0]["RankStep"]); // -- The rank of each rank. (confusing huh?)

            // -- Players name with color codes.
            CS.FormattedName = CS.PlayerRanks[CS.PlayerRanks.Count - 1].Prefix + CS.LoginName + CS.PlayerRanks[CS.PlayerRanks.Count - 1].Suffix;

            foreach (var r in CS.PlayerRanks) { // -- Determines if the player is op or not.
                if (!r.Op) continue;
                CS.Op = true;
                break;
            }

            var loginCounter = 0;

            if (playerObj.Rows[0]["LoginCounter"].GetType() != typeof (DBNull))
                loginCounter = (Convert.ToInt32(playerObj.Rows[0]["LoginCounter"]) + 1);

            ServerCore.DB.SetDatabase(CS.LoginName, "PlayerDB", "LoginCounter", loginCounter); // -- Increase the login counter for the client
            ServerCore.DB.SetDatabase(CS.LoginName, "PlayerDB", "IP", CS.Ip); // -- Update the IP for this user.
        }

        /// <summary>
        /// Performs functions to set the client as fully logged in.
        /// </summary>
        public void Login() {
            if (!ServerCore.DB.ContainsPlayer(CS.LoginName)) // -- If the client doesn't exist in the database
                ServerCore.DB.CreatePlayer(CS.LoginName, CS.Ip); // -- They are new! Create an entry.

            CS.LoginName = ServerCore.DB.GetPlayerName(CS.LoginName); // -- Get their case-correct username from the DB

            // -- Reduces number of queries to one, thus drastically speeding up login time for clients :)
            var dbEntry = ServerCore.DB.GetDataTable("SELECT * FROM PlayerDB WHERE Name='" + CS.LoginName + "' LIMIT 1");

            if ((Convert.ToInt32(dbEntry.Rows[0]["Banned"]) > 0)) { // -- Check if the player is banned
                KickPlayer("Banned: " + (string)(dbEntry.Rows[0]["BanMessage"]));
                ServerCore.Logger.Log("Client", "Disconnecting player " + CS.LoginName + ": Player is banned.", LogType.Info);
                return;
            }

            LoadDB(dbEntry); // -- Load the user's profile

            // -- Check to ensure noone with the same name is already logged in
            NetworkClient client;

            if (ServerCore.Nh.LoggedClients.TryGetValue(CS.LoginName, out client)) {
                client.KickNow("Logged in from another location");

                while (ServerCore.Nh.LoggedClients.TryGetValue(CS.LoginName, out client))
                    Thread.Sleep(1);
            }

            // -- Set the user as logged in.
            CS.LoggedIn = true;
            ServerCore.Nh.LoggedClients.Add(CS.LoginName, this); // -- Add them to the list of tracked fully-logged clients
            ServerCore.Nh.IntLoggedClients.Add(CS.Id, this);
            ServerCore.Nh.CreateLists();
            CS.NameId = ServerCore.FreeIds.Pop(); // -- Assign the player a nameid for ExtPlayerList
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
            var sv = CS.CurrentMap.GetSpawnVector();
            sv.X *= 32;
            sv.Y *= 32;
            sv.Z *= 32;
            sv.Z += 51;

            CS.MyEntity = new Entity(CS.CurrentMap, CS.LoginName, sv, CS.CurrentMap.CWMap.SpawnRotation, CS.CurrentMap.CWMap.SpawnLook)
            {
                MyClient = this,
                Boundblock = ServerCore.Blockholder.GetBlock(Convert.ToInt32(dbEntry.Rows[0]["BoundBlock"]))
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
        /// Other players are simply told the client left.
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
            if (CS.Stopped) { // -- Check if the player is disabled from building 
                Chat.SendClientChat(this, "§EYou are stopped, you cannot build.");
                CS.CurrentMap.SendBlock(this, x, y, z, CS.CurrentMap.GetBlock(x, y, z)); // -- Resend the block they changed.
                return;
            }

            if (!CS.CurrentMap.BlockInBounds(x, y, z)) // -- Out of bounds check.
                return;

            var myBlock = ServerCore.Blockholder.GetBlock(block);

            if (myBlock == CS.MyEntity.Boundblock && CS.MyEntity.BuildMaterial.Name != "Unknown") // -- If there is a bound block, change the material.
                myBlock = CS.MyEntity.BuildMaterial;

            CS.MyEntity.Lastmaterial = myBlock; // -- Track the last placed material (can give legacy clients a kind of heldblock, and used for /place.)

            if (CS.MyEntity.BuildMode.Name != "") { // -- Buildmode handling
                CS.MyEntity.ClientState.AddBlock(x, y, z); // -- Add modified blocks to buildmode resend buffer

                if (!ServerCore.BmContainer.Modes.ContainsValue(CS.MyEntity.BuildMode)) {
                    Chat.SendClientChat(this, "§EBuild mode '" + CS.MyEntity.BuildMode + "' not found.");
                    CS.MyEntity.BuildMode = new BmStruct {Name = ""};
                    CS.MyEntity.ClientState.ResendBlocks(this);
                    return;
                }

                if (CS.MyEntity.BuildMode.Plugin != "") // -- Runs lua or internal function for this buildmode.
                    ServerCore.Luahandler.RunFunction(CS.MyEntity.BuildMode.Plugin, this, CS.CurrentMap, x, y, z, mode, myBlock.Id);
                else 
                    CS.MyEntity.BuildMode.Function(this, CS.CurrentMap, new Vector3S {X = x, Y = y, Z = z,}, mode, myBlock);
                
            } else // -- If no buildmode and not stopped, go to the map handler.
                CS.CurrentMap.ClientChangeBlock(this, x, y, z, mode, myBlock);
            
        }

        /// <summary>
        /// Redoes previous block changes.
        /// </summary>
        /// <param name="steps">Number of blocks to redo</param>
        public void Redo(int steps) {
            if (CS.UndoObjects.Count == 0) // -- If the user has nothing in their undo queue, they can't redo nothing!
                return;

            if (steps > (CS.UndoObjects.Count - CS.CurrentIndex)) // -- If the user wants to redo more than they have in their redo/undo queue..
                steps = (CS.UndoObjects.Count - CS.CurrentIndex); // -- Set the steps to be exactly the number they have left in their undo/redo queue.

            if (CS.CurrentIndex == CS.UndoObjects.Count - 1) // -- If we're currently on the latest object in the queue...
                return; // -- Then we can't redo anymore, return.

            if (CS.CurrentIndex == -1) // -- Makes sure we arn't at a negative index (happens sometimes with repeated undos and redos)
                CS.CurrentIndex = 0;

            for (var i = CS.CurrentIndex; i < (CS.CurrentIndex + steps); i++) { // -- Iterates through each block in the range the client wants to redo
                // -- Creates and queues the redone block to be sent to clients, processed by the map, physics, ect.
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
            CS.CurrentIndex += (steps - 1); // -- Update the users current position in the undo system.
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
            if (newMap.FreeIds.Count == 0) { // -- No more free entity ids (1-127) on the map.
                Chat.SendClientChat(this, "§EYou cannot join this map (It is full).");
                return;
            }

            if (!HasAllPermissions(newMap.Joinperms.Values.ToList())) { // -- Check the users permissions
                if (!HasAllPermissions(newMap.Showperms.Values.ToList())) { // -- If they can't see the map, act like it doesn't exist.
                    Chat.SendClientChat(this, "§EMap '" + newMap.CWMap.MapName + "' not found.");
                    return;
                }

                Chat.SendClientChat(this, "§EYou do not have permission to join this map."); // -- If they can see it but they still dont have permission, tell them.
                return;
            }

            // -- Notify clients of the map change.
            Chat.SendMapChat(newMap, "§SPlayer " + CS.FormattedName + " §Schanged to map &f" + newMap.CWMap.MapName + ".", 0, true);
            Chat.SendMapChat(CS.CurrentMap, "§SPlayer " + CS.FormattedName + " §Schanged to map &f" + newMap.CWMap.MapName + ".");
            ServerCore.Luahandler.RunFunction("E_MapChange", this, CS.CurrentMap, newMap);

            lock (CS.CurrentMap.ClientLock) { // -- Update the map client list..
                CS.CurrentMap.Clients.Remove(CS.Id);
                CS.CurrentMap.CreateClientList();
            }

            CS.CurrentMap.DeleteEntity(ref CS.MyEntity); // -- Update the entity..
            CS.CurrentMap = newMap;

            newMap.Send(this); // -- Send the map to the client

            lock (newMap.ClientLock) { // -- Add the client's entity to the new map
                newMap.Clients.Add(CS.Id, this);
                newMap.CreateClientList();
            }

            CS.MyEntity.SetBlockPosition(CS.CurrentMap.GetSpawnVector()); // -- Spawn the entity at the spawn..
            CS.MyEntity.Rot = newMap.CWMap.SpawnRotation;
            CS.MyEntity.Look = newMap.CWMap.SpawnLook;
            CS.MyEntity.Map = newMap;
            CS.MyEntity.ClientId = (byte)newMap.FreeIds.Pop();

            ESpawn(CS.MyEntity.Name, CS.MyEntity.CreateStub());

            lock (newMap.EntityLock) {
                newMap.Entities.Add(CS.MyEntity.Id, CS.MyEntity);
                newMap.CreateEntityList();
            }

            CPE.UpdateExtPlayerList(this); // -- Update CPE listings as needed.
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
            var delete = new List<int>(); // -- List of entities to remove from our personal list of visible entities.

            foreach (var e in CS.Entities.Values) { // -- Loop through our whole list of tracked entities and process them for updating.
                if (e.Map != CS.CurrentMap) { // -- If they're no longer on the same map as us
                    // -- Delete the entity.
                    EDelete((sbyte)e.ClientId);
                    delete.Add(e.Id);
                    continue;
                }

                if (e.Id == CS.MyEntity.Id) { // -- If somehow you ended up with your own entity on the list
                    // -- Delete yourself
                    EDelete((sbyte)e.ClientId);
                    delete.Add(e.Id);
                    continue;
                }

                Entity p;

                if (!CS.CurrentMap.Entities.TryGetValue(e.Id, out p)) { // -- If we can't find this entity on the same map we're on
                    EDelete((sbyte)e.ClientId); // -- Delete the entity.
                    delete.Add(e.Id);
                    continue;
                }

                if (!e.Visible) { // -- If the entity is not visible.
                    EDelete((sbyte)e.ClientId); // -- Delete the entity.
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

                if (p.Model == e.Model) // -- If the entity still has the same model (CPE Changemodel)
                    continue; // -- Continue
                // -- Else, update it!
                e.Model = p.Model;
                EModelChange(e.ClientId, e.Model);
            }

            foreach (var i in delete) 
                CS.Entities.Remove(i); // -- If anyone needs to be removed, remove them. (Avoids collection modification)

            foreach (var e in CS.CurrentMap.EntitysList) { // -- Check the entity list of our map for location updates and new entities.
                EntityStub p;

                if (!CS.Entities.TryGetValue(e.Id, out p)) {
                    if (e.Id != CS.MyEntity.Id)
                        CS.Entities.Add(e.Id, e.CreateStub()); // -- If we do not have them yet, add them!
                    continue;
                }

                var csEnt = CS.Entities[e.Id]; // -- If we do have them..
                // -- Check their location and update if needed
                if (e.Location.X != csEnt.Location.X || e.Location.Y != csEnt.Location.Y || e.Location.Z != csEnt.Location.Z) {
                    csEnt.Location = e.Location;
                    csEnt.Changed = true;
                }

                if (e.Rot == csEnt.Rot && e.Look == csEnt.Look) 
                    continue;

                csEnt.Rot = e.Rot;
                csEnt.Look = e.Look;

                if (!csEnt.Changed)
                    csEnt.Looked = true;
            }

            if (!CS.MyEntity.SendOwn) // -- If we need to send our own entity to ourself (we've been teleported somewhere, or just spawned)
                return;

            EFullMove(CS.MyEntity, true);
            CS.MyEntity.SendOwn = false;
        }

        /// <summary>
        /// Checks if this client is within a teleporter, or needs to be killed by a kill block.
        /// </summary>
        void CheckPosition() {
            var myLoc = CS.MyEntity.GetBlockLocation(); // -- Gets our location in block coords
            var tele = CS.CurrentMap.Teleporters.FindTeleporter(myLoc); // -- Attempts to find a teleporter at our location

            if (tele != null) { // -- If we found a teleporter at our location
                if (tele.DestinationMap != CS.CurrentMap) // -- If the teleporter is on a different map
                    ChangeMap(tele.DestinationMap); // -- Change maps

                CS.MyEntity.SetBlockPosition(tele.Dest); // -- Then update our location to the destination of the teleporter.
                CS.MyEntity.Look = tele.DestLook;
                CS.MyEntity.Rot = tele.DestRot;
                CS.MyEntity.SendOwn = true;
            }

            if (CS.CurrentMap.GetBlock(myLoc.X, myLoc.Y, myLoc.Z).Kills) { // -- If the block at our location kills
                CS.MyEntity.Kill(); // -- THen we should die
                return;
            }

            if (!CS.CurrentMap.GetBlock(myLoc.X, myLoc.Y, (short) (myLoc.Z + 1)).Kills) // -- Or if the block one in front of us kills
                return;

            CS.MyEntity.Kill(); // -- Then we should die also.
        }

        /// <summary>
        /// Despawns all entities from the client and forces them to be re-added on the next entity update
        /// Used primarily by map resends.
        /// </summary>
        public void DespawnAll() {
            foreach (var kv in CS.Entities) {
                EDelete((sbyte)kv.Value.ClientId);
            }

            CS.Entities.Clear();
        }

        /// <summary>
        /// Spawns a new entity on the client.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="entity"></param>
        void ESpawn(string name, EntityStub entity) {
            int extVer;

            if (CS.CPEExtensions.TryGetValue("ExtPlayerList", out extVer) && extVer == 2) {
                CPESpawn(name, entity);
                return;
            }

            var spawn = new SpawnPlayer
            {
                PlayerName = name,
                X = entity.Location.X,
                Y = entity.Location.Y,
                Z = entity.Location.Z,
                Yaw = entity.Rot,
                Pitch = entity.Look
            };

            if (entity.Id == CS.MyEntity.Id)
                spawn.PlayerId = -1;
            else
                spawn.PlayerId = (sbyte)entity.ClientId;
            
            SendQueue.Enqueue(spawn);

            if (!CS.CPEExtensions.TryGetValue("ChangeModel", out extVer) || entity.Model == "default")
                return;

            var modelPack = new ChangeModel {
                EntityId = entity.ClientId,
                ModelName = entity.Model,
            };

            SendQueue.Enqueue(modelPack);
        }

        /// <summary>
        /// Sends a ExtAddEntity2 packet instead of a SpawnPlayer packet, for clients supporting ExtPlayerList v2.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="entity"></param>
        void CPESpawn(string name, EntityStub entity) {
            var spawn2 = new ExtAddEntity2 {
                EntityId = entity.ClientId,
                InGameName = name,
                SkinName = name,
                Spawn = entity.Location,
                SpawnPitch = entity.Look,
                SpawnYaw = entity.Rot,
            };

            if (entity.Id == CS.MyEntity.Id)
                spawn2.EntityId = 255;

            SendQueue.Enqueue(spawn2);

            int extVer;

            if (!CS.CPEExtensions.TryGetValue("ChangeModel", out extVer) || entity.Model == "default")
                return;

            var modelPack = new ChangeModel {
                EntityId = entity.ClientId,
                ModelName = entity.Model,
            };

            SendQueue.Enqueue(modelPack);
        }

        /// <summary>
        /// Updates the CPE model of an entity.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
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
                Location = fullEntity.Location,
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

        /// <summary>
        /// Handles checking of positions for teleporters and kill blocks.
        /// </summary>
        void ExtrasHandler() {
            while (BaseSocket.Connected) {
                if (!CS.LoggedIn || CS.MyEntity == null) {
                    Thread.Sleep(10);
                    continue;
                }

                CheckPosition();
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Handles incoming/outgoing packets and entity positions.
        /// </summary>
        void DataHandler() {
            while (BaseSocket.Connected) { // -- While the client is connected.
                if (BaseStream == null) { // -- If the stream got disconnected..
                    ServerCore.Nh.HandleDisconnect(this);
                    return;
                }

                if (BaseStream.DataAvailable) { // -- If we have data ready to be read
                    var opCode = WSock.ReadByte(); // -- Read the packet ID

                    Func<IPacket> packet;

                    if (!_packets.TryGetValue(opCode, out packet)) { // -- Try to get the packet
                        KickPlayer("Invalid packet received.");
                        ServerCore.Logger.Log("Client", "Invalid packet received: " + opCode, LogType.Warning);
                        return;
                    }

                    CS.LastActive = DateTime.UtcNow; // -- Set the clients active time.

                    var incoming = packet(); // -- Read and handle the incoming packet.
                    incoming.Read(this);
                    incoming.Handle(this);
                }

                try {
                    IPacket myPacket;

                    while (SendQueue.TryDequeue(out myPacket)) { // -- Handle our send queue.
                        myPacket.Write(this);
                    }
                }
                catch (IOException) {
                    ServerCore.Nh.HandleDisconnect(this);
                    break;
                }

                if ((DateTime.UtcNow - CS.LastActive).Seconds > 5 && (DateTime.UtcNow - CS.LastActive).Seconds < 10) { // -- Check to see if the client has timed out.
                    var myPing = new Ping(); // -- If they've been inavtive for 6-9 seconds, ping them for an update.
                    myPing.Write(this);
                } else if ((DateTime.UtcNow - CS.LastActive).Seconds > 10) { // -- If they've been inactive for 10 seconds, consider them disconnected.
                    ServerCore.Logger.Log("Timeout", "Player " + CS.Ip + " timed out.", LogType.Info);
                    KickPlayer("Timed out");
                    return;
                }

                if (CS.LoggedIn) // -- If the client is logged in
                    EntityPositions(); // -- Handle entity positions

                Thread.Sleep(1);
            }

            ServerCore.Nh.HandleDisconnect(this);
        }
        #endregion
    }
}
