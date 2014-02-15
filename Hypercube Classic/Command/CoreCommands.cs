using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hypercube_Classic.Client;
using Hypercube_Classic.Core;

namespace Hypercube_Classic.Command {

    public struct BanCommand : Command {
        public string Command { get { return "/ban"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "Op"; } }
        public string Help { get { return "&eBans a player.<br>&eUsage: /Ban [Name] <Reason>"; } }

        public string ShowRanks { get { return "2"; } }
        public string UseRanks { get { return "2"; } }

        public void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            if (args.Length == 0)
                return;

            if (!Core.Database.ContainsPlayer(args[0])) {
                Chat.SendClientChat(Client, "&4Error:&f Could not find a user with the name '" + args[0] + "'.");
                return;
            }

            string BanReason;

            if (args.Length == 1)
                BanReason = "Banned by staff.";
            else
                BanReason = Text2;

            Core.Logger._Log("Info", "Command", "Player " + args[0] + " was banned by " + Client.CS.LoginName + ". (" + BanReason + ")");
            Chat.SendGlobalChat(Core, "&ePlayer " + args[0] + "&e was banned by " + Client.CS.FormattedName + "&e. (&f" + BanReason + "&e)");

            var Values = new Dictionary<string, string>();
            Values.Add("Banned", "1");
            Values.Add("BanMessage", BanReason);
            Values.Add("BannedBy", Client.CS.FormattedName);
            Core.Database.Update("PlayerDB", Values, "Name='" + args[0] + "'");

