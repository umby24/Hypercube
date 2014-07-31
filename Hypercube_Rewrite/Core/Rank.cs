using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hypercube.Libraries;

namespace Hypercube.Core
{
    public class RankContainer {
        public SortedDictionary<int, Rank> numberList;
        public SortedDictionary<string, Rank> nameList;

        public ISettings Ranksfile;
        Hypercube ServerCore;

        public RankContainer(Hypercube Core) {
            numberList = new SortedDictionary<int, Rank>();
            nameList = new SortedDictionary<string, Rank>(StringComparer.InvariantCultureIgnoreCase);

            Ranksfile = Core.Settings.RegisterFile("Ranks.txt", true, new PBSettingsLoader.LoadSettings(LoadRanks));
            ServerCore = Core;
            ServerCore.Settings.ReadSettings(Ranksfile);
        }

        /// <summary>
        /// Splits a comma delimited string of rank IDs into a list of ranks.
        /// </summary>
        /// <param name="RankString"></param>
        /// <returns></returns>
        public static List<Rank> SplitRanks(Hypercube ServerCore, string RankString) {
            var result = new List<Rank>();
            var splitRanks = RankString.Split(',');

            foreach (string s in splitRanks)
                result.Add(ServerCore.Rankholder.GetRank(int.Parse(s)));

            return result;
        }

        /// <summary>
        /// Splits a comma delimited string of rank steps into a list of int (steps).
        /// </summary>
        /// <param name="StepString">The comma delimited string containing the steps to split.</param>
        /// <returns></returns>
        public static List<int> SplitSteps(string StepString) {
            var result = new List<int>();
            var splitSteps = StepString.Split(',');

            foreach (string s in splitSteps)
                result.Add(int.Parse(s));

            return result;
        }

