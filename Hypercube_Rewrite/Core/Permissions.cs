using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Hypercube.Libraries;

namespace Hypercube.Core {
    public class PermissionContainer {
        public SortedDictionary<string, Permission> Permissions = new SortedDictionary<string, Permission>(StringComparer.InvariantCultureIgnoreCase);
        public ISettings PermFile;
        Hypercube ServerCore;
        
        public PermissionContainer(Hypercube Core) {
            ServerCore = Core;
            PermFile = ServerCore.Settings.RegisterFile("Permissions.txt", false, new PBSettingsLoader.LoadSettings(LoadPermissions));
            ServerCore.Settings.ReadSettings(PermFile);
        }

        public void LoadPermissions() {
            Permissions.Clear();

            using (var SR = new StreamReader("Settings/Permissions.txt")) {
                while (!SR.EndOfStream) {
                    string Line = SR.ReadLine();

                    if (Line.StartsWith(";"))
                        continue;

                    if (!Line.Contains("."))
                        continue;

                    var newPerm = new Permission();
                    newPerm.Fullname = Line.ToLower().Replace(" ", "");
                    newPerm.Group = Line.Substring(0, Line.IndexOf("."));
                    newPerm.Perm = Line.Substring(Line.IndexOf(".") + 1, Line.Length - (Line.IndexOf(".") + 1));
                    Permissions.Add(newPerm.Fullname, newPerm);
                }
            }

            if (Permissions.Count == 0)
                CreatePermissions();

            ServerCore.Logger.Log("PermContainer", "Permissions loaded", LogType.Info);
        }

        public void SavePermissions() {
            using (var SW = new StreamWriter("Settings/Permissions.txt")) {
                foreach (Permission p in Permissions.Values)
                    SW.WriteLine(p.Fullname);
            }
        }

        public void CreatePermissions() {
            using (var SW = new StreamWriter("Settings/Permissions.txt")) {
                SW.WriteLine("map.addmap");
                SW.WriteLine("map.fillmap");
                SW.WriteLine("map.joinhiddenmap");
                SW.WriteLine("command.tp");
                SW.WriteLine("command.bring");
                SW.WriteLine("player.chat");
                SW.WriteLine("player.build");
                SW.WriteLine("player.delete");
                SW.WriteLine("player.op");
                SW.WriteLine("chat.readstaffchat");
                SW.WriteLine("chat.useemotes");
            }
        }

        public Permission GetPermission(string name) {
            if (Permissions.ContainsKey(name))
                return Permissions[name];
            else
                return null;
        }

        public static SortedDictionary<string, Permission> SplitPermissions(Hypercube Core, string perms) {
            var Result = new SortedDictionary<string, Permission>(StringComparer.InvariantCultureIgnoreCase);

            string[] mySplit = perms.Split(',');

            foreach (string s in mySplit) {
                var perm = Core.Permholder.GetPermission(s);

                if (perm != null)
                    Result.Add(perm.Fullname, perm);
            }

            return Result;
        }

        public static string PermissionsToString(SortedDictionary<string, Permission> perms) {
            string cds = "";

            foreach (Permission p in perms.Values) 
                cds += p.Fullname + ",";

            cds = cds.Substring(0, cds.Length - 1);
            return cds;
        }

        public static bool RankMatchesPermissions(Rank rank, List<Permission> permissions, bool MatchAll) {
            if (MatchAll && permissions.Count > rank.Permissions.Count)
                return false;

            if (MatchAll) {
                bool broke = false;

                foreach (Permission p in permissions) {
                    if (!rank.Permissions.ContainsKey(p.Fullname)) {
                        broke = true;
                        break;
                    }
                }

                if (broke)
                    return false;
                else
                    return true;
            }

            foreach (Permission p in permissions) {
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
