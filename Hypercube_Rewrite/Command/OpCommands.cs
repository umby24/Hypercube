using System;
using System.Collections.Generic;

using Hypercube.Core;
using Hypercube.Client;

namespace Hypercube.Command {
    internal static class OpCommands {
        public static void Init(CommandHandler holder) {
            holder.AddCommand("/addrank", CAddrank);
            holder.AddCommand("/ban", CBan);
            holder.AddCommand("/delrank", CDelrank);
            holder.AddCommand("/kick", CKick);
            holder.AddCommand("/mute", CMute);
            holder.AddCommand("/pinfo", CPinfo);
            holder.AddCommand("/pushrank", CPushrank);
            holder.AddCommand("/setrank", CSetrank);
            holder.AddCommand("/stop", CStop);
            holder.AddCommand("/unban", CUnban);
            holder.AddCommand("/unmute", CUnmute);
            holder.AddCommand("/unstop", CUnstop);
        }

        #region AddRank
        static readonly Command CAddrank = new Command {
            Plugin = "",
            Group = "Op",
            Help = "§SAdds a rank to a player.<br>§SUsage: /addrank [Name] [RankName]",
            AllPerms = true,
            Console = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            Handler = AddrankHandler,
        };

        static void AddrankHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length < 2) {
                Chat.SendClientChat(client, "&4Error: &fYou are missing some arguments. Look at /cmdhelp addrank.");
                return;
            }

            args[0] = ServerCore.DB.GetPlayerName(args[0]);

            if (args[0] == "") {
                Chat.SendClientChat(client, "§ECould not find player.");
                return;
            }

            var newRank = ServerCore.Rankholder.GetRank(args[1]);

            if (newRank == null) {
                Chat.SendClientChat(client, "§ECould not find the rank you specified.");
                return;
            }

            //TODO: Add permissions

