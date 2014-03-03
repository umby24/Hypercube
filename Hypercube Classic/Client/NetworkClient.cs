using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Hypercube_Classic.Packets;
using Hypercube_Classic.Map;
using Hypercube_Classic.Core;

namespace Hypercube_Classic.Client {
    /// <summary>
    /// A Container for a remotely connected client. Includes user's socket and so on.
    /// </summary>
    public class NetworkClient {
        #region Variables
        public ClassicWrapped.ClassicWrapped wSock;
        public TcpClient BaseSocket;
        public NetworkStream BaseStream;
        public Thread DataRunner;
        public ClientSettings CS;
        public Hypercube ServerCore;
        Dictionary<byte, Func<Packets.IPacket>> Packets;
        #endregion

        public NetworkClient(TcpClient baseSock, Hypercube Core) {
            BaseSocket = baseSock;
            BaseStream = BaseSocket.GetStream();

            ServerCore = Core;

            wSock = new ClassicWrapped.ClassicWrapped();
            wSock._Stream = BaseStream;

            Populate();

            CS = new ClientSettings();
            CS.CPEExtensions = new Dictionary<string, int>();
            CS.SelectionCuboids = new List<byte>();
            CS.LoggedIn = false;

            DataRunner = new Thread(DataHandler);
            DataRunner.Start();
        }

        /// <summary>
        /// As the name implies, sends a Minecraft handshake to the user. Mostly used for map sends.
        /// </summary>
        public void SendHandshake() {
            var Handshake = new Handshake();
            Handshake.Name = ServerCore.ServerName;
            Handshake.MOTD = ServerCore.MOTD;
            Handshake.ProtocolVersion = 7;

            if (CS.Op)
                Handshake.Usertype = 64;
            else
                Handshake.Usertype = 0;

            Handshake.Write(this);
        }

        public void KickPlayer(string Reason) {
            var Disconnect = new Disconnect();
            Disconnect.Reason = Reason;
            Disconnect.Write(this);

            if (BaseSocket.Connected == true)
                BaseSocket.Close();

            BaseStream.Close();
            BaseStream.Dispose();

            var Values = new Dictionary<string, string>(); // -- Update the PlayerDB.
            Values.Add("KickCounter", (ServerCore.Database.GetDatabaseInt(CS.LoginName, "PlayerDB", "KickCounter") + 1).ToString());
            Values.Add("KickMessage", Reason);
            ServerCore.Database.Update("PlayerDB", Values, "Name='" + CS.LoginName + "'");

            //ServerCore.nh.HandleDisconnect(this);
        }

