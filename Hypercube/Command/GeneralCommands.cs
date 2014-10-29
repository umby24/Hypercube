using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hypercube.Core;
using Hypercube.Client;
using Hypercube.Map;

namespace Hypercube.Command {
    internal static class GeneralCommands {
        public static void Init(CommandHandler holder) {
            holder.AddCommand("/about", CAbout);
            holder.AddCommand("/bring", CBring);
            holder.AddCommand("/global", CGlobal);
            holder.AddCommand("/getrank", CGetrank);
            holder.AddCommand("/players", CPlayers);
            holder.AddCommand("/ranks", CRanks);
            holder.AddCommand("/rules", CRules);
            holder.AddCommand("/commands", CCommands);
            holder.AddCommand("/cmdhelp", CCmdHelp);
            holder.AddCommand("/maps", CMaps);
            holder.AddCommand("/map", CMap);
            holder.AddCommand("/tp", Ctp);
            holder.AddCommand("/info", CInfo);
            holder.AddCommand("/model", CModel);
        }

        #region About
        static readonly Command CAbout = new Command {
            Plugin = "",
            Group = "General",
            Help = "§SLists information about this server.",
            AllPerms = true,
            Console = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
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
        #region Bring
        static readonly Command CBring = new Command {
            Plugin = "",
            Group = "General",
            Help = "§SBrings another player to you.<br>§SUsage: /bring [player name]",
            AllPerms = true,
            Console = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
                {"command.bring", new Permission { Fullname = "command.bring", Group = "command", Perm = "bring"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
                {"command.bring", new Permission { Fullname = "command.bring", Group = "command", Perm = "bring"}},
            },

            Handler = BringHandler,
        };

        private static void BringHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length == 0 || args.Length > 1) {
                Chat.SendClientChat(client, "§EIncorrect number of arguments. See /cmdhelp bring");
                return;
            }

            NetworkClient tpClient;
            if (!ServerCore.Nh.LoggedClients.TryGetValue(args[0], out tpClient)) {
                Chat.SendClientChat(client, "§ECould not find a player called '" + args[0] + "'.");
                return;
            }

            if (tpClient.CS.CurrentMap != client.CS.CurrentMap)
                tpClient.ChangeMap(client.CS.CurrentMap);

            if (client.CS.CurrentMap != tpClient.CS.CurrentMap)
                return;

            tpClient.CS.MyEntity.Location = client.CS.MyEntity.Location;
            tpClient.CS.MyEntity.Rot = client.CS.MyEntity.Rot;
            tpClient.CS.MyEntity.Look = client.CS.MyEntity.Look;
            tpClient.CS.MyEntity.SendOwn = true;
            Chat.SendClientChat(client, "§STeleported by " + client.CS.FormattedName);
        }

        #endregion
        #region Commands
        static readonly Command CCommands = new Command {
            Plugin = "",
            Group = "General",
            Help = "§SLists all availiable commands.<br>§SUsage: /commands [group (optional)]",
            AllPerms = true,
            Console = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            Handler = CommandsHandler,
        };

