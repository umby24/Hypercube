using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using System.Threading;
using System.Security.Cryptography;

using Hypercube.Client;
using Hypercube.Core;

namespace Hypercube.Network {
    public class Heartbeat {
        public string Salt;
        readonly Thread _heartbeatThread;

        /// <summary>
        /// Generates a new salt and starts heartbeating.
        /// </summary>
        public Heartbeat() {
            CreateSalt();

            _heartbeatThread = new Thread(DoHeartbeatClassicube);
            _heartbeatThread.Start();
        }

        /// <summary>
        /// Aborts the heartbeat thread.
        /// </summary>
        public void Shutdown() {
            if (_heartbeatThread != null)
                _heartbeatThread.Abort();
        }

        /// <summary>
        /// Creates a random 32-character salt for verification.
        /// </summary>
        public void CreateSalt() {
            Salt = "";
            var random = new Random();

            for (var i = 1; i < 33; i++)
                Salt += (char)(65 + random.Next(25));
        }

        public string GetIPv4Address(string site) {
            var addresses = Dns.GetHostAddresses(site);
            var v4 = addresses.First(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            return v4.ToString();
        }

        /// <summary>
        /// Sends a heartbeat to Classicube.net for this server.
        /// </summary>
        public void DoHeartbeatClassicube() {
            var request = new WebClient();

            try {
                request.Proxy = new WebProxy("http://" + GetIPv4Address("classicube.net") + ":80/"); // -- Makes sure we're using an IPv4 Address and not IPv6.
            } catch {
                Hypercube.Logger.Log("Heartbeat", "Failed to send heartbeat.", LogType.Error);
                return;
            }

            while (Hypercube.Running) {
                try {
                    var response = request.DownloadString("http://www.classicube.net/heartbeat.jsp?port=" + Hypercube.Nh.Port + "&users=" + Hypercube.OnlinePlayers + "&max=" + Hypercube.Nh.MaxPlayers + "&name=" + HttpUtility.UrlEncode(Hypercube.ServerName) + "&public=" + Hypercube.Nh.Public + "&software=Hypercube&salt=" + HttpUtility.UrlEncode(Salt));
                    Hypercube.Logger.Log("Heartbeat", "Heartbeat sent.", LogType.Info);
                    Hypercube.Luahandler.RunFunction("E_Heartbeat");
                    File.WriteAllText("ServerURL.txt", response);
                } catch (Exception e) {
                    Hypercube.Logger.Log("Heartbeat", "Failed to send heartbeat.", LogType.Error);
                    Hypercube.Logger.Log("Classicube", e.Message, LogType.Error);
                }

                Thread.Sleep(45000);
            }
        }

        /// <summary>
        /// Verifys the authenticity of this user.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool VerifyClientName(NetworkClient client) {
            if (client.CS.Ip == "127.0.0.1" || client.CS.Ip.Substring(0, 7) == "192.168" || Hypercube.Nh.VerifyNames == false)
                return true;

            var md5Creator = MD5.Create();
            var correct = BitConverter.ToString(md5Creator.ComputeHash(Encoding.ASCII.GetBytes(Salt + client.CS.LoginName))).Replace("-", "");

            if (correct.Trim().ToLower() == client.CS.MpPass.Trim().ToLower()) 
                return true;

            Hypercube.Logger.Log("Heartbeat", correct.Trim() + " != " + client.CS.MpPass.Trim(), LogType.Warning);
            return false;
        }
    }
}