        /// <summary>
        /// Performs basic login functions for this client. 
        /// </summary>
        public void Login() {
            if (!ServerCore.Database.ContainsPlayer(CS.LoginName)) // -- Create the user in the PlayerDB.
                ServerCore.Database.CreatePlayer(CS.LoginName, CS.IP, ServerCore);

            CS.LoginName = ServerCore.Database.GetPlayerName(CS.LoginName);

            if ((ServerCore.Database.GetDatabaseInt(CS.LoginName, "PlayerDB", "Banned") > 0)) {
                var Disconnecter = new Disconnect();

                Disconnecter.Reason = "Banned: " + ServerCore.Database.GetDatabaseString(CS.LoginName, "PlayerDB", "BanMessage");
                Disconnecter.Write(this);

                if (BaseSocket.Connected == true)
                    BaseSocket.Close();

                BaseStream.Close();
                BaseStream.Dispose();

                ServerCore.Logger._Log("Info", "Client", "Disconnecting player " + CS.LoginName + ": Player is banned.");
                return;
            }

            //TODO: Load From PlayerDB.
            CS.ID = ServerCore.Database.GetDatabaseInt(CS.LoginName, "PlayerDB", "Number");
            CS.Stopped = (ServerCore.Database.GetDatabaseInt(CS.LoginName, "PlayerDB", "Stopped") > 0);
            CS.Global = (ServerCore.Database.GetDatabaseInt(CS.LoginName, "PlayerDB", "Global") > 0);
            CS.MuteTime = ServerCore.Database.GetDatabaseInt(CS.LoginName, "PlayerDB", "Time_Muted");
            
            CS.LoggedIn = true;
            
            CS.PlayerRanks = RankContainer.SplitRanks(ServerCore, ServerCore.Database.GetDatabaseString(CS.LoginName, "PlayerDB", "Rank"));
            CS.RankSteps = RankContainer.SplitSteps(ServerCore.Database.GetDatabaseString(CS.LoginName, "PlayerDB", "RankStep"));
            CS.FormattedName = CS.PlayerRanks[CS.PlayerRanks.Count - 1].Prefix + CS.LoginName + CS.PlayerRanks[CS.PlayerRanks.Count - 1].Suffix;

            foreach (Rank r in CS.PlayerRanks) {
                if (r.Op) {
                    CS.Op = true;
                    break;
                }
            }

            ServerCore.Database.SetDatabase(CS.LoginName, "PlayerDB", "LoginCounter", (ServerCore.Database.GetDatabaseInt(CS.LoginName, "PlayerDB", "LoginCounter") + 1));
            ServerCore.Database.SetDatabase(CS.LoginName, "PlayerDB", "IP", CS.IP);

            CS.CurrentMap = ServerCore.MainMap;

            // -- Finds our main map, and sends it to the client.

            CS.CurrentMap.SendMap(this);
            CS.CurrentMap.Clients.Add(this); // -- Add the client to the map

            CS.MyEntity = new Entity(ServerCore, CS.CurrentMap, CS.LoginName, (short)(CS.CurrentMap.Map.SpawnX * 32), (short)(CS.CurrentMap.Map.SpawnZ * 32), (short)((CS.CurrentMap.Map.SpawnY * 32) + 51), CS.CurrentMap.Map.SpawnRotation, CS.CurrentMap.Map.SpawnLook); // -- Create the entity..
            CS.MyEntity.MyClient = this;
            CS.MyEntity.Boundblock = ServerCore.Blockholder.GetBlock(ServerCore.Database.GetDatabaseInt(CS.LoginName, "PlayerDB", "BoundBlock"));
            CS.CurrentMap.SpawnEntity(CS.MyEntity); // -- Send the client spawn to everyone.
            CS.CurrentMap.Entities.Add(CS.MyEntity); // -- Add the entity to the map so that their location will be updated.

            CS.CurrentMap.SendAllEntities(this);

            ServerCore.Logger._Log("Info", "Client", "Player logged in. (Name = " + CS.LoginName + ")");

            Chat.SendGlobalChat(ServerCore, "&ePlayer " + CS.FormattedName + "&e has joined.");
            Chat.SendClientChat(this, ServerCore.WelcomeMessage);
            //TODO: CPE ExtPlayerList
            ServerCore.OnlinePlayers += 1;
        }

        /// <summary>
        /// Populates the list of accetpable packets from the client. Anything other than these will be rejected.
        /// </summary>
        void Populate() {
            Packets = new Dictionary<byte, Func<Packets.IPacket>> {
                {0, () => new Handshake()},
                {5, () => new SetBlock()},
                {8, () => new PlayerTeleport()},
                {13, () => new Message()},
                {16, () => new ExtInfo()},
                {17, () => new ExtEntry()},
                {19, () => new CustomBlockSupportLevel()}
            };
        }

        /// <summary>
        /// Blocks until data is received, then handles that data.
        /// </summary>
        void DataHandler() {
            try {
                byte PacketType = 255;

                while ((PacketType = wSock.ReadByte()) != 255) {
                    if (BaseSocket.Connected == true) {
                        if (Packets.ContainsKey(PacketType) == false) // -- Kick player, unknown packet received.
                            KickPlayer("Invalid packet received: " + PacketType.ToString());

                        var IncomingPacket = Packets[PacketType]();
                        IncomingPacket.Read(this);
                        IncomingPacket.Handle(this, ServerCore);
                    }
                }

            } catch (Exception e) {
                if (e.GetType() != typeof(System.IO.IOException)) {
                    ServerCore.Logger._Log("Error", "Dunno", e.Message);
                    ServerCore.Logger._Log("Error", "Dunno", e.StackTrace);
                }

                // -- User probably disconnected.
                if (BaseSocket.Connected == true)
                    BaseSocket.Close();

                BaseStream.Close();
                BaseStream.Dispose();

                ServerCore.nh.HandleDisconnect(this);
            }
        }
    }
}
