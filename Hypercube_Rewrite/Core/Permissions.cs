using System;
using System.Collections.Generic;
using System.IO;

using Hypercube.Libraries;

namespace Hypercube.Core {
    public class PermissionContainer {
        public SortedDictionary<string, Permission> Permissions = new SortedDictionary<string, Permission>(StringComparer.InvariantCultureIgnoreCase);
        public Settings PermFile;
        
        public PermissionContainer() {
            PermFile = Hypercube.Settings.RegisterFile("Permissions.txt", false, LoadPermissions);
            Hypercube.Settings.ReadSettings(PermFile);
        }

        public void LoadPermissions() {
            Permissions.Clear();

            using (var sr = new StreamReader("SettingsDictionary/Permissions.txt")) {
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

            Hypercube.Logger.Log("PermContainer", "Permissions loaded", LogType.Info);
        }

        public void SavePermissions() {
            using (var sw = new StreamWriter("SettingsDictionary/Permissions.txt")) {
                foreach (var p in Permissions.Values)
                    sw.WriteLine(p.Fullname);
            }
        }

        public void CreatePermissions() {
            using (var sw = new StreamWriter("SettingsDictionary/Permissions.txt")) {
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
                sw.WriteLine("chat.readstaffchat");
                sw.WriteLine("chat.useemotes");
            }

            Hypercube.Logger.Log("PermContainer", "Permissions created", LogType.Info);
        }

        public Permission GetPermission(string name) {
            if (Permissions.ContainsKey(name))
                return Permissions[name];
            
            return null;
        }

        public static SortedDictionary<string, Permission> SplitPermissions(string perms) {
            var result = new SortedDictionary<string, Permission>(StringComparer.InvariantCultureIgnoreCase);

            var mySplit = perms.Split(',');

            foreach (var s in mySplit) {
                var perm = Hypercube.Permholder.GetPermission(s);

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

        public static bool RankMatchesPermissions(Rank rank, List<Permission> permissions, bool matchAll) {
            if (matchAll && permissions.Count > rank.Permissions.Count)
                return false;

            if (matchAll) {
                var broke = false;

                foreach (var p in permissions) {
                    if (!rank.Permissions.ContainsKey(p.Fullname)) {
                        broke = true;
                        break;
                    }
                }

                if (broke)
                    return false;
                
                return true;
            }

            foreach (var p in permissions) {
                if (rank.Permissions.ContainsKey(p.Fullname))
                    return true;
            }

            return false;
        }
    }

    public class Permission {
        public string Fullname, Group, Perm;
    }
}
