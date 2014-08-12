using System;
using System.Collections.Generic;
using System.Linq;
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
        public Settings Ns;
        public List<NetworkClient> Clients = new List<NetworkClient>();
        public Dictionary<string, NetworkClient> LoggedClients = new Dictionary<string, NetworkClient>(StringComparer.InvariantCultureIgnoreCase);

        public NetworkClient[] ClientList;

        public TcpListener CoreListener;
        public object ClientLock = new object();

        // -- Network SettingsDictionary
        public int Port, MaxPlayers, MaxPerIp;
        public bool VerifyNames, Public;

        Thread _listenThread;
        #endregion

        public NetworkHandler() {
            Ns = Hypercube.Settings.RegisterFile("Network.txt", true, LoadSettings);
            Hypercube.Settings.ReadSettings(Ns);
        }

        public void CreateShit() {
            ClientList = LoggedClients.Values.ToArray();
        }

        void LoadSettings() {
            Port = int.Parse(Hypercube.Settings.ReadSetting(Ns, "Port", "25565"));
            MaxPlayers = int.Parse(Hypercube.Settings.ReadSetting(Ns, "MaxPlayers", "128"));
            VerifyNames = bool.Parse(Hypercube.Settings.ReadSetting(Ns, "VerifyNames", "true"));
            Public = bool.Parse(Hypercube.Settings.ReadSetting(Ns, "Public", "true"));
            MaxPerIp = int.Parse(Hypercube.Settings.ReadSetting(Ns, "MaxPerIP", "5"));

            Hypercube.Logger.Log("Network", "Network settings loaded.", LogType.Info);
        }

        public void SaveSettings() {
            Hypercube.Settings.SaveSetting(Ns, "Port", Port.ToString());
            Hypercube.Settings.SaveSetting(Ns, "MaxPlayers", MaxPlayers.ToString());
            Hypercube.Settings.SaveSetting(Ns, "VerifyNames", VerifyNames.ToString());
            Hypercube.Settings.SaveSetting(Ns, "Public", Public.ToString());
        }

        public void Start() {
            if (Hypercube.Running)
                Stop();

            CoreListener = new TcpListener(IPAddress.Any, Port);
            Hypercube.Running = true;
            CoreListener.Start();

            _listenThread = new Thread(Listen);
            _listenThread.Start();

            CreateShit();

            Hypercube.Logger.Log("Network", "Server started on port " + Port, LogType.Info);
        }

        public void Stop() {
            if (Hypercube.Running == false)
                return;

            CoreListener.Stop();
            Hypercube.Running = false;
            _listenThread.Abort();

            lock (ClientLock) {
                foreach (NetworkClient client in Clients)
                    client.KickPlayer("Server closing.");
            }
        }

        public void HandleDisconnect(NetworkClient client) {
            lock (ClientLock) {
                Clients.Remove(client);
            }

            if (client.CS.LoggedIn) {
                lock (client.CS.CurrentMap.ClientLock) {
                    client.CS.CurrentMap.Clients.Remove(client.CS.Id);
                    client.CS.CurrentMap.CreateClientList();
                }

                LoggedClients.Remove(client.CS.LoginName);

                client.CS.CurrentMap.DeleteEntity(ref client.CS.MyEntity);

                Hypercube.OnlinePlayers -= 1;
                Hypercube.FreeId = client.CS.NameId;
                Hypercube.EFree = (short)client.CS.MyEntity.Id;

                var remove = new ExtRemovePlayerName {NameId = client.CS.NameId};

                lock (ClientLock) {
                    foreach (var c in Clients) {
                        if (c.CS.CPEExtensions.ContainsKey("ExtPlayerList"))
                            c.SendQueue.Enqueue(remove);
                            //Remove.Write(c);
                    }
                }

                Hypercube.Logger.Log("Network", "Player " + client.CS.LoginName + " has disconnected.", LogType.Info); // -- Notify of their disconnection.
                Hypercube.Luahandler.RunFunction("E_PlayerDisconnect", client.CS.LoginName);
                Chat.SendGlobalChat(Hypercube.TextFormats.SystemMessage + "Player " + client.CS.FormattedName + Hypercube.TextFormats.SystemMessage + " left.");
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
        /// <param name="message"></param>
        /// <param name="client"></param>
        public void RawKick(string message, TcpClient client) {
            var tempWrapped = new ClassicWrapped.ClassicWrapped();
            tempWrapped.Stream = client.GetStream();

            tempWrapped.WriteByte(14);
            tempWrapped.WriteString(message);
            tempWrapped.Purge();
            Thread.Sleep(100); // -- Small delay to ensure message delivery
        }

        public void Listen() {
            while (Hypercube.Running) {
                TcpClient tempClient;

                try {
                    tempClient = CoreListener.AcceptTcpClient(); // -- This will block until someone tries to connect.
                } catch {
                    continue; // -- Catches in the event of a server shutdown.
                }

                var ip = tempClient.Client.RemoteEndPoint.ToString().Substring(0, tempClient.Client.RemoteEndPoint.ToString().IndexOf(":")); // -- Strips the port the user is connecting from.

                if (Hypercube.DB.IsIpBanned(ip)) {
                    Hypercube.Logger.Log("Network", "Disconnecting client " + ip + ": IP banned.", LogType.Info);
                    RawKick("IP Banned", tempClient);
                    tempClient.Close();
                    continue;
                }

                if (ip != "127.0.0.1" && Clients.Count(p => p.CS.Ip.Equals(ip)) > MaxPerIp) {
                    Hypercube.Logger.Log("Network", "Disconnecting client " + ip + ": Connection limit reached.", LogType.Info);
                    RawKick("Connection limit reached for this IP!", tempClient);
                    tempClient.Close();
                    continue;
                }

                if (Hypercube.OnlinePlayers >= MaxPlayers) {
                    Hypercube.Logger.Log("Network", "Disconnecting client " + ip + ": Server is full.", LogType.Info);
                    RawKick("Server is full.", tempClient);
                    tempClient.Close();
                    continue;
                }

                var newClient = new NetworkClient(tempClient, ip); // -- Creates a new network client, which will begin waiting for and parsing packets.
                
                lock (ClientLock) {
                    Clients.Add(newClient);
                }

                Hypercube.Logger.Log("Network", "Client created (IP = " + newClient.CS.Ip + ")", LogType.Info);
            }
        }
    }
}
