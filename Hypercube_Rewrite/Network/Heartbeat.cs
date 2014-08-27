using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using System.Security.Cryptography;

using Hypercube.Client;
using Hypercube.Core;
using Hypercube.Libraries;

namespace Hypercube.Network {
    public class Heartbeat {
        public string Salt;
        public string ServerURL;
        /// <summary>
        /// Generates a new salt and starts heartbeating.
        /// </summary>
        public Heartbeat() {
            CreateSalt();

            TaskScheduler.CreateTask("Heartbeat", new TimeSpan(0, 0, 45), DoHeartbeatClassicube);
        }

        /// <summary>
        /// Aborts the heartbeat thread.
        /// </summary>
        public void Shutdown() {
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
                ServerCore.Logger.Log("Heartbeat", "Failed to send heartbeat.", LogType.Error);
                return;
            }

            try {
                var response = request.DownloadString("http://www.classicube.net/heartbeat.jsp?port=" + ServerCore.Nh.Port + "&users=" + ServerCore.OnlinePlayers + "&max=" + ServerCore.Nh.MaxPlayers + "&name=" + HttpUtility.UrlEncode(ServerCore.ServerName) + "&public=" + ServerCore.Nh.Public + "&software=ServerCore&salt=" + HttpUtility.UrlEncode(Salt));
                ServerCore.Logger.Log("Heartbeat", "Heartbeat sent.", LogType.Info);
                ServerCore.Luahandler.RunFunction("E_Heartbeat");
                ServerURL = response;
                File.WriteAllText("ServerURL.txt", response);
            } catch (Exception e) {
                ServerCore.Logger.Log("Heartbeat", "Failed to send heartbeat.", LogType.Error);
                ServerCore.Logger.Log("Classicube", e.Message, LogType.Error);
            }
        }

        /// <summary>
        /// Verifys the authenticity of this user.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool VerifyClientName(NetworkClient client) {
            if (client.CS.Ip == "127.0.0.1" || client.CS.Ip.Substring(0, 7) == "192.168" || ServerCore.Nh.VerifyNames == false)
                return true;

            var md5Creator = MD5.Create();
            var correct = BitConverter.ToString(md5Creator.ComputeHash(Encoding.ASCII.GetBytes(Salt + client.CS.LoginName))).Replace("-", "");

            if (correct.Trim().ToLower() == client.CS.MpPass.Trim().ToLower()) 
                return true;

            ServerCore.Logger.Log("Heartbeat", correct.Trim() + " != " + client.CS.MpPass.Trim(), LogType.Warning);
            return false;
        }
    }
}
