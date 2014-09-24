using System;
using System.Collections.Generic;
using System.Linq;
using Hypercube.Libraries;

namespace Hypercube.Core
{
    public class RankContainer {
        public SortedDictionary<int, Rank> NumberList;
        public SortedDictionary<string, Rank> NameList;
        public Dictionary<Rank, List<Rank>> RankHierarchy; 
        public Settings Ranksfile;

        public RankContainer() {
            NumberList = new SortedDictionary<int, Rank>();
            NameList = new SortedDictionary<string, Rank>(StringComparer.InvariantCultureIgnoreCase);
            RankHierarchy = new Dictionary<Rank, List<Rank>>();

            Ranksfile = ServerCore.Settings.RegisterFile("Ranks.txt", true, LoadRanks);
            ServerCore.Settings.ReadSettings(Ranksfile);
        }

        /// <summary>
        /// Splits a comma delimited string of rank IDs into a list of ranks.
        /// </summary>
        /// <param name="rankString"></param>
        /// <returns></returns>
        public static List<Rank> SplitRanks(string rankString) {
            var splitRanks = rankString.Split(',');
            Console.WriteLine(rankString);
            return splitRanks.Select(s => ServerCore.Rankholder.GetRank(int.Parse(s))).ToList();
        }

        /// <summary>
        /// Splits a comma delimited string of rank steps into a list of int (steps).
        /// </summary>
        /// <param name="stepString">The comma delimited string containing the steps to split.</param>
        /// <returns></returns>
        public static List<int> SplitSteps(string stepString) {
            var splitSteps = stepString.Split(',');
            return splitSteps.Select(int.Parse).ToList();
        }

        /// <summary>
        /// Searches a list of ranks to see if it contains one of the ranks in another list.
        /// </summary>
        /// <param name="search">The list to search. if a rank from Compare is found here, the function returns true.</param>
        /// <param name="compare">The list to iterate through and compare against Search.</param>
        /// <returns></returns>
        public static bool RankListContains(List<Rank> search, List<Rank> compare)
        {
            return compare.Any(search.Contains);
        }

        /// <summary>
        /// Returns a rank object for modification or reference.
        /// </summary>
        /// <returns> A rank object for modification or reference.</returns>
        public Rank GetRank(int id) {
            return NumberList.ContainsKey(id) ? NumberList[id] : null;
        }

        public Rank GetRank(string name) {
            return NameList.ContainsKey(name) ? NameList[name] : null;
        }

        public void LoadRanks() {
            NameList.Clear();
            NumberList.Clear();

            foreach (var id in Ranksfile.SettingsDictionary.Keys) {
                ServerCore.Settings.SelectGroup(Ranksfile, id);

                var newRank = new Rank
                {
                    Id = int.Parse(id),
                    Name = ServerCore.Settings.ReadSetting(Ranksfile, "Name", "DEFAULT"),
                    Prefix = ServerCore.Settings.ReadSetting(Ranksfile, "Prefix", ""),
                    Suffix = ServerCore.Settings.ReadSetting(Ranksfile, "Suffix", ""),
                    NextRank = ServerCore.Settings.ReadSetting(Ranksfile, "NextRank", ""),
                    Group = ServerCore.Settings.ReadSetting(Ranksfile, "Group", "Default"),
                    PointsInRank = ServerCore.Settings.ReadSetting(Ranksfile, "Points", 5),
                    Op = bool.Parse(ServerCore.Settings.ReadSetting(Ranksfile, "IsOp", "false")),
                    Permissions =
                        PermissionContainer.SplitPermissions(
                            ServerCore.Settings.ReadSetting(Ranksfile, "Perms",
                                "map.joinmap,player.chat,player.build,player.delete,chat.useemotes,command.tp"))
                };

                NameList.Add(newRank.Name, newRank);
                NumberList.Add(newRank.Id, newRank);
            }

            if (NameList.Count == 0) {
                var guestRank = new Rank("Guest", "Default", "&f", "", false, 50, "Builder")
                {
                    Id = 0,
                    Permissions =
                        PermissionContainer.SplitPermissions(
                            "map.joinmap,player.chat,player.build,player.delete,chat.useemotes,command.tp")
                };

                var builderRank = new Rank("Builder", "Default", "&e", "", false, 15, "")
                {
                    Id = 1,
                    Permissions =
                        PermissionContainer.SplitPermissions(
                            "map.joinmap,player.chat,player.build,player.delete,chat.useemotes,command.tp")
                };

                var opRank = new Rank("Op", "Staff", "&9", "", true, 100, "")
                {
                    Id = 2,
                    Permissions =
                        PermissionContainer.SplitPermissions(
                            "map.joinmap,player.op,player.chat,player.build,player.delete,chat.useemotes,command.tp,command.bring,map.joinhiddenmap,chat.readstaffchat,player.kick,player.mute,player.stop")
                };

                var ownerRank = new Rank("Owner", "Staff", "&0", "", true, 0, "")
                {
                    Id = 3,
                    Permissions =
                        PermissionContainer.SplitPermissions(
                            "map.addmap,map.joinmap,player.op,player.chat,player.build,player.delete,chat.useemotes,command.tp,command.bring,map.joinhiddenmap,chat.readstaffchat,map.fillmap,player.kick,player.mute,player.stop,player.ban")
                };

                NumberList.Add(0, guestRank);
                NameList.Add("Guest", guestRank);

                NumberList.Add(1, builderRank);
                NameList.Add("Builder", builderRank);

                NumberList.Add(2, opRank);
                NameList.Add("Op", opRank);

                NumberList.Add(3, ownerRank);
                NameList.Add("Owner", ownerRank);

                SaveRanks();
            }

            CreateHierarchy();
            ServerCore.Logger.Log("RankContainer", "Ranks loaded.", LogType.Info);
        }

