﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Web.Services;
using System.IO;
using System.Threading;
using System.Security.Cryptography;

using Hypercube.Client;
using Hypercube.Core;

namespace Hypercube.Network {
    public class Heartbeat {
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

            for (var i = 1; i < 33; i++)
                Salt += (char)(65 + Random.Next(25));
        }

        public string GetIPv4Address(string site) {
            var Addresses = Dns.GetHostAddresses(site);
            var v4 = Addresses.First(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            return v4.ToString();
        }

        /// <summary>
        /// Sends a heartbeat to Classicube.net for this server.
        /// </summary>
        public void DoHeartbeatClassicube() {
            var Request = new WebClient();
            try {
                Request.Proxy = new WebProxy("http://" + GetIPv4Address("classicube.net") + ":80/"); // -- Makes sure we're using an IPv4 Address and not IPv6.
            } catch {
                ServerCore.Logger.Log("Heartbeat", "Failed to send heartbeat.", LogType.Error);
                return;
            }
            while (ServerCore.Running) {
                try {
                    var Response = Request.DownloadString("http://www.classicube.net/heartbeat.jsp?port=" + ServerCore.nh.Port.ToString() + "&users=" + ServerCore.OnlinePlayers.ToString() + "&max=" + ServerCore.nh.MaxPlayers.ToString() + "&name=" + HttpUtility.UrlEncode(ServerCore.ServerName) + "&public=" + ServerCore.nh.Public.ToString() + "&software=Hypercube&salt=" + HttpUtility.UrlEncode(Salt));
                    ServerCore.Logger.Log("Heartbeat", "Heartbeat sent.", LogType.Info);
                    ServerCore.Luahandler.RunFunction("E_Heartbeat");
                    File.WriteAllText("ServerURL.txt", Response);
                } catch (Exception e) {
                    ServerCore.Logger.Log("Heartbeat", "Failed to send heartbeat.", LogType.Error);
                    ServerCore.Logger.Log("Classicube", e.Message, LogType.Error);
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
            if (Client.CS.Ip == "127.0.0.1" || Client.CS.Ip.Substring(0, 7) == "192.168" || ServerCore.nh.VerifyNames == false)
                return true;

            var MD5Creator = MD5.Create();
            var Correct = BitConverter.ToString(MD5Creator.ComputeHash(Encoding.ASCII.GetBytes(Salt + Client.CS.LoginName))).Replace("-", "");

            if (Correct.Trim().ToLower() == Client.CS.MpPass.Trim().ToLower()) 
                return true;
            else {
                ServerCore.Logger.Log("Heartbeat", Correct.Trim() + " != " + Client.CS.MpPass.Trim(), LogType.Warning);
                return false;
            }
        }
    }
}
