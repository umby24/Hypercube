using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using System.Threading;
using System.Security.Cryptography;

using Hypercube_Classic.Client;

namespace Hypercube_Classic.Core {
    class Heartbeat {
        public string Salt;
        Hypercube ServerCore;
        Thread HeartbeatThread;

        /// <summary>
        /// Generates a new salt and starts heartbeating.
        /// </summary>
        /// <param name="Core"></param>
        public Heartbeat(Hypercube Core) {
            ServerCore = Core;
            CreateSalt();

            HeartbeatThread = new Thread(DoHeartbeatClassicube);
            HeartbeatThread.Start();
        }

        /// <summary>
        /// Aborts the heartbeat thread.
        /// </summary>
        public void Shutdown() {
            if (HeartbeatThread != null)
                HeartbeatThread.Abort();
        }

        /// <summary>
        /// Creates a random 32-character salt for verification.
        /// </summary>
        public void CreateSalt() {
            Salt = "";
            var Random = new Random();

            for (int i = 1; i < 33; i++)
                Salt += (char)(65 + Random.Next(25));

        }

        /// <summary>
        /// Sends a heartbeat to Classicube.net for this server.
        /// </summary>
        public void DoHeartbeatClassicube() {
            var Request = new WebClient();

            while (ServerCore.Running) {
                try {
                    string Response = Request.DownloadString("http://www.classicube.net/heartbeat.jsp?port=" + ServerCore.nh.Port.ToString() + "&users=" + ServerCore.OnlinePlayers.ToString() + "&max=" + ServerCore.nh.MaxPlayers.ToString() + "&name=" + HttpUtility.UrlEncode(ServerCore.ServerName) + "&public=" + ServerCore.nh.Public.ToString() + "&salt=" + HttpUtility.UrlEncode(Salt));
                    ServerCore.Logger._Log("Info", "Heartbeat", "Heartbeat sent.");
                    File.WriteAllText("ServerURL.txt", Response);
                } catch {
                    ServerCore.Logger._Log("Error", "Heartbeat", "Failed to send heartbeat.");
                }

                Thread.Sleep(45000);
            }
        }

        /// <summary>
        /// Verifys the authenticity of this user.
        /// </summary>
        /// <param name="Client"></param>
        /// <returns></returns>
        public bool VerifyClientName(NetworkClient Client) {
            if (Client.CS.IP == "127.0.0.1" || Client.CS.IP.Substring(0, 7) == "192.168" || ServerCore.nh.VerifyNames == false) 
                return true;

            var MD5Creator = MD5.Create();
            string Correct = Encoding.ASCII.GetString(MD5Creator.ComputeHash(Encoding.ASCII.GetBytes(Salt + Client.CS.LoginName)));

            if (Correct.Trim().ToLower() == Client.CS.MPPass.Trim().ToLower())
                return true;
            else
                return false;
        }
    }
}