            var ranks = RankContainer.SplitRanks(ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "Rank"));
            var steps = RankContainer.SplitSteps(ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "RankStep"));
            ranks.Add(newRank);
            steps.Add(0);

            var rankString = "";

            foreach (var r in ranks)
                rankString += r.Id + ",";

            rankString = rankString.Substring(0, rankString.Length - 1);

            ServerCore.DB.SetDatabase(args[0], "PlayerDB", "Rank", rankString);
            ServerCore.DB.SetDatabase(args[0], "PlayerDB", "RankStep", string.Join(",", steps.ToArray()));

            if (ServerCore.Nh.LoggedClients.ContainsKey(args[0])) {
                ServerCore.Nh.LoggedClients[args[0]].CS.PlayerRanks = ranks;
                ServerCore.Nh.LoggedClients[args[0]].CS.RankSteps = steps;
                Chat.SendClientChat(ServerCore.Nh.LoggedClients[args[0]], "§SYou now have a rank of " + newRank.Prefix + newRank.Name + newRank.Suffix + "!");
                ServerCore.Nh.LoggedClients[args[0]].CS.FormattedName = newRank.Prefix + ServerCore.Nh.LoggedClients[args[0]].CS.LoginName + newRank.Suffix;
                Network.CPE.UpdateExtPlayerList(ServerCore.Nh.LoggedClients[args[0]]);
            }

            Chat.SendClientChat(client, "§S" + args[0] + "'s Rank was updated.");
        }
        #endregion
        #region Ban
        static readonly Command CBan = new Command {
            Plugin = "",
            Group = "Op",
            Help = "§SBans a player.<br>§SUsage: /Ban [Name] <Reason>",
            AllPerms = true,
            Console = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            Handler = BanHandler,
        };

        static void BanHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length > 2 || args.Length < 1) {
                Chat.SendClientChat(client, "§EIncorrect usage. See /cmdhelp ban");
                return;
            }

            if (!ServerCore.DB.ContainsPlayer(args[0])) {
                Chat.SendClientChat(client, "§ECould not find a user called '" + args[0] + "'.");
                return;
            }

            var reason = args.Length == 1 ? "You have been banned." : text2;

            Chat.SendGlobalChat("§SPlayer " + args[0] + " was banned by " + client.CS.FormattedName + "§S. (&f" + reason + "§S).", 0, true);
            ServerCore.DB.BanPlayer(ServerCore.DB.GetPlayerName(args[0]), reason, client.CS.LoginName);

            if (ServerCore.Nh.LoggedClients.ContainsKey(args[0])) 
                ServerCore.Nh.LoggedClients[args[0]].KickPlayer("Banned: " + reason);
            
        }
        #endregion
        #region Delete Rank
        static readonly Command CDelrank = new Command {
            Plugin = "",
            Group = "Op",
            Help = "§SRemoves a rank to a player.<br>§SUsage: /delrank [Name] [RankName]",
            Console = true,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            Handler = DelrankHandler,
        };

        static void DelrankHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length < 2) {
                Chat.SendClientChat(client, "&4Error: &fYou are missing some arguments. Look at /cmdhelp delrank.");
                return;
            }

            args[0] = ServerCore.DB.GetPlayerName(args[0]);

            if (args[0] == "") {
                Chat.SendClientChat(client, "§ECould not find player.");
                return;
            }

            var newRank = ServerCore.Rankholder.GetRank(args[1]);

            if (newRank == null) {
                Chat.SendClientChat(client, "§ECould not find the rank you specified.");
                return;
            }

            //TODO: Add permissions

            var ranks = RankContainer.SplitRanks(ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "Rank"));
            var steps = RankContainer.SplitSteps(ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "RankStep"));
            steps.RemoveAt(ranks.IndexOf(newRank));
            ranks.Remove(newRank);

            var rankString = "";

            foreach (var r in ranks)
                rankString += r.Id + ",";

            rankString = rankString.Substring(0, rankString.Length - 1);

            ServerCore.DB.SetDatabase(args[0], "PlayerDB", "Rank", rankString);
            ServerCore.DB.SetDatabase(args[0], "PlayerDB", "RankStep", string.Join(",", steps.ToArray()));

            if (ServerCore.Nh.LoggedClients.ContainsKey(args[0])) {
                ServerCore.Nh.LoggedClients[args[0]].CS.PlayerRanks = ranks;
                ServerCore.Nh.LoggedClients[args[0]].CS.RankSteps = steps;
                Chat.SendClientChat(ServerCore.Nh.LoggedClients[args[0]], "§SYour rank of " + newRank.Prefix + newRank.Name + newRank.Suffix + "§S has been removed.");
                ServerCore.Nh.LoggedClients[args[0]].CS.FormattedName = newRank.Prefix + ServerCore.Nh.LoggedClients[args[0]].CS.LoginName + newRank.Suffix;
                Network.CPE.UpdateExtPlayerList(ServerCore.Nh.LoggedClients[args[0]]);
            }

            Chat.SendClientChat(client, "§S" + args[0] + "'s Ranks were updated.");
        }
        #endregion
        #region Kick
        static readonly Command CKick = new Command {
            Plugin = "",
            Group = "Op",
            Help = "§SKicks a player.<br>§SUsage: /kick [Name] <Reason>",
            AllPerms = true,
            Console = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            Handler = KickHandler,
        };

        static void KickHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length > 2 || args.Length < 1) {
                Chat.SendClientChat(client, "§EIncorrect usage. See /cmdhelp ban");
                return;
            }

            var reason = args.Length == 1 ? "Kicked." : text2;

            if (ServerCore.Nh.LoggedClients.ContainsKey(args[0])) {
                ServerCore.Logger.Log("Command", "Player " + args[0] + " was kicked by " + client.CS.LoginName + ". (" + reason + ")", LogType.Info);
                ServerCore.Nh.LoggedClients[args[0]].KickPlayer(reason, true);
            } else
                Chat.SendClientChat(client, "§ECould not find a user with the name '" + args[0] + "'.");
        }
        #endregion
        #region Mute
        static readonly Command CMute = new Command {
            Plugin = "",
            Group = "Op",
            Help = "§SMutes a player, he can't speak now.<br>§SUsage: /mute [Name] <minutes>",
            AllPerms = true,
            Console = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            Handler = MuteHandler,
        };

        static void MuteHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length < 1 || args.Length > 2) {
                Chat.SendClientChat(client, "§EInvalid number of arguments, see /cmdhelp mute.");
                return;
            }

            if (!ServerCore.DB.ContainsPlayer(args[0])) {
                Chat.SendClientChat(client, "§ECould not find a player called '" + args[0] + "'.");
                return;
            }

            var muteDuration = 999999;

            if (args.Length == 2)
                muteDuration = int.Parse(text2);

            Chat.SendGlobalChat("§SPlayer " + args[0] + "§S was muted for " + muteDuration + " minutes.", 0, true);

            var mutedUntil = DateTime.UtcNow.AddMinutes(muteDuration) - ServerCore.UnixEpoch;

            ServerCore.DB.MutePlayer(args[0], (int)mutedUntil.TotalSeconds, "You are muted");

            if (ServerCore.Nh.LoggedClients.ContainsKey(args[0]))
                ServerCore.Nh.LoggedClients[args[0]].CS.MuteTime = (int)mutedUntil.TotalSeconds;
        }
        #endregion
        #region PInfo
        static readonly Command CPinfo = new Command {
            Plugin = "",
            Group = "Op",
            Help = "§SGives some information about a player.<br>§SUsage: /pinfo [Name]",
            AllPerms = true,
            Console = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            Handler = PInfoHandler,
        };

        static void PInfoHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length == 0) {
                Chat.SendClientChat(client, "§EUsage: /pinfo [name]");
                return;
            }

            args[0] = ServerCore.DB.GetPlayerName(args[0]);

            if (args[0] == "") {
                Chat.SendClientChat(client, "§ECould not find player.");
                return;
            }

            var playerInfo = "§SPlayerinfo:<br>";

            //ServerCore.DB.GetDataTable("SELECT * FROM PlayerDB WHERE Name='" + args[0] + "' LIMIT 1");
            playerInfo += "§SNumber: " + ServerCore.DB.GetDatabaseInt(args[0], "PlayerDB", "Number") + "<br>";
            playerInfo += "§SName: " + args[0] + "<br>";

            var playerRanks = RankContainer.SplitRanks(ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "Rank"));
            playerInfo += "§SRank(s): ";

            foreach (var r in playerRanks)
                playerInfo += r.Prefix + r.Name + r.Suffix + ",";

            playerInfo = playerInfo.Substring(0, playerInfo.Length - 1); // -- Remove the final comma.
            playerInfo += "<br>";
            playerInfo += "§SIP: " + ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "IP") + "<br>";
            playerInfo += "§SLogins: " + ServerCore.DB.GetDatabaseInt(args[0], "PlayerDB", "LoginCounter") + "<br>";
            playerInfo += "§SKicks: " + ServerCore.DB.GetDatabaseInt(args[0], "PlayerDB", "KickCounter") + "( " + ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "KickMessage") + ")<br>";

            if (ServerCore.DB.GetDatabaseInt(args[0], "PlayerDB", "Banned") > 0)
                playerInfo += "§SBanned: " + ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "BanMessage") + " (" + ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "BannedBy") + ")<br>";

            if (ServerCore.DB.GetDatabaseInt(args[0], "PlayerDB", "Stopped") > 0)
                playerInfo += "§SStopped: " + ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "StopMessage") + " (" + ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "StoppedBy") + ")<br>";

            if (ServerCore.DB.GetDatabaseInt(args[0], "PlayerDB", "Time_Muted") > ServerCore.GetCurrentUnixTime())
                playerInfo += "§SMuted: " + ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "MuteMessage") + "<br>";

            Chat.SendClientChat(client, playerInfo);
        }
        #endregion
        #region Pushrank
        static readonly Command CPushrank = new Command {
            Plugin = "",
            Group = "Op",
            Help = "§SSets a rank as the player's active rank. (Sets their name color)<br>§SUsage: /pushrank [Name] [RankName]",
            AllPerms = true,
            Console = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            Handler = HandlePushRank,
        };

        static void HandlePushRank(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length < 2) {
                Chat.SendClientChat(client, "&4Error: &fYou are missing some arguments. Look at /cmdhelp pushrank.");
                return;
            }

            args[0] = ServerCore.DB.GetPlayerName(args[0]);

            if (args[0] == "") {
                Chat.SendClientChat(client, "§ECould not find player.");
                return;
            }

            var newRank = ServerCore.Rankholder.GetRank(args[1]);

            if (newRank == null) {
                Chat.SendClientChat(client, "&4Error: &fCould not find the rank you specified.");
                return;
            }
            //TODO: Add permissions

            var ranks = RankContainer.SplitRanks(ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "Rank"));

            if (!ranks.Contains(newRank)) {
                Chat.SendClientChat(client, "&4Error: &fPlayer '" + args[0] + "' does not have rank '" + args[1] + "'.");
                return;
            }

            var steps = RankContainer.SplitSteps(ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "RankStep"));
            var tempInt = steps[ranks.IndexOf(newRank)];
            steps.RemoveAt(ranks.IndexOf(newRank));
            ranks.Remove(newRank);
            ranks.Add(newRank);
            steps.Add(tempInt);

            var rankString = "";

            foreach (var r in ranks)
                rankString += r.Id + ",";

            rankString = rankString.Substring(0, rankString.Length - 1);

            ServerCore.DB.SetDatabase(args[0], "PlayerDB", "Rank", rankString);
            ServerCore.DB.SetDatabase(args[0], "PlayerDB", "RankStep", string.Join(",", steps.ToArray()));


            if (ServerCore.Nh.LoggedClients.ContainsKey(args[0])) {
                ServerCore.Nh.LoggedClients[args[0]].CS.PlayerRanks = ranks;
                ServerCore.Nh.LoggedClients[args[0]].CS.RankSteps = steps;
                ServerCore.Nh.LoggedClients[args[0]].CS.FormattedName = newRank.Prefix + ServerCore.Nh.LoggedClients[args[0]].CS.LoginName + newRank.Suffix;
            }

            Chat.SendClientChat(client, "§S" + args[0] + "'s Rank was updated.");
            Network.CPE.UpdateExtPlayerList(client);
        }
        #endregion
        #region Setrank
        static readonly Command CSetrank = new Command {
            Plugin = "",
            Group = "Op",
            Help = "§SChanges the step of a player's rank.<br>§SUsage: /setrank [Name] [RankName] [Step]",
            Console = true,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            Handler = SetrankHandler,
        };

        static void SetrankHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length < 3) {
                Chat.SendClientChat(client, "&4Error: &fYou are missing some arguments. Look at /cmdhelp setrank.");
                return;
            }

            args[0] = ServerCore.DB.GetPlayerName(args[0]);

            if (args[0] == "") {
                Chat.SendClientChat(client, "§ECould not find player.");
                return;
            }

            var newRank = ServerCore.Rankholder.GetRank(args[1]);

            if (newRank == null) {
                Chat.SendClientChat(client, "&4Error: &fCould not find the rank you specified.");
                return;
            }
            //TODO: Add permissions
            var ranks = RankContainer.SplitRanks(ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "Rank"));

            if (!ranks.Contains(newRank)) {
                Chat.SendClientChat(client, "&4Error: &fPlayer '" + args[0] + "' does not have rank '" + args[1] + "'.");
                return;
            }

            var steps = RankContainer.SplitSteps(ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "RankStep"));
            steps[ranks.IndexOf(newRank)] = int.Parse(args[2]);

            ServerCore.DB.SetDatabase(args[0], "PlayerDB", "RankStep", string.Join(",", steps.ToArray()));


            if (ServerCore.Nh.LoggedClients.ContainsKey(args[0])) {
                ServerCore.Nh.LoggedClients[args[0]].CS.RankSteps = steps;
                Chat.SendClientChat(ServerCore.Nh.LoggedClients[args[0]], "§SYour rank of " + newRank.Prefix + newRank.Name + newRank.Suffix + " has been updated.");
                ServerCore.Nh.LoggedClients[args[0]].CS.FormattedName = newRank.Prefix + ServerCore.Nh.LoggedClients[args[0]].CS.LoginName + newRank.Suffix;
            }

            Chat.SendClientChat(client, "§S" + args[0] + "'s Rank was updated.");
            Network.CPE.UpdateExtPlayerList(client);
        }
        #endregion
        #region Stop
        static readonly Command CStop = new Command {
            Plugin = "",
            Group = "Op",
            Help = "§SStops a player, he can't built now.<br>§SUsage: /Stop [Name] <Reason>",
            AllPerms = true,
            Console = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            Handler = Handlestop,
        };

        static void Handlestop(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length == 0) {
                Chat.SendClientChat(client, "§EIncorrect number of arguments. See /cmdhelp stop");
                return;
            }

            if (!ServerCore.DB.ContainsPlayer(args[0])) {
                Chat.SendClientChat(client, "§ECould not find a user with the name '" + args[0] + "'.");
                return;
            }

            var stopReason = "You have been stopped.";

            if (args.Length > 1) 
                stopReason = text2;

            Chat.SendGlobalChat("§SPlayer " + args[0] + "§S was stopped by " + client.CS.FormattedName + "§S. (&f" + stopReason + "§S)", 0, true);

            ServerCore.DB.StopPlayer(args[0], stopReason, client.CS.LoginName);

            if (ServerCore.Nh.LoggedClients.ContainsKey(args[0])) 
                ServerCore.Nh.LoggedClients[args[0]].CS.Stopped = true;
                    
        }
        #endregion
        #region Unban
        static readonly Command CUnban = new Command {
            Plugin = "",
            Group = "Op",
            Help = "§SUnbans a player.<br>§SUsage: /Unban [Name]",
            Console = true,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            Handler = UnbanHandler,
        };

        static void UnbanHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length < 1) {
                Chat.SendClientChat(client, "§EInvalid number of arguments, see /cmdhelp unban.");
                return;
            }

            if (!ServerCore.DB.ContainsPlayer(args[0])) {
                Chat.SendClientChat(client, "§ECould not find a user with the name '" + args[0] + "'.");
                return;
            }

            Chat.SendGlobalChat("§SPlayer " + args[0] + "§S was unbanned by " + client.CS.FormattedName + "§S.", 0, true);

            ServerCore.DB.UnbanPlayer(args[0]);
        }
        #endregion
        #region Unmute
        static readonly Command CUnmute = new Command {
            Plugin = "",
            Group = "Op",
            Help = "§SUnmutes a player, he can speak now.<br>§SUsage: /unmute [Name]",
            Console = true,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            Handler = UnmuteHandler,
        };

        static void UnmuteHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length < 1) {
                Chat.SendClientChat(client, "§EInvalid number of arguments, see /cmdhelp unmute.");
                return;
            }

            if (!ServerCore.DB.ContainsPlayer(args[0])) {
                Chat.SendClientChat(client, "§ECould not find a user with the name '" + args[0] + "'.");
                return;
            }

            Chat.SendGlobalChat("§SPlayer " + args[0] + "§S was unmuted.", 0, true);

            ServerCore.DB.UnmutePlayer(args[0]);


            if (ServerCore.Nh.LoggedClients.ContainsKey(args[0])) 
                ServerCore.Nh.LoggedClients[args[0]].CS.MuteTime = 0;
        }
        #endregion
        #region Unstop
        static readonly Command CUnstop = new Command {
            Plugin = "",
            Group = "Op",
            Help = "§SUnstops a player, he can build now.<br>§SUsage: /Unstop [Name]",
            Console = true,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            Handler = UnstopHandler,
        };

        static void UnstopHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length < 1) {
                Chat.SendClientChat(client, "§EInvalid number of arguments, see /cmdhelp unstop.");
                return;
            }

            if (!ServerCore.DB.ContainsPlayer(args[0])) {
                Chat.SendClientChat(client, "§ECould not find a user with the name '" + args[0] + "'.");
                return;
            }

            Chat.SendGlobalChat("§SPlayer " + args[0] + "§S was unstopped by " + client.CS.FormattedName + "§S.");

            ServerCore.DB.UnstopPlayer(args[0]);

            if (ServerCore.Nh.LoggedClients.ContainsKey(args[0]))
                ServerCore.Nh.LoggedClients[args[0]].CS.Stopped = false;
        }
        #endregion
    }
}
