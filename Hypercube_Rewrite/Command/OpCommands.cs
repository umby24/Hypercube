using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hypercube.Core;
using Hypercube.Client;

namespace Hypercube.Command {
    internal static class OpCommands {
        public static void Init(CommandHandler holder) {
            holder.AddCommand("/addrank", cAddrank);
            holder.AddCommand("/ban", cBan);
            holder.AddCommand("/delrank", cDelrank);
            holder.AddCommand("/kick", cKick);
            holder.AddCommand("/mute", cMute);
            holder.AddCommand("/pinfo", cPinfo);
            holder.AddCommand("/pushrank", cPushrank);
            holder.AddCommand("/setrank", cSetrank);
            holder.AddCommand("/stop", cStop);
            holder.AddCommand("/unban", cUnban);
            holder.AddCommand("/unmute", cUnmute);
            holder.AddCommand("/unstop", cUnstop);
        }

        #region AddRank
        static readonly Command cAddrank = new Command {
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

        static void AddrankHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length < 2) {
                Chat.SendClientChat(Client, "&4Error: &fYou are missing some arguments. Look at /cmdhelp addrank.");
                return;
            }

            args[0] = Client.ServerCore.DB.GetPlayerName(args[0]);

            if (args[0] == "") {
                Chat.SendClientChat(Client, "§ECould not find player.");
                return;
            }

            var newRank = Client.ServerCore.Rankholder.GetRank(args[1]);

            if (newRank == null) {
                Chat.SendClientChat(Client, "§ECould not find the rank you specified.");
                return;
            }

            //TODO: Add permissions

