using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Hypercube.Libraries {
    public class WatchdogModule {
        public string Message;
        public int CallsSecond;
    }
    public static class Watchdog {
        private const string htmlHeaders = @"<html>
    <head>
        <title>Hypercube Watchdog</title>
        <style type=""text/css"">
            body {
                font-family: ""Microsoft PhagsPa"";
                color:#2f2f2f;
                background-color:#F7F7F7;
            }
            h1.header {
                background-color:darkblue;
                text-shadow:2px 1px 0px rgba(0,0,0,.2); 
                font-size:25px;
                font-weight:bold;
                text-decoration:none;
                text-align:center;
                color:white;
                margin:0;
                height:42px; 
                width:auto;
                border-bottom: 1px black solid;
                height: 42px;
                margin: -8px;
                line-height: 42px;
            }
            table {
                border: 1px solid #A0A0A0;
                table-layout: auto;
                empty-cells: hide;
                border-collapse: collapse;
            }
            tr {
                border: 1px solid #A0A0A0;
                background-color: #D0D0D0;
                color: #212121;
                opacity:1.0;
            }
            td {
                border-right: 1px solid #A0A0A0;
            }
            th {
                border-right: 1px solid #A0A0A0;
            }
        </style>
    </head>
    
    <body>
        <h1 class=""header"">Hypercube Watchdog (Server stats)</h1>";

        public static void GenHtml() {
            
            string page = htmlHeaders;
            page += "\t\t<p>Generated at " + DateTime.UtcNow.ToLongTimeString() + " (All times are UTC +0)</p>\n";

            // -- Scheduled Tasks
            page += "\t\t<h3>Tasks:</h3>\n";
            page += "\t\t<table>\n";
            page += "<th>Task Name</th>\n\t\t\t<th>Run Interval</th>\n\t\t\t<th>Last Run</th>\n";

            lock (TaskScheduler.TaskLock) {
                foreach (var task in TaskScheduler.ScheduledTasks) {
                    page += "\t\t\t<tr>\n";
                    page += "\t\t\t\t<td>" + task.Key + "</td>\n";
                    page += "\t\t\t\t<td>" + task.Value.RunInterval + "</td>\n";
                    page += "\t\t\t\t<td>" + task.Value.LastRun.ToLongTimeString() + "</td>\n";
                    page += "\t\t\t</tr>\n";
                }
            }

            page += "\t\t</table>\n";

            // -- Maps
            page += "\t\t<h3>Maps:</h3>\n";
            page += "\t\t<table>\n";
            page += "<th>Map Name</th>\n\t\t\t<th>Filename</th>\n\t\t\t<th>Size</th>\n";
            page += "\t\t\t<th>Loaded</th>\n\t\t\t<th>Blockchanging</th>\n\t\t\t<th>Physics</th>";
            page +=
                "\n\t\t\t<th>History</th>\n\t\t\t<th>Physics Queue</th>\n\t\t\t<th>Send Queue</th>\n\t\t\t<th>Clients</th>\n";

            foreach (var map in ServerCore.Maps) {
                page += "\t\t\t<tr>\n";
                page += "\t\t\t\t<td>" + map.CWMap.MapName + "</td>\n";
                page += "\t\t\t\t<td>" + map.Path + "</td>\n";
                page += "\t\t\t\t<td>" + map.CWMap.SizeX + "x" + map.CWMap.SizeZ + "x" + map.CWMap.SizeY + "</td>\n";
                page += "\t\t\t\t<td>" + map.Loaded + "</td>\n";
                page += "\t\t\t\t<td>" + map.HCSettings.Building + "</td>\n";
                page += "\t\t\t\t<td>" + map.HCSettings.Physics + "</td>\n";
                page += "\t\t\t\t<td>" + map.HCSettings.History + "</td>\n";
                page += "\t\t\t\t<td>" + map.PhysicsQueue.Count + "</td>\n";
                page += "\t\t\t\t<td>" + map.BlockchangeQueue.Count + "</td>\n";
                page += "\t\t\t\t<td>" + map.Clients.Count + "</td>\n";
                page += "\t\t\t</tr>\n";
            }

            page += "\t\t</table>\n";

            // -- Network info
            page += "\t\t<h3>Network:</h3>\n";
            page += "\t\tPort: " + ServerCore.Nh.Port + "<br>\n";
            page += "\t\tPublic: " + ServerCore.Nh.Public + "<br>\n";
            page += "\t\tVerify Names?: " + ServerCore.Nh.VerifyNames + "<br>\n";
            page += "\t\tServer URL: " + ServerCore.Hb.ServerURL + "<br>\n";
            page += "\t\tClients (Logged): " + ServerCore.Nh.LoggedClients.Count + "<br>\n\n";

            page += "\t\t<h4>Clients:</h4>\n";
            page += "\t\t<table>\n";
            page += "\t\t\t<th>ID</th>\n\t\t\t<th>Login Name</th>\n\t\t\t<th>IP</th>\n\t\t\t<th>NameID</th>";
            page += "\n\t\t\t<th>Supports CPE</th>\n\t\t\t<th>Appname</th>\n\t\t\t<th>Extensions</th>\n\t\t\t<th>Map</th>";
            page += "\n\t\t\t<th>Entity ID</th>\n\t\t\t<th>Send Queue</th>\n";

            foreach (var client in ServerCore.Nh.ClientList) {
                page += "\t\t\t<tr>\n";
                page += "\t\t\t\t<td>" + client.CS.Id + "</td>\n";
                page += "\t\t\t\t<td>" + client.CS.LoginName + "</td>\n";
                page += "\t\t\t\t<td>" + client.CS.Ip + "</td>\n";
                page += "\t\t\t\t<td>" + client.CS.NameId + "</td>\n";
                page += "\t\t\t\t<td>" + client.CS.CPE + "</td>\n";
                page += "\t\t\t\t<td>" + client.CS.Appname + "</td>\n";
                page += "\t\t\t\t<td>" + client.CS.Extensions + "</td>\n";
                page += "\t\t\t\t<td>" + client.CS.CurrentMap.CWMap.MapName + "</td>\n";
                page += "\t\t\t\t<td>" + client.CS.MyEntity.Id + "(" + client.CS.MyEntity.ClientId + ")" + "</td>\n";
                page += "\t\t\t\t<td>" + client.SendQueue.Count + "</td>\n";
                page += "\t\t\t</tr>\n";
            }

            page += "\t\t</table>\n";
            page += "\t</body>\n";
            page += "</html>";

            if (!Directory.Exists("HTML"))
                Directory.CreateDirectory("HTML");

            File.WriteAllText("HTML/Watchdog.html", page);
        }
    }
}
