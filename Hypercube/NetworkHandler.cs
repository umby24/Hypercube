using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Hypercube.Core;
using Hypercube.Libraries;
using Hypercube.Client;
using Hypercube.Network;

namespace Hypercube {
    /// <summary>
    /// Handles incoming client connections, client disconnections, and some basic settings relating to the server networking
    /// </summary>
    public class NetworkHandler {
        #region Variables
        public Settings Ns;
        public List<NetworkClient> Clients = new List<NetworkClient>();
        public Dictionary<string, NetworkClient> LoggedClients = new Dictionary<string, NetworkClient>(StringComparer.InvariantCultureIgnoreCase);
        public Dictionary<int, NetworkClient> IntLoggedClients = new Dictionary<int, NetworkClient>(); 
        public NetworkClient[] ClientList;

        public TcpListener CoreListener;
        public object ClientLock = new object();

        // -- Network Settings
        public int Port, MaxPlayers, MaxPerIp;
        public bool VerifyNames, Public;

        Thread _listenThread;
        #endregion

        /// <summary>
        /// Creates a new NetworkHandler Instance, and loads network settings.
        /// </summary>
        public NetworkHandler() {
            Ns = new Settings("Network.txt", LoadSettings);
            ServerCore.Setting.RegisterFile(Ns);
            Ns.LoadFile();
            CreateLists();
        }

        /// <summary>
        /// Creates the readonly list of currently logged in clients.
        /// </summary>
        public void CreateLists() {
            ClientList = LoggedClients.Values.ToArray();
        }

        /// <summary>
        /// Loads network settings from file (port, max players, ect.)
        /// </summary>
        void LoadSettings() {
            Port = int.Parse(Ns.Read("Port", "25565"));
            MaxPlayers = int.Parse(Ns.Read("MaxPlayers", "128"));
            VerifyNames = bool.Parse(Ns.Read("VerifyNames", "true"));
            Public = bool.Parse(Ns.Read("Public", "true"));
            MaxPerIp = int.Parse(Ns.Read("MaxPerIP", "5"));

            ServerCore.Logger.Log("Network", "Network settings loaded.", LogType.Info);
        }

        /// <summary>
        /// Saves network settings to file.
        /// </summary>
        public void SaveSettings() {
            Ns.Write("Port", Port.ToString());
            Ns.Write("MaxPlayers", MaxPlayers.ToString());
            Ns.Write("VerifyNames", VerifyNames.ToString());
            Ns.Write("Public", Public.ToString());
        }

        /// <summary>
        /// Starts listening for outside connections.
        /// </summary>
        public void Start() {
            if (ServerCore.Running)
                Stop();

            CoreListener = new TcpListener(IPAddress.Any, Port);
            ServerCore.Running = true;
            CoreListener.Start();

            _listenThread = new Thread(Listen);
            _listenThread.Start();

            CreateLists();

            ServerCore.Logger.Log("Network", "Server started on port " + Port, LogType.Info);
        }

        /// <summary>
        /// Stops listening for connections, kicks all clients, and resets in preperation for reuse.
        /// </summary>
        public void Stop() {
            if (ServerCore.Running == false)
                return;

            CoreListener.Stop();
            ServerCore.Running = false;
            _listenThread.Abort();

            var myCopy = ClientList;

            foreach (var networkClient in myCopy) {
                networkClient.KickNow("Server closing.");
            }
        }

        /// <summary>
        /// Handles a client that is disconnecting
        /// </summary>
        /// <param name="client">The client that has disconnected.</param>
        public void HandleDisconnect(NetworkClient client) {
            
            lock (ClientLock) {
                Clients.Remove(client);
            }

            try {
                client.BaseSocket.Close();
            } catch (IOException) {
            }

            if (!client.CS.LoggedIn) 
                return;

            client.CS.LoggedIn = false;

            lock (client.CS.CurrentMap.ClientLock) {
                client.CS.CurrentMap.Clients.Remove(client.CS.Id);
                client.CS.CurrentMap.CreateClientList();
            }

            if (client.CS.MyEntity != null) {
                client.CS.CurrentMap.DeleteEntity(ref client.CS.MyEntity);
                ServerCore.FreeEids.Push((short)client.CS.MyEntity.Id);
            }

            ServerCore.OnlinePlayers -= 1;
            ServerCore.FreeIds.Push(client.CS.NameId);
            LoggedClients.Remove(client.CS.LoginName);
            IntLoggedClients.Remove(client.CS.Id);
            CreateLists();

            var remove = new ExtRemovePlayerName {NameId = client.CS.NameId};
                
            foreach (var c in ClientList) {
                if (c.CS.CPEExtensions.ContainsKey("ExtPlayerList"))
                    c.SendQueue.Enqueue(remove);
            }

            ServerCore.Logger.Log("Network", "Player " + client.CS.LoginName + " has disconnected.", LogType.Info); // -- Notify of their disconnection.
            ServerCore.Luahandler.RunFunction("E_PlayerDisconnect", client.CS.LoginName);
            Chat.SendGlobalChat(ServerCore.TextFormats.SystemMessage + "Player " + client.CS.FormattedName + ServerCore.TextFormats.SystemMessage + " left.");
        }

        /// <summary>
        /// For kicking clients in the pre-connection stage with a message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="client"></param>
        public void RawKick(string message, TcpClient client) {
            var tempWrapped = new ClassicWrapped.ClassicWrapped {Stream = client.GetStream()};

            tempWrapped.WriteByte(14);
            tempWrapped.WriteString(message);
            tempWrapped.Purge();
            Thread.Sleep(100); // -- Small delay to ensure message delivery
        }

        /// <summary>
        /// Loop to be run in a thread to listen for new clients, and do initial setup for them.
        /// </summary>
        public void Listen() {
            while (ServerCore.Running) {
                TcpClient tempClient;

                try {
                    tempClient = CoreListener.AcceptTcpClient(); // -- This will block until someone tries to connect.
                } catch {
                    continue; // -- Catches in the event of a server shutdown.
                }

                var ip = tempClient.Client.RemoteEndPoint.ToString().Substring(0, tempClient.Client.RemoteEndPoint.ToString().IndexOf(":")); // -- Strips the port the user is connecting from.

                if (ServerCore.DB.IsIpBanned(ip)) {
                    ServerCore.Logger.Log("Network", "Disconnecting client " + ip + ": IP banned.", LogType.Info);
                    RawKick("IP Banned", tempClient);
                    tempClient.Close();
                    continue;
                }

                if (ip != "127.0.0.1" && Clients.Count(p => p.CS.Ip.Equals(ip)) > MaxPerIp) {
                    ServerCore.Logger.Log("Network", "Disconnecting client " + ip + ": Connection limit reached.", LogType.Info);
                    RawKick("Connection limit reached for this IP!", tempClient);
                    tempClient.Close();
                    continue;
                }

                if (ServerCore.OnlinePlayers >= MaxPlayers) {
                    ServerCore.Logger.Log("Network", "Disconnecting client " + ip + ": Server is full.", LogType.Info);
                    RawKick("Server is full.", tempClient);
                    tempClient.Close();
                    continue;
                }

                var newClient = new NetworkClient(tempClient, ip); // -- Creates a new network client, which will begin waiting for and parsing packets.
                
                lock (ClientLock) {
                    Clients.Add(newClient);
                }

                ServerCore.Logger.Log("Network", "Client created (IP = " + newClient.CS.Ip + ")", LogType.Info);
            }
        }
    }
}