            var Ranks = RankContainer.SplitRanks(Client.ServerCore, Client.ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "Rank"));
            var Steps = RankContainer.SplitSteps(Client.ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "RankStep"));
            Ranks.Add(newRank);
            Steps.Add(0);

            string RankString = "";

            foreach (Rank r in Ranks)
                RankString += r.ID.ToString() + ",";

            RankString = RankString.Substring(0, RankString.Length - 1);

            Client.ServerCore.DB.SetDatabase(args[0], "PlayerDB", "Rank", RankString);
            Client.ServerCore.DB.SetDatabase(args[0], "PlayerDB", "RankStep", string.Join(",", Steps.ToArray()));

            foreach (NetworkClient c in Client.ServerCore.nh.Clients) {
                if (c.CS.LoginName.ToLower() == args[0]) {
                    c.CS.PlayerRanks = Ranks;
                    c.CS.RankSteps = Steps;
                    Chat.SendClientChat(c, "§SYou now have a rank of " + newRank.Prefix + newRank.Name + newRank.Suffix + "!");
                    c.CS.FormattedName = newRank.Prefix + c.CS.LoginName + newRank.Suffix;
                    Network.CPE.UpdateExtPlayerList(c);
                }
            }

            Chat.SendClientChat(Client, "§S" + args[0] + "'s Rank was updated.");
        }
        #endregion
        #region Ban
        static readonly Command cBan = new Command {
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

        static void BanHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length > 2 || args.Length < 1) {
                Chat.SendClientChat(Client, "§EIncorrect usage. See /cmdhelp ban");
                return;
            }

            if (!Client.ServerCore.DB.ContainsPlayer(args[0])) {
                Chat.SendClientChat(Client, "§ECould not find a user called '" + args[0] + "'.");
                return;
            }

            string Reason;

            if (args.Length == 1)
                Reason = "You have been banned.";
            else
                Reason = Text2;

            Chat.SendGlobalChat(Client.ServerCore, "§SPlayer " + args[0] + " was banned by " + Client.CS.FormattedName + "§S. (&f" + Reason + "§S).", 0, true);
            Client.ServerCore.DB.BanPlayer(Client.ServerCore.DB.GetPlayerName(args[0]), Reason, Client.CS.LoginName);

            for (int i = 0; i < Client.ServerCore.nh.Clients.Count; i++) {
                if (Client.ServerCore.nh.Clients[i].CS.LoginName.ToLower() == args[0].ToLower() && Client.ServerCore.nh.Clients[i].CS.LoggedIn) {
                    Client.ServerCore.nh.Clients[i].KickPlayer("Banned: " + Reason);
                    break;
                }
            }

        }
        #endregion
        #region Delete Rank
        static readonly Command cDelrank = new Command {
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

        static void DelrankHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length < 2) {
                Chat.SendClientChat(Client, "&4Error: &fYou are missing some arguments. Look at /cmdhelp delrank.");
                return;
            }

            args[0] = Client.ServerCore.DB.GetPlayerName(args[0]);

            if (args[0] == "") {
                Chat.SendClientChat(Client, "§ECould not find player.");
                return;
            }

            var newRank = Client.ServerCore.Rankholder.GetRank(args[1]);

            if (newRank == null) {
                Chat.SendClientChat(Client, "§ECould not find the rank you specified.");
                return;
            }

            //TODO: Add permissions

            var Ranks = RankContainer.SplitRanks(Client.ServerCore, Client.ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "Rank"));
            var Steps = RankContainer.SplitSteps(Client.ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "RankStep"));
            Steps.RemoveAt(Ranks.IndexOf(newRank));
            Ranks.Remove(newRank);

            string RankString = "";

            foreach (Rank r in Ranks)
                RankString += r.ID.ToString() + ",";

            RankString = RankString.Substring(0, RankString.Length - 1);

            Client.ServerCore.DB.SetDatabase(args[0], "PlayerDB", "Rank", RankString);
            Client.ServerCore.DB.SetDatabase(args[0], "PlayerDB", "RankStep", string.Join(",", Steps.ToArray()));

            foreach (NetworkClient c in Client.ServerCore.nh.Clients) {
                if (c.CS.LoginName.ToLower() == args[0]) {
                    c.CS.PlayerRanks = Ranks;
                    c.CS.RankSteps = Steps;
                    Chat.SendClientChat(c, "§SYour rank of " + newRank.Prefix + newRank.Name + newRank.Suffix + "§S has been removed.");
                    c.CS.FormattedName = newRank.Prefix + c.CS.LoginName + newRank.Suffix;
                    Network.CPE.UpdateExtPlayerList(c);
                }
            }

            Chat.SendClientChat(Client, "§S" + args[0] + "'s Ranks were updated.");
        }
        #endregion
        #region Kick
        static readonly Command cKick = new Command {
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

        static void KickHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length > 2 || args.Length < 1) {
                Chat.SendClientChat(Client, "§EIncorrect usage. See /cmdhelp ban");
                return;
            }

            string Reason;
            bool kicked = false;

            if (args.Length == 1)
                Reason = "Kicked.";
            else
                Reason = Text2;

            for (int i = 0; i < Client.ServerCore.nh.Clients.Count; i++) {
                if (Client.ServerCore.nh.Clients[i].CS.LoginName.ToLower() == args[0].ToLower() && Client.ServerCore.nh.Clients[i].CS.LoggedIn) {
                    Client.ServerCore.Logger.Log("Command", "Player " + args[0] + " was kicked by " + Client.CS.LoginName + ". (" + Reason + ")", LogType.Info);
                    Client.ServerCore.nh.Clients[i].KickPlayer(Reason, true);
                    kicked = true;
                    break;
                }
            }

            if (!kicked)
                Chat.SendClientChat(Client, "§ECould not find a user with the name '" + args[0] + "'.");
        }
        #endregion
        #region Mute
        static readonly Command cMute = new Command {
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

        static void MuteHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length < 1 || args.Length > 2) {
                Chat.SendClientChat(Client, "§EInvalid number of arguments, see /cmdhelp mute.");
                return;
            }

            if (!Client.ServerCore.DB.ContainsPlayer(args[0])) {
                Chat.SendClientChat(Client, "§ECould not find a player called '" + args[0] + "'.");
                return;
            }

            int MuteDuration = 999999;

            if (args.Length == 2)
                MuteDuration = int.Parse(Text2);

            Chat.SendGlobalChat(Client.ServerCore, "§SPlayer " + args[0] + "§S was muted for " + MuteDuration.ToString() + " minutes.", 0, true);

            var MutedUntil = DateTime.UtcNow.AddMinutes((double)MuteDuration) - Hypercube.UnixEpoch;

            Client.ServerCore.DB.MutePlayer(args[0], (int)MutedUntil.TotalSeconds, "You are muted");

            foreach (NetworkClient c in Client.ServerCore.nh.Clients) {
                if (c.CS.LoginName.ToLower() == args[0].ToLower() && c.CS.LoggedIn) {
                    c.CS.MuteTime = (int)MutedUntil.TotalSeconds;
                    break;
                }
            }
        }
        #endregion
        #region PInfo
        static readonly Command cPinfo = new Command {
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

        static void PInfoHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length == 0) {
                Chat.SendClientChat(Client, "§EUsage: /pinfo [name]");
                return;
            }

            args[0] = Client.ServerCore.DB.GetPlayerName(args[0]);

            if (args[0] == "") {
                Chat.SendClientChat(Client, "§ECould not find player.");
                return;
            }

            string PlayerInfo = "§SPlayerinfo:<br>";

            var dt = Client.ServerCore.DB.GetDataTable("SELECT * FROM PlayerDB WHERE Name='" + args[0] + "' LIMIT 1");
            PlayerInfo += "§SNumber: " + Client.ServerCore.DB.GetDatabaseInt(args[0], "PlayerDB", "Number").ToString() + "<br>";
            PlayerInfo += "§SName: " + args[0] + "<br>";

            var PlayerRanks = RankContainer.SplitRanks(Client.ServerCore, Client.ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "Rank"));
            PlayerInfo += "§SRank(s): ";

            foreach (Rank r in PlayerRanks)
                PlayerInfo += r.Prefix + r.Name + r.Suffix + ",";

            PlayerInfo = PlayerInfo.Substring(0, PlayerInfo.Length - 1); // -- Remove the final comma.
            PlayerInfo += "<br>";
            PlayerInfo += "§SIP: " + Client.ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "IP") + "<br>";
            PlayerInfo += "§SLogins: " + Client.ServerCore.DB.GetDatabaseInt(args[0], "PlayerDB", "LoginCounter").ToString() + "<br>";
            PlayerInfo += "§SKicks: " + Client.ServerCore.DB.GetDatabaseInt(args[0], "PlayerDB", "KickCounter").ToString() + "( " + Client.ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "KickMessage") + ")<br>";

            if (Client.ServerCore.DB.GetDatabaseInt(args[0], "PlayerDB", "Banned") > 0)
                PlayerInfo += "§SBanned: " + Client.ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "BanMessage") + " (" + Client.ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "BannedBy") + ")<br>";

            if (Client.ServerCore.DB.GetDatabaseInt(args[0], "PlayerDB", "Stopped") > 0)
                PlayerInfo += "§SStopped: " + Client.ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "StopMessage") + " (" + Client.ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "StoppedBy") + ")<br>";

            if (Client.ServerCore.DB.GetDatabaseInt(args[0], "PlayerDB", "Time_Muted") > Hypercube.GetCurrentUnixTime())
                PlayerInfo += "§SMuted: " + Client.ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "MuteMessage") + "<br>";

            Chat.SendClientChat(Client, PlayerInfo);
        }
        #endregion
        #region Pushrank
        static readonly Command cPushrank = new Command {
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

        static void HandlePushRank(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length < 2) {
                Chat.SendClientChat(Client, "&4Error: &fYou are missing some arguments. Look at /cmdhelp pushrank.");
                return;
            }

            args[0] = Client.ServerCore.DB.GetPlayerName(args[0]);

            if (args[0] == "") {
                Chat.SendClientChat(Client, "§ECould not find player.");
                return;
            }

            var newRank = Client.ServerCore.Rankholder.GetRank(args[1]);

            if (newRank == null) {
                Chat.SendClientChat(Client, "&4Error: &fCould not find the rank you specified.");
                return;
            }
            //TODO: Add permissions

            var Ranks = RankContainer.SplitRanks(Client.ServerCore, Client.ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "Rank"));

            if (!Ranks.Contains(newRank)) {
                Chat.SendClientChat(Client, "&4Error: &fPlayer '" + args[0] + "' does not have rank '" + args[1] + "'.");
                return;
            }

            var Steps = RankContainer.SplitSteps(Client.ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "RankStep"));
            int TempInt = Steps[Ranks.IndexOf(newRank)];
            Steps.RemoveAt(Ranks.IndexOf(newRank));
            Ranks.Remove(newRank);
            Ranks.Add(newRank);
            Steps.Add(TempInt);

            string RankString = "";

            foreach (Rank r in Ranks)
                RankString += r.ID.ToString() + ",";

            RankString = RankString.Substring(0, RankString.Length - 1);

            Client.ServerCore.DB.SetDatabase(args[0], "PlayerDB", "Rank", RankString);
            Client.ServerCore.DB.SetDatabase(args[0], "PlayerDB", "RankStep", string.Join(",", Steps.ToArray()));

            foreach (NetworkClient c in Client.ServerCore.nh.Clients) {
                if (c.CS.LoginName.ToLower() == args[0]) {
                    c.CS.PlayerRanks = Ranks;
                    c.CS.RankSteps = Steps;
                    c.CS.FormattedName = newRank.Prefix + c.CS.LoginName + newRank.Suffix;
                }
            }

            Chat.SendClientChat(Client, "§S" + args[0] + "'s Rank was updated.");
            Network.CPE.UpdateExtPlayerList(Client);
        }
        #endregion
        #region Setrank
        static readonly Command cSetrank = new Command {
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

        static void SetrankHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length < 3) {
                Chat.SendClientChat(Client, "&4Error: &fYou are missing some arguments. Look at /cmdhelp setrank.");
                return;
            }

            args[0] = Client.ServerCore.DB.GetPlayerName(args[0]);

            if (args[0] == "") {
                Chat.SendClientChat(Client, "§ECould not find player.");
                return;
            }

            var newRank = Client.ServerCore.Rankholder.GetRank(args[1]);

            if (newRank == null) {
                Chat.SendClientChat(Client, "&4Error: &fCould not find the rank you specified.");
                return;
            }
            //TODO: Add permissions
            var Ranks = RankContainer.SplitRanks(Client.ServerCore, Client.ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "Rank"));

            if (!Ranks.Contains(newRank)) {
                Chat.SendClientChat(Client, "&4Error: &fPlayer '" + args[0] + "' does not have rank '" + args[1] + "'.");
                return;
            }

            var Steps = RankContainer.SplitSteps(Client.ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "RankStep"));
            Steps[Ranks.IndexOf(newRank)] = int.Parse(args[2]);

            Client.ServerCore.DB.SetDatabase(args[0], "PlayerDB", "RankStep", string.Join(",", Steps.ToArray()));

            foreach (NetworkClient c in Client.ServerCore.nh.Clients) {
                if (c.CS.LoginName.ToLower() == args[0]) {
                    c.CS.RankSteps = Steps;
                    Chat.SendClientChat(c, "§SYour rank of " + newRank.Prefix + newRank.Name + newRank.Suffix + " has been updated.");
                    c.CS.FormattedName = newRank.Prefix + c.CS.LoginName + newRank.Suffix;
                }
            }

            Chat.SendClientChat(Client, "§S" + args[0] + "'s Rank was updated.");
            Network.CPE.UpdateExtPlayerList(Client);
        }
        #endregion
        #region Stop
        static readonly Command cStop = new Command {
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

        static void Handlestop(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length == 0) {
                Chat.SendClientChat(Client, "§EIncorrect number of arguments. See /cmdhelp stop");
                return;
            }

            if (!Client.ServerCore.DB.ContainsPlayer(args[0])) {
                Chat.SendClientChat(Client, "§ECould not find a user with the name '" + args[0] + "'.");
                return;
            }

            string StopReason = "You have been stopped.";

            if (args.Length > 1) 
                StopReason = Text2;

            Chat.SendGlobalChat(Client.ServerCore, "§SPlayer " + args[0] + "§S was stopped by " + Client.CS.FormattedName + "§S. (&f" + StopReason + "§S)", 0, true);

            Client.ServerCore.DB.StopPlayer(args[0], StopReason, Client.CS.LoginName);

            foreach (NetworkClient c in Client.ServerCore.nh.Clients) {
                if (c.CS.LoginName.ToLower() == args[0].ToLower()) {
                    c.CS.Stopped = true;
                    break;
                }
            }
        }
        #endregion
        #region Unban
        static readonly Command cUnban = new Command {
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

        static void UnbanHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length < 1) {
                Chat.SendClientChat(Client, "§EInvalid number of arguments, see /cmdhelp unban.");
                return;
            }

            if (!Client.ServerCore.DB.ContainsPlayer(args[0])) {
                Chat.SendClientChat(Client, "§ECould not find a user with the name '" + args[0] + "'.");
                return;
            }

            Chat.SendGlobalChat(Client.ServerCore, "§SPlayer " + args[0] + "§S was unbanned by " + Client.CS.FormattedName + "§S.", 0, true);

            Client.ServerCore.DB.UnbanPlayer(args[0]);
        }
        #endregion
        #region Unmute
        static readonly Command cUnmute = new Command {
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

        static void UnmuteHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length < 1) {
                Chat.SendClientChat(Client, "§EInvalid number of arguments, see /cmdhelp unmute.");
                return;
            }

            if (!Client.ServerCore.DB.ContainsPlayer(args[0])) {
                Chat.SendClientChat(Client, "§ECould not find a user with the name '" + args[0] + "'.");
                return;
            }

            Chat.SendGlobalChat(Client.ServerCore, "§SPlayer " + args[0] + "§S was unmuted.", 0, true);

            Client.ServerCore.DB.UnmutePlayer(args[0]);

            foreach (NetworkClient c in Client.ServerCore.nh.Clients) {
                if (c.CS.LoginName.ToLower() == args[0].ToLower() && c.CS.LoggedIn) {
                    c.CS.MuteTime = 0;
                    break;
                }
            }
        }
        #endregion
        #region Unstop
        static readonly Command cUnstop = new Command {
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

        static void UnstopHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length < 1) {
                Chat.SendClientChat(Client, "§EInvalid number of arguments, see /cmdhelp unstop.");
                return;
            }

            if (!Client.ServerCore.DB.ContainsPlayer(args[0])) {
                Chat.SendClientChat(Client, "§ECould not find a user with the name '" + args[0] + "'.");
                return;
            }

            Chat.SendGlobalChat(Client.ServerCore, "§SPlayer " + args[0] + "§S was unstopped by " + Client.CS.FormattedName + "§S.");

            Client.ServerCore.DB.UnstopPlayer(args[0]);

            foreach (NetworkClient c in Client.ServerCore.nh.Clients) {
                if (c.CS.LoginName.ToLower() == args[0].ToLower()) {
                    c.CS.Stopped = false;
                    break;
                }
            }
        }
        #endregion
    }
}
