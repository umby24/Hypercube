﻿using System.Collections.Generic;
using System.Linq;

using Hypercube.Core;
using Hypercube.Client;

namespace Hypercube.Command {
    internal static class GeneralCommands {
        public static void Init(CommandHandler holder) {
            holder.AddCommand("/about", cAbout);
            holder.AddCommand("/global", cGlobal);
            holder.AddCommand("/getrank", cGetrank);
            holder.AddCommand("/players", cPlayers);
            holder.AddCommand("/ranks", cRanks);
            holder.AddCommand("/rules", cRules);
            holder.AddCommand("/commands", cCommands);
            holder.AddCommand("/cmdhelp", cCmdHelp);
            holder.AddCommand("/maps", cMaps);
            holder.AddCommand("/map", cMap);
        }

        #region About
        static readonly Command cAbout = new Command {
            Plugin = "",
            Group = "General",
            Help = "§SLists information about this server.",
            AllPerms = true,
            Console = true,

            UsePermissions = new List<Permission> {
                new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"},
            },

            ShowPermissions = new List<Permission> {
                new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"},
            },

            Handler = AboutHandler,
        };

        static void AboutHandler(NetworkClient client, string[] args, string text1, string text2) {
            Chat.SendClientChat(client, "§SServer Software:&f ServerCore");
            Chat.SendClientChat(client, "§SServer Developer:&f Umby24");
            Chat.SendClientChat(client, "§SServer Version:&f 0.0 ALPHA");
            Chat.SendClientChat(client, "§SThis server is written from scratch in C#<br>§Sand supports Lua scripting!");
        }
        #endregion
        #region Commands
        static readonly Command cCommands = new Command {
            Plugin = "",
            Group = "General",
            Help = "§SLists all availiable commands.<br>§SUsage: /commands [group (optional)]",
            AllPerms = true,
            Console = true,

            UsePermissions = new List<Permission> {
                new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"},
            },

            ShowPermissions = new List<Permission> {
                new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"},
            },

            Handler = CommandsHandler,
        };