            foreach (NetworkClient c in Core.nh.Clients) {
                if (c.CS.LoginName.ToLower() == args[0].ToLower() && c.CS.LoggedIn) {
                    c.KickPlayer("&eBanned: " + BanReason);
                    break;
                }
            }


        }
    }
    public struct KickCommand : Command {
        public string Command { get { return "/kick"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "Op"; } }
        public string Help { get { return "&eKicks a player.<br>&eUsage: /kick [Name] <Reason>"; } }

        public string ShowRanks { get { return "2"; } }
        public string UseRanks { get { return "2"; } }

        public void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            if (args.Length == 0)
                return;
            
            bool kicked = false;
            string KickReason;

            if (args.Length == 1)
                KickReason = "Kicked by staff.";
            else
                KickReason = Text2;

            foreach (NetworkClient c in Core.nh.Clients) {
                if (c.CS.LoginName.ToLower() == args[0].ToLower() && c.CS.LoggedIn) {
                    Core.Logger._Log("Info", "Command", "Player " + c.CS.LoginName + " was kicked by " + Client.CS.LoginName + ". (" + KickReason + ")");
                    Chat.SendGlobalChat(Core, "&ePlayer " + c.CS.FormattedName + "&e was kicked by " + Client.CS.FormattedName + "&e. (&f" + KickReason + "&e)");

                    c.KickPlayer("&e" + KickReason);

                    kicked = true;
                    break;
                }
            }

            if (!kicked)
                Chat.SendClientChat(Client, "&4Error:&f Could not find a user with the name '" + args[0] + "'.");
        }
    }
    public struct MuteCommand : Command {
        public string Command { get { return "/mute"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "Op"; } }
        public string Help { get { return "&eMutes a player, he can't speak now.<br>&eUsage: /mute [Name] <minutes>"; } }

        public string ShowRanks { get { return "2"; } }
        public string UseRanks { get { return "2"; } }

        public void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            if (args.Length == 0)
                return;

            if (!Core.Database.ContainsPlayer(args[0])) {
                Chat.SendClientChat(Client, "&4Error:&f Could not find a user with the name '" + args[0] + "'.");
                return;
            }

            int MuteDuration;
            string muteReason = "You have been muted";

            if (args.Length == 1)
                MuteDuration = 999999;
            else if (args.Length == 2)
                MuteDuration = int.Parse(Text2);
            else {
                MuteDuration = int.Parse(args[1]);
                muteReason = Text2.Substring(Text2.IndexOf(" ") + 1, Text2.Length - (Text2.IndexOf(" ") + 1));
            }

            Core.Logger._Log("Info", "Command", "Player " + args[0] + " was muted for " + MuteDuration.ToString() + " Minutes. (" + muteReason + ")");
            Chat.SendGlobalChat(Core, "&ePlayer " + args[0] + "&e was muted for " + MuteDuration.ToString() + " minutes. (&f" + muteReason + "&e)");

            var Values = new Dictionary<string, string>();
            Values.Add("MuteMessage", muteReason);
            Values.Add("Time_Muted", MuteDuration.ToString());
            Core.Database.Update("PlayerDB", Values, "Name='" + args[0] + "'");

            foreach (NetworkClient c in Core.nh.Clients) {
                if (c.CS.LoginName.ToLower() == args[0].ToLower() && c.CS.LoggedIn) {
                    c.CS.MuteTime = MuteDuration;
                    break;
                }
            }
        }
    }
    public struct RanksCommand : Command {
        public string Command { get { return "/ranks"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "General"; } }
        public string Help { get { return "&3Shows a list of all possible ranks in the server."; } }

        public string ShowRanks { get { return "1,2"; } }
        public string UseRanks { get { return "1,2"; } }

        public void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            Chat.SendClientChat(Client, "&eGroups&f:");
            var GroupDict = new Dictionary<string, string>();

            foreach (Rank r in Core.Rankholder.Ranks) {
                if (GroupDict.Keys.Contains(r.Group))
                    GroupDict[r.Group] += "&e| " + r.Prefix + r.Name + r.Suffix + " ";
                else
                    GroupDict.Add(r.Group, "&e" + r.Group + "&f: " + r.Prefix + r.Name + r.Suffix + " ");
            }

            foreach (string b in GroupDict.Keys)
                Chat.SendClientChat(Client, GroupDict[b]);

        }
    }
    public struct StopCommand : Command {
        public string Command { get { return "/stop"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "Op"; } }
        public string Help { get { return "&eStops a player, he can't built now.<br>&eUsage: /Stop [Name] <Reason>"; } }

        public string ShowRanks { get { return "2"; } }
        public string UseRanks { get { return "2"; } }

        public void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            if (args.Length == 0)
                return;

            if (!Core.Database.ContainsPlayer(args[0])) {
                Chat.SendClientChat(Client, "&4Error:&f Could not find a user with the name '" + args[0] + "'.");
                return;
            }

            string StopReason = "You have been stopped.";

            if (args.Length > 1) {
                StopReason = Text2;
            }

            Core.Logger._Log("Info", "Command", "Player " + args[0] + " was stopped by " + Client.CS.LoginName + ". (" + StopReason + ")");
            Chat.SendGlobalChat(Core, "&ePlayer " + args[0] + "&e was stopped by " + Client.CS.FormattedName + "&e. (&f" + StopReason + "&e)");

            var Values = new Dictionary<string, string>();
            Values.Add("Stopped", "1");
            Values.Add("StoppedBy", Client.CS.FormattedName);
            Values.Add("StopMessage", StopReason);
            Core.Database.Update("PlayerDB", Values, "Name='" + args[0] + "'");

            foreach (NetworkClient c in Core.nh.Clients) {
                if (c.CS.LoginName.ToLower() == args[0].ToLower()) {
                    c.CS.Stopped = true;
                    break;
                }
            }

        }
    }
    public struct UnbanCommand : Command {
        public string Command { get { return "/unban"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "Op"; } }
        public string Help { get { return "&eUnbans a player.<br>&eUsage: /Unban [Name]"; } }

        public string ShowRanks { get { return "2"; } }
        public string UseRanks { get { return "2"; } }

        public void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            if (args.Length == 0)
                return;

            if (!Core.Database.ContainsPlayer(args[0])) {
                Chat.SendClientChat(Client, "&4Error:&f Could not find a user with the name '" + args[0] + "'.");
                return;
            }

            Core.Logger._Log("Info", "Command", "Player " + args[0] + " was unbanned by " + Client.CS.LoginName + ".");
            Chat.SendGlobalChat(Core, "&ePlayer " + args[0] + "&e was unbanned by " + Client.CS.FormattedName + "&e.");

            var Values = new Dictionary<string, string>();
            Values.Add("Banned", "0");
            Core.Database.Update("PlayerDB", Values, "Name='" + args[0] + "'");
        }
    }
    public struct UnmuteCommand : Command {
        public string Command { get { return "/unmute"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "Op"; } }
        public string Help { get { return "&eUnmutes a player, he can speak now.<br>&eUsage: /mute [Name]"; } }

        public string ShowRanks { get { return "2"; } }
        public string UseRanks { get { return "2"; } }

        public void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            if (args.Length == 0)
                return;

            if (!Core.Database.ContainsPlayer(args[0])) {
                Chat.SendClientChat(Client, "&4Error:&f Could not find a user with the name '" + args[0] + "'.");
                return;
            }

            Core.Logger._Log("Info", "Command", "Player " + args[0] + " was unmuted.");
            Chat.SendGlobalChat(Core, "&ePlayer " + args[0] + "&e was unmuted.");

            var Values = new Dictionary<string, string>();
            Values.Add("Time_Muted", "0");
            Core.Database.Update("PlayerDB", Values, "Name='" + args[0] + "'");

            foreach (NetworkClient c in Core.nh.Clients) {
                if (c.CS.LoginName.ToLower() == args[0].ToLower() && c.CS.LoggedIn) {
                    c.CS.MuteTime = 0;
                    break;
                }
            }
        }
    }
    public struct UnstopCommand : Command {
        public string Command { get { return "/unstop"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "Op"; } }
        public string Help { get { return "&eUnstops a player, he can build now.<br>&eUsage: /Unstop [Name]"; } }

        public string ShowRanks { get { return "2"; } }
        public string UseRanks { get { return "2"; } }

        public void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            if (args.Length == 0)
                return;

            if (!Core.Database.ContainsPlayer(args[0])) {
                Chat.SendClientChat(Client, "&4Error:&f Could not find a user with the name '" + args[0] + "'.");
                return;
            }

            Core.Logger._Log("Info", "Command", "Player " + args[0] + " was unstopped by " + Client.CS.LoginName + ".");
            Chat.SendGlobalChat(Core, "&ePlayer " + args[0] + "&e was unstopped by " + Client.CS.FormattedName + "&e.");

            var Values = new Dictionary<string, string>();
            Values.Add("Stopped", "0");
            Core.Database.Update("PlayerDB", Values, "Name='" + args[0] + "'");

            foreach (NetworkClient c in Core.nh.Clients) {
                if (c.CS.LoginName.ToLower() == args[0].ToLower()) {
                    c.CS.Stopped = false;
                    break;
                }
            }

        }
    }
}