        /// <summary>
        /// Saves the internal ranks list to file. 
        /// </summary>
        public void SaveRanks() {
            Ranksfile.SettingsDictionary.Clear();

            foreach (var r in NumberList.Values) {
                ServerCore.Settings.SelectGroup(Ranksfile, r.Id.ToString());
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

        /// <summary>
        /// Is rank1 a higher rank than rank 2?
        /// </summary>
        /// <param name="rank1"></param>
        /// <param name="rank2"></param>
        /// <returns></returns>
        public bool IsRankHigher(Rank rank1, Rank rank2) {
            var myList = RankHierarchy[rank1];

            return !myList.Contains(rank2);
        }

        public Rank GetHighestRank(List<Rank> ranks) {
            var highest = ranks.First();

            foreach (var rank in ranks) {
                if (IsRankHigher(rank, highest))
                    highest = rank;
            }

            return highest;
        }

        /// <summary>
        /// Creates the Hierarchy table used for determining what rank is 'higher' than another.
        /// </summary>
        public void CreateHierarchy() {
            RankHierarchy.Clear();

            // -- Now that the groups, and the number of points in them has been esablished, we need to calculate what ranks are higher than others.
            foreach (var rank in NameList.Values) {
                var myList = new List<Rank>();
                var workingRank = rank;

                // -- Finds all ranks after our current one, and adds it to the list to say that these ranks are higher.
                while (true) {
                    if (GetRank(workingRank.NextRank) == null)
                        break;

                    workingRank = GetRank(workingRank.NextRank);
                    myList.Add(workingRank);
                }

                RankHierarchy.Add(rank, myList); // -- Add the list to the heirarchy system.
            }
        }
    }

    public class Rank {
        public string Name, Prefix, Suffix, NextRank, Group;
        public int Id, PointsInRank;
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
        /// <param name="rankName">Name of the rank.</param>
        /// <param name="rankGroup"></param>
        /// <param name="rankPrefix">The Prefix any user with this rank will have on their name.</param>
        /// <param name="rankSuffix">The Suffix any user with this rank will have on their name.</param>
        /// <param name="isOp">Indicates if this user is considered an Operator or higher.</param>
        /// <param name="rankPoints">Indicates the number of points contained in this rank. When this number has been exceeded, a user will be moved to NextRank.</param>
        /// <param name="nextRankUp">The Next rank the user will acheive if they have received all points possible for this rank. Blank indicates no higher rank.</param>
        public Rank(string rankName, string rankGroup, string rankPrefix, string rankSuffix, bool isOp, int rankPoints, string nextRankUp) {
            Name = rankName;
            Prefix = rankPrefix;
            Suffix = rankSuffix;
            Op = isOp;
            PointsInRank = rankPoints;
            NextRank = nextRankUp;
            Group = rankGroup;
        }

        public bool HasPermission(string permission) {
            Permission perm;
            return Permissions.TryGetValue(permission, out perm);
        }

        public bool HasAllPermissions(SortedDictionary<string, Permission> prms) {
            return prms.All(perm => HasPermission(perm.Key));
        }

        public bool HasAllPermissions(List<Permission> prms) {
           return prms.All(perm => HasPermission(perm.Fullname));
        }
    }
}