        static void CommandsHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length == 0) { // -- List command groups
                Chat.SendClientChat(Client, "§SCommand groups:");
                Chat.SendClientChat(Client, "&a    All");

                foreach (var a in ServerCore.Commandholder.Groups.Keys)
                    Chat.SendClientChat(Client, "&a    " + a);

            } else if (args.Length == 1) { // -- list a group.
                if (!ServerCore.Commandholder.Groups.ContainsKey(args[0]) && args[0].ToLower() != "all") {
                    Chat.SendClientChat(Client, "§EGroup '" + args[0] + "' not found.");
                    return;
                }

                var commandString = "§D&f ";
                var currentLen = 5;

                if (args[0].ToLower() == "all") {
                    foreach (var b in ServerCore.Commandholder.CommandDict.Keys) {
                        if (!ServerCore.Commandholder.CommandDict[b].CanBeSeen(Client))
                            continue;

                        if ((b.Substring(1, b.Length - 1) + " §D&f ").Length + currentLen >= 59) {
                            commandString += "<br>§D&f " + b.Substring(1, b.Length - 1) + " §D&f ";
                            currentLen = ("§D&f " + b.Substring(1, b.Length - 1) + " §D&f ").Length;
                        } else {
                            commandString += b.Substring(1, b.Length - 1) + " §D&f ";
                            currentLen += (b.Substring(1, b.Length - 1) + " §D&f ").Length;
                        }
                    }

                    Chat.SendClientChat(Client, "&aAll Commands:<br>" + commandString);
                    return;
                }

                foreach (var b in ServerCore.Commandholder.Groups[args[0]]) {
                    if (!ServerCore.Commandholder.CommandDict["/" + b].CanBeSeen(Client))
                        continue;

                    if ((b.Substring(1, b.Length - 1) + " §D&f ").Length + currentLen >= 59) {
                        commandString += "<br>§D&f " + b + " §D&f ";
                        currentLen = ("§D&f " + b + " §D&f ").Length;
                    } else {
                        commandString += b + " §D&f ";
                        currentLen += (b + " §D&f ").Length;
                    }
                }

                Chat.SendClientChat(Client, "&aGroup " + args[0]);
                Chat.SendClientChat(Client, commandString);
            } else {
                Chat.SendClientChat(Client, "§EWrong number of arguments supplied. See /cmdhelp commands");
                return;
            }
        }

        #endregion
        #region CommandHelp
        static readonly Command cCmdHelp = new Command {
            Plugin = "",
            Group = "General",
            Help = "§SShows help for a command.<br>§SUsage: /cmdhelp [command]",
            AllPerms = true,
            Console = true,

            UsePermissions = new List<Permission> {
                new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"},
            },

            ShowPermissions = new List<Permission> {
                new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"},
            },

            Handler = CmdhelpHandler,
        };

        static void CmdhelpHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length == 0) {
                Chat.SendClientChat(Client, "§E&fUsage of this command: /cmdhelp [command].");
                return;
            }

            if (ServerCore.Commandholder.CommandDict.ContainsKey("/" + args[0].ToLower()) == false) {
                Chat.SendClientChat(Client, "§E&fCommand not found.");
                return;
            }

            if (!ServerCore.Commandholder.CommandDict["/" + args[0]].CanBeSeen(Client)) {
                Chat.SendClientChat(Client, "§E&fCommand not found.");
                return;
            }

            var thisCommand = ServerCore.Commandholder.CommandDict["/" + args[0].ToLower()];
            Chat.SendClientChat(Client, "§S/" + args[0]);
            Chat.SendClientChat(Client, thisCommand.Help);
        }
        #endregion
        #region Getrank
        static readonly Command cGetrank = new Command {
            Plugin = "",
            Group = "General",
            Help = "§SGives the rank(s) of a player.<br>§SUsage: /getrank [Name]",
            Console = true,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"},
            },

            ShowPermissions = new List<Permission> {
                new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"},
            },

            Handler = Getrankhandler,
        };

        static void Getrankhandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length != 1) {
                Chat.SendClientChat(Client, "§EIncorrect usage. See /cmdhelp getrank.");
                return;
            }

            args[0] = ServerCore.DB.GetPlayerName(args[0]);

            if (args[0] == "") {
                Chat.SendClientChat(Client, "§ECould not find the player.");
                return;
            }


            var playerRanks = RankContainer.SplitRanks(ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "Rank"));
            var playerSteps = RankContainer.SplitSteps(ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "RankStep"));
            var playerInfo = "§SRank(s) for " + args[0] + ": ";

            foreach (var r in playerRanks)
                playerInfo += r.Prefix + r.Name + r.Suffix + "(" + playerSteps[playerRanks.IndexOf(r)] + "), ";

            playerInfo = playerInfo.Substring(0, playerInfo.Length - 1); // -- Remove the final comma.
            playerInfo += "<br>";

            Chat.SendClientChat(Client, playerInfo);
        }
        #endregion
        #region Global
        static readonly Command cGlobal = new Command {
            Plugin = "",
            Group = "General",
            Help = "§SAllows you to switch between chat modes.<br>§SUsage: /global (optional)[on/off]",
            AllPerms = true,
            Console = false,

            UsePermissions = new List<Permission> {
                new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"},
            },

            ShowPermissions = new List<Permission> {
                new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"},
            },

            Handler = GlobalHandler,
        };

        static void GlobalHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length == 0) {
                // -- Toggle.
                if (ServerCore.DB.GetDatabaseInt(Client.CS.LoginName, "PlayerDB", "Global") == 1) {
                    Client.CS.Global = false;
                    Chat.SendClientChat(Client, "§SGlobal chat is now off by default.");
                    ServerCore.DB.SetDatabase(Client.CS.LoginName, "PlayerDB", "Global", "0");
                } else {
                    Client.CS.Global = true;
                    Chat.SendClientChat(Client, "§SGlobal chat is now on by default.");
                    ServerCore.DB.SetDatabase(Client.CS.LoginName, "PlayerDB", "Global", "1");
                }
            } else if (args.Length == 1) {
                if (args[0].ToLower() == "on" || args[0].ToLower() == "true") {
                    Client.CS.Global = true;
                    Chat.SendClientChat(Client, "§SGlobal chat is now on by default.");
                    ServerCore.DB.SetDatabase(Client.CS.LoginName, "PlayerDB", "Global", "1");
                } else if (args[0].ToLower() == "off" || args[0].ToLower() == "false") {
                    Client.CS.Global = false;
                    Chat.SendClientChat(Client, "§SGlobal chat is now off by default.");
                    ServerCore.DB.SetDatabase(Client.CS.LoginName, "PlayerDB", "Global", "0");
                }
            } else
                Chat.SendClientChat(Client, "§EIncorrect number of arguments, see /cmdhelp global.");
        }
        #endregion
        #region Players
        static readonly Command cPlayers = new Command {
            Plugin = "",
            Group = "General",
            Help = "§SDisplays a list of players on the server and the map they are on.",
            AllPerms = true,
            Console = true,

            UsePermissions = new List<Permission> {
                new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"},
            },

            ShowPermissions = new List<Permission> {
                new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"},
            },

            Handler = PlayersHandler,
        };

        static void PlayersHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            var onlineString = "§SOnline Players: " + ServerCore.Nh.Clients.Count + "<br>";

            foreach (var hm in ServerCore.Maps) {
                onlineString += "§S" + hm.CWMap.MapName + "&f: ";

                foreach(var c in hm.ClientsList)
                    onlineString += c.CS.FormattedName + "§D";

                onlineString += "<br>";
            }

            Chat.SendClientChat(Client, onlineString);
        }
        #endregion
        #region Ranks
        static readonly Command cRanks = new Command {
            Plugin = "",
            Group = "General",
            Help = "§SShows a list of all possible ranks in the server.",
            AllPerms = true,
            Console = true,

            UsePermissions = new List<Permission> {
                new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"},
            },

            ShowPermissions = new List<Permission> {
                new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"},
            },

            Handler = RanksHandler,
        };

        static void RanksHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            Chat.SendClientChat(Client, "§SGroups&f:");
            var groupDict = new Dictionary<string, string>();

            foreach (var r in ServerCore.Rankholder.NameList.Values) {
                if (groupDict.Keys.Contains(r.Group))
                    groupDict[r.Group] += "§S| " + r.Prefix + r.Name + r.Suffix + " ";
                else
                    groupDict.Add(r.Group, "§S" + r.Group + "&f: " + r.Prefix + r.Name + r.Suffix + " ");
            }

            foreach (var b in groupDict.Keys)
                Chat.SendClientChat(Client, groupDict[b]);
        }
        #endregion
        #region Rules
        static readonly Command cRules = new Command {
            Plugin = "",
            Group = "General",
            Help = "§SShows the server rules.<br>§SUsage: /rules",
            AllPerms = true,
            Console = false,

            UsePermissions = new List<Permission> {
                new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"},
            },

            ShowPermissions = new List<Permission> {
                new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"},
            },

            Handler = RulesHandler,
        };

        static void RulesHandler(NetworkClient client, string[] args, string text1, string text2) {
            Chat.SendClientChat(client, "&6Server Rules:");

            for (var i = 0; i < ServerCore.Rules.Count; i++)
                Chat.SendClientChat(client, "&6" + (i + 1) + ": " + ServerCore.Rules[i]);
        }
        #endregion
        #region Maps
        static readonly Command cMaps = new Command {
            Plugin = "",
            Group = "General",
            Help = "§SGives a list of available maps.",
            Console = true,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"},
            },

            ShowPermissions = new List<Permission> {
                new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"},
            },

            Handler = MapsHandler,
        };

        static void MapsHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            var MapString = "§SMaps:<br>";

            foreach (var m in ServerCore.Maps) {
                var Cansee = false;

                foreach (var r in Client.CS.PlayerRanks) {
                    if (PermissionContainer.RankMatchesPermissions(r, m.Showperms.Values.ToList(), true)) {
                        Cansee = true;
                        break;
                    }
                }

                if (Cansee) 
                    MapString += "§S" + m.CWMap.MapName + " §D ";
            }

            Chat.SendClientChat(Client, MapString);
        }
        #endregion
        #region Map
        static readonly Command cMap = new Command {
            Plugin = "",
            Group = "General",
            Help = "§STeleports you in the selected map.<br>§SUsage: /map [Name]",
            Console = false,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"},
            },

            ShowPermissions = new List<Permission> {
                new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"},
            },

            Handler = MapHandler,
        };

        static void MapHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length == 0) {
                Chat.SendClientChat(Client, "§EInvalid number of arguments. See /cmdhelp map");
                return;
            }

            var found = false;

            foreach (var m in ServerCore.Maps) {
                if (m.CWMap.MapName.ToLower() == args[0].ToLower()) {
                    var canSee = false;
                    var canJoin = false;

                    foreach (var r in Client.CS.PlayerRanks) {
                        if (PermissionContainer.RankMatchesPermissions(r, m.Showperms.Values.ToList(), true))
                            canSee = true;

                        if (PermissionContainer.RankMatchesPermissions(r, m.Joinperms.Values.ToList(), true)) {
                            canJoin = true;
                            break;
                        }
                    }

                    if (canJoin) {
                        found = true;
                        Client.ChangeMap(m);
                    } else {
                        if (canSee) {
                            Chat.SendClientChat(Client, "§EYou are not allowed to join this map.");
                            return;
                        } else {
                            Chat.SendClientChat(Client, "§EMap '" + args[0] + "' not found.");
                            return;
                        }
                    }
                }
            }

            if (!found)
                Chat.SendClientChat(Client, "§EMap '" + args[0] + "' not found.");
        }
        #endregion
    }
}
