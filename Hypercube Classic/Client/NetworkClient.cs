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
            Handshake.Usertype = 0; //TODO: Implement this.
            Handshake.Write(this);
        }

        
        /// <summary>
        /// Performs basic login functions for this client. 
        /// </summary>
        public void Login() {
            if (!ServerCore.Database.ContainsPlayer(CS.LoginName)) // -- Create the user in the PlayerDB.
                ServerCore.Database.CreatePlayer(CS.LoginName, CS.IP);

            //TODO: Check if player banned.
            CS.LoggedIn = true;
            //TODO: Get formatted name from PlayerDB
            //TODO: Set Logins, and IP to PlayerDB.
            CS.CurrentMap = ServerCore.MainMap;

            // -- Finds our main map, and sends it to the client.
            foreach (HypercubeMap m in ServerCore.Maps) {
                if (CS.CurrentMap == m.Map.MapName) {
                    m.SendMap(this);
                    m.Clients += 1;
                    break;
                }
            }

            ServerCore.Logger._Log("Info", "Client", "Player logged in. (Name = " + CS.LoginName + ")");

            Chat.SendGlobalChat(ServerCore, "&ePlayer " + CS.LoginName + " has joined.");
            //TODO: Send Entity to all.
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
                        if (Packets.ContainsKey(PacketType) == false) {
                            // -- Kick player, unknown packet received.

                        }

                        var IncomingPacket = Packets[PacketType]();
                        IncomingPacket.Read(this);
                        IncomingPacket.Handle(this, ServerCore);
                    }
                }

            } catch {

            }
        }
    }
}
