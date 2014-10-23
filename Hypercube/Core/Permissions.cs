using System;
using System.Collections.Generic;
using System.IO;

using Hypercube.Libraries;

namespace Hypercube.Core {
    public class PermissionContainer {
        public SortedDictionary<string, Permission> Permissions = new SortedDictionary<string, Permission>(StringComparer.InvariantCultureIgnoreCase);
        public Settings PermFile;
        
        public PermissionContainer() {
            PermFile = ServerCore.Settings.RegisterFile("Permissions.txt", false, LoadPermissions);
            ServerCore.Settings.ReadSettings(PermFile);
        }

        public void LoadPermissions() {
            Permissions.Clear();

            using (var sr = new StreamReader("Settings/Permissions.txt")) {
                while (!sr.EndOfStream) {
                    var line = sr.ReadLine();

                    if (line != null && line.StartsWith(";"))
                        continue;

                    if (line != null && !line.Contains("."))
                        continue;

                    if (line != null) {
                        var newPerm = new Permission
                        {
                            Fullname = line.ToLower().Replace(" ", ""),
                            Group = line.Substring(0, line.IndexOf(".")),
                            Perm = line.Substring(line.IndexOf(".") + 1, line.Length - (line.IndexOf(".") + 1))
                        };

                        Permissions.Add(newPerm.Fullname, newPerm);
                    }
                }
            }

            if (Permissions.Count == 0) {
                CreatePermissions();
                LoadPermissions();
            }

            ServerCore.Logger.Log("PermContainer", "Permissions loaded", LogType.Info);
        }

        public void SavePermissions() {
            using (var sw = new StreamWriter("Settings/Permissions.txt")) {
                foreach (var p in Permissions.Values)
                    sw.WriteLine(p.Fullname);
            }
        }

        public void CreatePermissions() {
            using (var sw = new StreamWriter("Settings/Permissions.txt")) {
                sw.WriteLine("map.addmap");
                sw.WriteLine("map.joinmap");
                sw.WriteLine("map.fillmap");
                sw.WriteLine("map.joinhiddenmap");
                sw.WriteLine("command.tp");
                sw.WriteLine("command.bring");
                sw.WriteLine("player.chat");
                sw.WriteLine("player.build");
                sw.WriteLine("player.delete");
                sw.WriteLine("player.op");
                sw.WriteLine("player.ban");
                sw.WriteLine("player.kick");
                sw.WriteLine("player.mute");
                sw.WriteLine("player.stop");
                sw.WriteLine("player.seehidden");
                sw.WriteLine("player.hide");
                sw.WriteLine("chat.readstaffchat");
                sw.WriteLine("chat.useemotes");
                sw.WriteLine("block.disabled");
            }

            ServerCore.Logger.Log("PermContainer", "Permissions created", LogType.Info);
        }

        public Permission GetPermission(string name) {
            Permission perm;
            Permissions.TryGetValue(name, out perm);
            return perm;
        }

        public static SortedDictionary<string, Permission> SplitPermissions(string perms) {
            var result = new SortedDictionary<string, Permission>(StringComparer.InvariantCultureIgnoreCase);

            var mySplit = perms.Split(',');

            foreach (var s in mySplit) {
                var perm = ServerCore.Permholder.GetPermission(s);

                if (perm != null)
                    result.Add(perm.Fullname, perm);
            }

            return result;
        }

        public static string PermissionsToString(SortedDictionary<string, Permission> perms) {
            var cds = "";

            foreach (var p in perms.Values) 
                cds += p.Fullname + ",";

            if (cds.EndsWith(","))
                cds = cds.Substring(0, cds.Length - 1);

            return cds;
        }
    }

    public class Permission {
        public string Fullname, Group, Perm;
    }
}
