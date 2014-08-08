﻿using System;
using System.Collections.Generic;
using System.Linq;
using Hypercube.Libraries;

namespace Hypercube.Core
{
    public class RankContainer {
        public SortedDictionary<int, Rank> NumberList;
        public SortedDictionary<string, Rank> NameList;

        public ISettings Ranksfile;
        Hypercube ServerCore;

        public RankContainer(Hypercube core) {
            NumberList = new SortedDictionary<int, Rank>();
            NameList = new SortedDictionary<string, Rank>(StringComparer.InvariantCultureIgnoreCase);

            Ranksfile = core.Settings.RegisterFile("Ranks.txt", true, LoadRanks);
            ServerCore = core;
            ServerCore.Settings.ReadSettings(Ranksfile);
        }

        /// <summary>
        /// Splits a comma delimited string of rank IDs into a list of ranks.
        /// </summary>
        /// <param name="serverCore"></param>
        /// <param name="rankString"></param>
        /// <returns></returns>
        public static List<Rank> SplitRanks(Hypercube serverCore, string rankString) {
            var splitRanks = rankString.Split(',');

            return splitRanks.Select(s => serverCore.Rankholder.GetRank(int.Parse(s))).ToList();
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
            return compare.Any(r => search.Contains(r));
        }

        /// <summary>
        /// Returns a rank object for modification or reference.
        /// </summary>
        /// <returns> A rank object for modification or reference.</returns>
        public Rank GetRank(int id) {
            if (NumberList.ContainsKey(id))
                return NumberList[id];
            
            return null;
        }

        public Rank GetRank(string name) {
            if (NameList.ContainsKey(name))
                return NameList[name];
            
            return null;
        }

        public void LoadRanks() {
            NameList.Clear();
            NumberList.Clear();

            foreach (var id in Ranksfile.Settings.Keys) {
                ServerCore.Settings.SelectGroup(Ranksfile, id);

                var newRank = new Rank
                {
                    ID = int.Parse(id),
                    Name = ServerCore.Settings.ReadSetting(Ranksfile, "Name", "DEFAULT"),
                    Prefix = ServerCore.Settings.ReadSetting(Ranksfile, "Prefix", ""),
                    Suffix = ServerCore.Settings.ReadSetting(Ranksfile, "Suffix", ""),
                    NextRank = ServerCore.Settings.ReadSetting(Ranksfile, "NextRank", ""),
                    Group = ServerCore.Settings.ReadSetting(Ranksfile, "Group", "Default"),
                    PointsInRank = ServerCore.Settings.ReadSetting(Ranksfile, "Points", 5),
                    Op = bool.Parse(ServerCore.Settings.ReadSetting(Ranksfile, "IsOp", "false")),
                    Permissions =
                        PermissionContainer.SplitPermissions(ServerCore,
                            ServerCore.Settings.ReadSetting(Ranksfile, "Perms",
                                "map.joinmap,player.chat,player.build,player.delete,chat.useemotes,command.tp"))
                };

                NameList.Add(newRank.Name, newRank);
                NumberList.Add(newRank.ID, newRank);
            }

            if (NameList.Count == 0) {
                var guestRank = new Rank("Guest", "Default", "&f", "", false, 50, "Builder")
                {
                    ID = 0,
                    Permissions =
                        PermissionContainer.SplitPermissions(ServerCore,
                            "map.joinmap,player.chat,player.build,player.delete,chat.useemotes,command.tp")
                };

                var builderRank = new Rank("Builder", "Default", "&e", "", false, 15, "")
                {
                    ID = 1,
                    Permissions =
                        PermissionContainer.SplitPermissions(ServerCore,
                            "map.joinmap,player.chat,player.build,player.delete,chat.useemotes,command.tp")
                };

                var opRank = new Rank("Op", "Staff", "&9", "", true, 100, "")
                {
                    ID = 2,
                    Permissions =
                        PermissionContainer.SplitPermissions(ServerCore,
                            "map.joinmap,player.op,player.chat,player.build,player.delete,chat.useemotes,command.tp,command.bring,map.joinhiddenmap,chat.readstaffchat")
                };

                var ownerRank = new Rank("Owner", "Staff", "&0", "", true, 0, "")
                {
                    ID = 3,
                    Permissions =
                        PermissionContainer.SplitPermissions(ServerCore,
                            "map.joinmap,player.op,player.chat,player.build,player.delete,chat.useemotes,command.tp,command.bring,map.joinhiddenmap,chat.readstaffchat,map.fillmap")
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

            ServerCore.Logger.Log("RankContainer", "Ranks loaded.", LogType.Info);
        }

        /// <summary>
        /// Saves the internal ranks list to file. 
        /// </summary>
        public void SaveRanks() {
            Ranksfile.Settings.Clear();

            foreach (var r in NumberList.Values) {
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
    }
}