        /// <summary>
        /// Searches a list of ranks to see if it contains one of the ranks in another list.
        /// </summary>
        /// <param name="Search">The list to search. if a rank from Compare is found here, the function returns true.</param>
        /// <param name="Compare">The list to iterate through and compare against Search.</param>
        /// <returns></returns>
        public static bool RankListContains(List<Rank> Search, List<Rank> Compare) {
            foreach (Rank r in Compare) {
                if (Search.Contains(r))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a rank object for modification or reference.
        /// </summary>
        /// <returns> A rank object for modification or reference.</returns>
        public Rank GetRank(int ID) {
            if (numberList.ContainsKey(ID))
                return numberList[ID];
            else
                return null;
        }

        public Rank GetRank(string name) {
            if (nameList.ContainsKey(name))
                return nameList[name];
            else
                return null;
        }

        public void LoadRanks() {
            nameList.Clear();
            numberList.Clear();

            foreach (string ID in Ranksfile.Settings.Keys) {
                ServerCore.Settings.SelectGroup(Ranksfile, ID);

                var newRank = new Rank();
                newRank.ID = int.Parse(ID);
                newRank.Name = ServerCore.Settings.ReadSetting(Ranksfile, "Name", "DEFAULT");
                newRank.Prefix = ServerCore.Settings.ReadSetting(Ranksfile, "Prefix", "");
                newRank.Suffix = ServerCore.Settings.ReadSetting(Ranksfile, "Suffix", "");
                newRank.NextRank = ServerCore.Settings.ReadSetting(Ranksfile, "NextRank", "");
                newRank.Group = ServerCore.Settings.ReadSetting(Ranksfile, "Group", "Default");
                newRank.PointsInRank = ServerCore.Settings.ReadSetting(Ranksfile, "Points", 5);
                newRank.Op = bool.Parse(ServerCore.Settings.ReadSetting(Ranksfile, "IsOp", "false"));
                newRank.Permissions = PermissionContainer.SplitPermissions(ServerCore, ServerCore.Settings.ReadSetting(Ranksfile, "Perms", "player.chat,player.build,player.delete,chat.useemotes,command.tp"));
                nameList.Add(newRank.Name, newRank);
                numberList.Add(newRank.ID, newRank);
            }

            if (nameList.Count == 0) {
                var GuestRank = new Rank("Guest", "Default", "&f", "", false, 50, "Builder");
                GuestRank.ID = 0;
                GuestRank.Permissions = PermissionContainer.SplitPermissions(ServerCore, "player.chat,player.build,player.delete,chat.useemotes,command.tp");

                var BuilderRank = new Rank("Builder", "Default", "&e", "", false, 15, "");
                BuilderRank.ID = 1;
                BuilderRank.Permissions = PermissionContainer.SplitPermissions(ServerCore, "player.chat,player.build,player.delete,chat.useemotes,command.tp");

                var OpRank = new Rank("Op", "Staff", "&9", "", true, 100, "");
                OpRank.ID = 2;
                OpRank.Permissions = PermissionContainer.SplitPermissions(ServerCore, "player.chat,player.build,player.delete,chat.useemotes,command.tp,command.bring,map.joinhiddenmap,chat.readstaffchat");

                var OwnerRank = new Rank("Owner", "Staff", "&0", "", true, 0, "");
                OwnerRank.ID = 3;
                OwnerRank.Permissions = PermissionContainer.SplitPermissions(ServerCore, "player.chat,player.build,player.delete,chat.useemotes,command.tp,command.bring,map.joinhiddenmap,chat.readstaffchat,map.fillmap");

                numberList.Add(0, GuestRank);
                nameList.Add("Guest", GuestRank);

                numberList.Add(1, BuilderRank);
                nameList.Add("Builder", BuilderRank);

                numberList.Add(2, OpRank);
                nameList.Add("Op", OpRank);

                numberList.Add(3, OwnerRank);
                nameList.Add("Owner", OwnerRank);

                SaveRanks();
            }

            ServerCore.Logger.Log("RankContainer", "Ranks loaded.", LogType.Info);
        }

        /// <summary>
        /// Saves the internal ranks list to file. 
        /// </summary>
        public void SaveRanks() {
            Ranksfile.Settings.Clear();

            foreach (Rank r in numberList.Values) {
                ServerCore.Settings.SelectGroup(Ranksfile, r.ID.ToString());
                ServerCore.Settings.SaveSetting(Ranksfile, "Name", r.Name);
                ServerCore.Settings.SaveSetting(Ranksfile, "Prefix", r.Prefix);
                ServerCore.Settings.SaveSetting(Ranksfile, "Suffix", r.Suffix);
                ServerCore.Settings.SaveSetting(Ranksfile, "NextRank", r.NextRank);
                ServerCore.Settings.SaveSetting(Ranksfile, "Group", r.Group);
                ServerCore.Settings.SaveSetting(Ranksfile, "Points", r.PointsInRank.ToString());
                ServerCore.Settings.SaveSetting(Ranksfile, "IsOp", r.Op.ToString());
                ServerCore.Settings.SaveSetting(Ranksfile, "Perms", PermissionContainer.PermissionsToString(r.Permissions));
            }

            ServerCore.Settings.SaveSettings(Ranksfile);
        }
    }

    public class Rank {
        public string Name, Prefix, Suffix, NextRank, Group;
        public int ID, PointsInRank;
        public bool Op;
        public SortedDictionary<string, Permission> Permissions = new SortedDictionary<string, Permission>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Creates a blank rank object that you can assign values to.
        /// </summary>
        public Rank() {

        }

        /// <summary>
        /// Creates a filled rank object with all fields filled.
        /// </summary>
        /// <param name="RankName">Name of the rank.</param>
        /// <param name="RankPrefix">The Prefix any user with this rank will have on their name.</param>
        /// <param name="RankSuffix">The Suffix any user with this rank will have on their name.</param>
        /// <param name="IsOp">Indicates if this user is considered an Operator or higher.</param>
        /// <param name="RankPoints">Indicates the number of points contained in this rank. When this number has been exceeded, a user will be moved to NextRank.</param>
        /// <param name="NextRankUp">The Next rank the user will acheive if they have received all points possible for this rank. Blank indicates no higher rank.</param>
        public Rank(string RankName, string RankGroup, string RankPrefix, string RankSuffix, bool IsOp, int RankPoints, string NextRankUp) {
            Name = RankName;
            Prefix = RankPrefix;
            Suffix = RankSuffix;
            Op = IsOp;
            PointsInRank = RankPoints;
            NextRank = NextRankUp;
            Group = RankGroup;
        }
    }
}
