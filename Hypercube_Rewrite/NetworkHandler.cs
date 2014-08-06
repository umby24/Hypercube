using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Hypercube.Core;
using Hypercube.Libraries;
using Hypercube.Client;
using Hypercube.Network;

namespace Hypercube {
    public class NetworkHandler {
        #region Variables
        public ISettings NS;
        public List<NetworkClient> Clients = new List<NetworkClient>();
        public Dictionary<string, NetworkClient> LoggedClients = new Dictionary<string, NetworkClient>(StringComparer.InvariantCultureIgnoreCase);

        public NetworkClient[] ClientList;

        public TcpListener CoreListener;
        public object ClientLock = new object();

        // -- Network Settings
        public int Port, MaxPlayers, MaxPerIP;
        public bool VerifyNames, Public;

        Hypercube ServerCore;
        Thread ListenThread;
        #endregion

        public NetworkHandler(Hypercube Core) {
            ServerCore = Core;
            NS = ServerCore.Settings.RegisterFile("Network.txt", true, new PBSettingsLoader.LoadSettings(LoadSettings));
            ServerCore.Settings.ReadSettings(NS);
        }

        public void CreateShit() {
            ClientList = LoggedClients.Values.ToArray();
        }

        void LoadSettings() {
            Port = int.Parse(ServerCore.Settings.ReadSetting(NS, "Port", "25565"));
            MaxPlayers = int.Parse(ServerCore.Settings.ReadSetting(NS, "MaxPlayers", "128"));
            VerifyNames = bool.Parse(ServerCore.Settings.ReadSetting(NS, "VerifyNames", "true"));
            Public = bool.Parse(ServerCore.Settings.ReadSetting(NS, "Public", "true"));
            MaxPerIP = int.Parse(ServerCore.Settings.ReadSetting(NS, "MaxPerIP", "5"));

            ServerCore.Logger.Log("Network", "Network settings loaded.", LogType.Info);
        }

        public void SaveSettings() {
            ServerCore.Settings.SaveSetting(NS, "Port", Port.ToString());
            ServerCore.Settings.SaveSetting(NS, "MaxPlayers", MaxPlayers.ToString());
            ServerCore.Settings.SaveSetting(NS, "VerifyNames", VerifyNames.ToString());
            ServerCore.Settings.SaveSetting(NS, "Public", Public.ToString());
        }

        public void Start() {
            if (ServerCore.Running)
                Stop();

            CoreListener = new TcpListener(IPAddress.Any, Port);
            ServerCore.Running = true;
            CoreListener.Start();

            ListenThread = new Thread(Listen);
            ListenThread.Start();

            CreateShit();

            ServerCore.Logger.Log("Network", "Server started on port " + Port.ToString(), LogType.Info);
        }

        public void Stop() {
            if (ServerCore.Running == false)
                return;

            CoreListener.Stop();
            ServerCore.Running = false;
            ListenThread.Abort();

            lock (ClientLock) {
                for (int i = 0; i < Clients.Count; i++)
                    Clients[i].KickPlayer("Server closing.");
            }
        }

        public void HandleDisconnect(NetworkClient client) {
            lock (ClientLock) {
                Clients.Remove(client);
            }

            if (client.CS.LoggedIn) {
                lock (client.CS.CurrentMap.ClientLock) {
                    client.CS.CurrentMap.Clients.Remove(client.CS.ID);
                    client.CS.CurrentMap.CreateList();
                }

                LoggedClients.Remove(client.CS.LoginName);

                client.CS.CurrentMap.DeleteEntity(ref client.CS.MyEntity);

                ServerCore.OnlinePlayers -= 1;
                ServerCore.FreeID = client.CS.NameID;
                ServerCore.EFree = (short)client.CS.MyEntity.ID;

                var Remove = new ExtRemovePlayerName();
                Remove.NameID = client.CS.NameID;

                lock (ClientLock) {
                    foreach (NetworkClient c in Clients) {
                        if (c.CS.CPEExtensions.ContainsKey("ExtPlayerList"))
                            c.SendQueue.Enqueue(Remove);
                            //Remove.Write(c);
                    }
                }

                ServerCore.Logger.Log("Network", "Player " + client.CS.LoginName + " has disconnected.", LogType.Info); // -- Notify of their disconnection.
                ServerCore.Luahandler.RunFunction("E_PlayerDisconnect", client.CS.LoginName);
                Chat.SendGlobalChat(ServerCore, ServerCore.TextFormats.SystemMessage + "Player " + client.CS.FormattedName + ServerCore.TextFormats.SystemMessage + " left.");
                client.CS.LoggedIn = false;
                CreateShit();
            }

            try {
                client.BaseSocket.Close();
            } catch {

            }
        }

        /// <summary>
        /// For kicking clients in the pre-connection stage with a message.
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="client"></param>
        public void RawKick(string Message, TcpClient client) {
            var tempWrapped = new ClassicWrapped.ClassicWrapped();
            tempWrapped._Stream = client.GetStream();

            tempWrapped.WriteByte(14);
            tempWrapped.WriteString(Message);
            tempWrapped.Purge();

            tempWrapped = null;
            Thread.Sleep(100); // -- Small delay to ensure message delivery
        }

        public void Listen() {
            while (ServerCore.Running) {
                TcpClient tempClient;

                try {
                    tempClient = CoreListener.AcceptTcpClient(); // -- This will block until someone tries to connect.
                } catch {
                    continue; // -- Catches in the event of a server shutdown.
                }

                string IP = tempClient.Client.RemoteEndPoint.ToString().Substring(0, tempClient.Client.RemoteEndPoint.ToString().IndexOf(":")); // -- Strips the port the user is connecting from.

                if (ServerCore.DB.IsIPBanned(IP)) {
                    ServerCore.Logger.Log("Network", "Disconnecting client " + IP + ": IP banned.", LogType.Info);
                    RawKick("IP Banned", tempClient);
                    tempClient.Close();
                    continue;
                }

                if (IP != "127.0.0.1" && Clients.Count(p => p.CS.IP.Equals(IP)) > MaxPerIP) {
                    ServerCore.Logger.Log("Network", "Disconnecting client " + IP + ": Connection limit reached.", LogType.Info);
                    RawKick("Connection limit reached for this IP!", tempClient);
                    tempClient.Close();
                    continue;
                }

                if (ServerCore.OnlinePlayers >= MaxPlayers) {
                    ServerCore.Logger.Log("Network", "Disconnecting client " + IP + ": Server is full.", LogType.Info);
                    RawKick("Server is full.", tempClient);
                    tempClient.Close();
                    continue;
                }

                var NewClient = new NetworkClient(tempClient, ServerCore, IP); // -- Creates a new network client, which will begin waiting for and parsing packets.
                
                lock (ClientLock) {
                    Clients.Add(NewClient);
                }

                ServerCore.Logger.Log("Network", "Client created (IP = " + NewClient.CS.IP + ")", LogType.Info);
            }
        }
    }
}