        static void CommandsHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length == 0) { // -- List command groups
                Chat.SendClientChat(client, "§SCommand groups:");
                Chat.SendClientChat(client, "&a    All");

                foreach (var a in ServerCore.Commandholder.Groups.Keys)
                    Chat.SendClientChat(client, "&a    " + a);

            } else if (args.Length == 1) { // -- list a group.
                if (!ServerCore.Commandholder.Groups.ContainsKey(args[0]) && args[0].ToLower() != "all") {
                    Chat.SendClientChat(client, "§EGroup '" + args[0] + "' not found.");
                    return;
                }

                var commandString = "§D&f ";
                var currentLen = 5;

                if (args[0].ToLower() == "all") {
                    foreach (var b in ServerCore.Commandholder.CommandDict.Keys) {
                        if (!ServerCore.Commandholder.CommandDict[b].CanBeSeen(client))
                            continue;

                        if ((b.Substring(1, b.Length - 1) + " §D&f ").Length + currentLen >= 59) {
                            commandString += "<br>§D&f " + b.Substring(1, b.Length - 1) + " §D&f ";
                            currentLen = ("§D&f " + b.Substring(1, b.Length - 1) + " §D&f ").Length;
                        } else {
                            commandString += b.Substring(1, b.Length - 1) + " §D&f ";
                            currentLen += (b.Substring(1, b.Length - 1) + " §D&f ").Length;
                        }
                    }

                    Chat.SendClientChat(client, "&aAll Commands:<br>" + commandString);
                    return;
                }

                foreach (var b in ServerCore.Commandholder.Groups[args[0]]) {
                    if (!ServerCore.Commandholder.CommandDict["/" + b].CanBeSeen(client))
                        continue;

                    if ((b.Substring(1, b.Length - 1) + " §D&f ").Length + currentLen >= 59) {
                        commandString += "<br>§D&f " + b + " §D&f ";
                        currentLen = ("§D&f " + b + " §D&f ").Length;
                    } else {
                        commandString += b + " §D&f ";
                        currentLen += (b + " §D&f ").Length;
                    }
                }

                Chat.SendClientChat(client, "&aGroup " + args[0]);
                Chat.SendClientChat(client, commandString);
            } else {
                Chat.SendClientChat(client, "§EWrong number of arguments supplied. See /cmdhelp commands");
            }
        }

        #endregion
        #region CommandHelp
        static readonly Command CCmdHelp = new Command {
            Plugin = "",
            Group = "General",
            Help = "§SShows help for a command.<br>§SUsage: /cmdhelp [command]",
            AllPerms = true,
            Console = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            Handler = CmdhelpHandler,
        };

        static void CmdhelpHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length == 0) {
                Chat.SendClientChat(client, "§E&fUsage of this command: /cmdhelp [command].");
                return;
            }

            if (ServerCore.Commandholder.CommandDict.ContainsKey("/" + args[0].ToLower()) == false) {
                Chat.SendClientChat(client, "§E&fCommand not found.");
                return;
            }

            if (!ServerCore.Commandholder.CommandDict["/" + args[0]].CanBeSeen(client)) {
                Chat.SendClientChat(client, "§E&fCommand not found.");
                return;
            }

            var thisCommand = ServerCore.Commandholder.CommandDict["/" + args[0].ToLower()];
            Chat.SendClientChat(client, "§S/" + args[0]);
            Chat.SendClientChat(client, thisCommand.Help);
        }
        #endregion
        #region Getrank
        static readonly Command CGetrank = new Command {
            Plugin = "",
            Group = "General",
            Help = "§SGives the rank(s) of a player.<br>§SUsage: /getrank [Name]",
            Console = true,
            AllPerms = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            Handler = Getrankhandler,
        };

        static void Getrankhandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length != 1) {
                Chat.SendClientChat(client, "§EIncorrect usage. See /cmdhelp getrank.");
                return;
            }

            args[0] = ServerCore.DB.GetPlayerName(args[0]);

            if (args[0] == "") {
                Chat.SendClientChat(client, "§ECould not find the player.");
                return;
            }


            var playerRanks = RankContainer.SplitRanks(ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "Rank"));
            var playerSteps = RankContainer.SplitSteps(ServerCore.DB.GetDatabaseString(args[0], "PlayerDB", "RankStep"));
            var playerInfo = "§SRank(s) for " + args[0] + ": ";

            foreach (var r in playerRanks)
                playerInfo += r.Prefix + r.Name + r.Suffix + "(" + playerSteps[playerRanks.IndexOf(r)] + "), ";

            playerInfo = playerInfo.Substring(0, playerInfo.Length - 1); // -- Remove the final comma.
            playerInfo += "<br>";

            Chat.SendClientChat(client, playerInfo);
        }
        #endregion
        #region Global
        static readonly Command CGlobal = new Command {
            Plugin = "",
            Group = "General",
            Help = "§SAllows you to switch between chat modes.<br>§SUsage: /global (optional)[on/off]",
            AllPerms = true,
            Console = false,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            Handler = GlobalHandler,
        };

        static void GlobalHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length == 0) {
                // -- Toggle.
                if (ServerCore.DB.GetDatabaseInt(client.CS.LoginName, "PlayerDB", "Global") == 1) {
                    client.CS.Global = false;
                    Chat.SendClientChat(client, "§SGlobal chat is now off by default.");
                    ServerCore.DB.SetDatabase(client.CS.LoginName, "PlayerDB", "Global", "0");
                } else {
                    client.CS.Global = true;
                    Chat.SendClientChat(client, "§SGlobal chat is now on by default.");
                    ServerCore.DB.SetDatabase(client.CS.LoginName, "PlayerDB", "Global", "1");
                }
            } else if (args.Length == 1) {
                if (args[0].ToLower() == "on" || args[0].ToLower() == "true") {
                    client.CS.Global = true;
                    Chat.SendClientChat(client, "§SGlobal chat is now on by default.");
                    ServerCore.DB.SetDatabase(client.CS.LoginName, "PlayerDB", "Global", "1");
                } else if (args[0].ToLower() == "off" || args[0].ToLower() == "false") {
                    client.CS.Global = false;
                    Chat.SendClientChat(client, "§SGlobal chat is now off by default.");
                    ServerCore.DB.SetDatabase(client.CS.LoginName, "PlayerDB", "Global", "0");
                }
            } else
                Chat.SendClientChat(client, "§EIncorrect number of arguments, see /cmdhelp global.");
        }
        #endregion
        #region Players
        static readonly Command CPlayers = new Command {
            Plugin = "",
            Group = "General",
            Help = "§SDisplays a list of players on the server and the map they are on.",
            AllPerms = true,
            Console = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            Handler = PlayersHandler,
        };

        static void PlayersHandler(NetworkClient client, string[] args, string text1, string text2) {
            var onlineString = "§SOnline Players: " + ServerCore.Nh.Clients.Count + "<br>";

            foreach (var hm in ServerCore.Maps.Values) {
                onlineString += "§S" + hm.CWMap.MapName + "&f: ";

                foreach(var c in hm.ClientsList)
                    onlineString += c.CS.FormattedName + " §D ";

                onlineString += "<br>";
            }

            Chat.SendClientChat(client, onlineString);
        }
        #endregion
        #region Ranks
        static readonly Command CRanks = new Command {
            Plugin = "",
            Group = "General",
            Help = "§SShows a list of all possible ranks in the server.",
            AllPerms = true,
            Console = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            Handler = RanksHandler,
        };

        static void RanksHandler(NetworkClient client, string[] args, string text1, string text2) {
            Chat.SendClientChat(client, "§SGroups&f:");
            var groupDict = new Dictionary<string, string>();

            foreach (var r in ServerCore.Rankholder.NameList.Values) {
                if (groupDict.Keys.Contains(r.Group))
                    groupDict[r.Group] += "§S| " + r.Prefix + r.Name + r.Suffix + " ";
                else
                    groupDict.Add(r.Group, "§S" + r.Group + "&f: " + r.Prefix + r.Name + r.Suffix + " ");
            }

            foreach (var b in groupDict.Keys)
                Chat.SendClientChat(client, groupDict[b]);
        }
        #endregion
        #region Rules
        static readonly Command CRules = new Command {
            Plugin = "",
            Group = "General",
            Help = "§SShows the server rules.<br>§SUsage: /rules",
            AllPerms = true,
            Console = false,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
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
        static readonly Command CMaps = new Command {
            Plugin = "",
            Group = "General",
            Help = "§SGives a list of available maps.",
            Console = true,
            AllPerms = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            Handler = MapsHandler,
        };

        static void MapsHandler(NetworkClient client, string[] args, string text1, string text2) {
            var mapString = "§SMaps:<br>";

            foreach (var m in ServerCore.Maps.Values) {
                if (client.HasAllPermissions(m.Showperms.Values.ToList()))
                    mapString += "§S" + m.CWMap.MapName + " §D ";
            }

            Chat.SendClientChat(client, mapString);
        }
        #endregion
        #region Map
        static readonly Command CMap = new Command {
            Plugin = "",
            Group = "General",
            Help = "§STeleports you in the selected map.<br>§SUsage: /map [Name]",
            Console = false,
            AllPerms = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            Handler = MapHandler,
        };

        static void MapHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length == 0) {
                Chat.SendClientChat(client, "§EInvalid number of arguments. See /cmdhelp map");
                return;
            }

            HypercubeMap m;
            ServerCore.Maps.TryGetValue(args[0], out m);

            if (m != null) 
                client.ChangeMap(m);
             else
                Chat.SendClientChat(client, "§EMap '" + args[0] + "' not found.");
                
        }
        #endregion
        #region Info
        static readonly Command CInfo = new Command {
            Plugin = "",
            Group = "General",
            Help = "§SShows some information about this server<br>§SUsage: /info",
            AllPerms = true,
            Console = false,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            Handler = InfoHandler,
        };

        static void InfoHandler(NetworkClient client, string[] args, string text1, string text2) {
            Chat.SendClientChat(client, "§SServer Info:");
            Chat.SendClientChat(client, "§SThis server runs the &8Hypercube §SSoftware, by Umby24.");
            Chat.SendClientChat(client, "§SServer Version: " + Assembly.GetExecutingAssembly().GetName().Version + " on .NET " + Environment.Version + " (" + Environment.OSVersion + ")");
            Chat.SendClientChat(client, "§SUptime: " + (DateTime.UtcNow - ServerCore.Uptime).ToString("hh"));
        }
        #endregion
        #region TP
        static readonly Command Ctp = new Command {
            Plugin = "",
            Group = "General",
            Help = "§STeleports you to another player.<br>§SUsage: /tp [player name]",
            AllPerms = true,
            Console = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
                {"command.tp", new Permission { Fullname = "command.tp", Group = "command", Perm = "tp"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
                {"command.tp", new Permission { Fullname = "command.tp", Group = "command", Perm = "tp"}},
            },

            Handler = TPHandler,
        };

        private static void TPHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length == 0 || args.Length > 1) {
                Chat.SendClientChat(client, "§EIncorrect number of arguments. See /cmdhelp tp");
                return;
            }

            NetworkClient tpClient;
            if (!ServerCore.Nh.LoggedClients.TryGetValue(args[0], out tpClient)) {
                Chat.SendClientChat(client, "§ECould not find a player called '" + args[0] + "'.");
                return;
            }

            if (tpClient.CS.CurrentMap != client.CS.CurrentMap)
                client.ChangeMap(tpClient.CS.CurrentMap);

            if (client.CS.CurrentMap != tpClient.CS.CurrentMap)
                return;

            client.CS.MyEntity.Location = tpClient.CS.MyEntity.Location;
            client.CS.MyEntity.Rot = tpClient.CS.MyEntity.Rot;
            client.CS.MyEntity.Look = tpClient.CS.MyEntity.Look;
            client.CS.MyEntity.SendOwn = true;
            Chat.SendClientChat(client, "§STeleported.");
        }
        #endregion

        #region Model
        static readonly Command CModel = new Command {
            Plugin = "",
            Group = "General",
            Help = "§SChanges you into a different model!<br>§SUsage: /model [mob or block]<br>§SYou may insert a block ID, or one of the following types:<br>Chicken, Creeper, Croc, Printer, Zombie, Pig, Sheep, Skeleton, Spider",
            AllPerms = true,
            Console = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            Handler = ModelHandler,
        };

        private static void ModelHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length > 1) {
                Chat.SendClientChat(client, "§EInvalid number of arguments. See /cmdhelp model");
                return;
            }

            if (args.Length == 0) {
                client.CS.MyEntity.Model = "default";
                Chat.SendClientChat(client, "§SYour model has been reset.");
                return;
            }

            int block;

            if (int.TryParse(args[0], out block)) {
                if (block > 66 || block < 1) {
                    Chat.SendClientChat(client, "§EInvalid block number. Must be from 1-66.");
                    return;
                }

                client.CS.MyEntity.Model = block.ToString();
                Chat.SendClientChat(client, "§SModel changed.");
                return;
            }

            switch (args[0].ToLower()) {
                case "croc":
                    args[0] = "crocodile";
                    goto case "crocodile";
                case "skele":
                    args[0] = "skele";
                    goto case "skeleton";
                case "skeleton":
                case "spider":
                case "crocodile":
                case "pig":
                case "printer":
                case "chicken":
                case "creeper":
                case "sheep":
                case "zombie":
                    client.CS.MyEntity.Model = args[0].ToLower();
                    break;
                default:
                    client.CS.MyEntity.Model = "default";
                    break;
            }
            Chat.SendClientChat(client, "§SModel changed.");
        }

        #endregion
    }
}
