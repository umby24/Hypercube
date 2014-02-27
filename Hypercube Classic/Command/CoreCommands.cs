using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hypercube_Classic.Client;
using Hypercube_Classic.Core;
using Hypercube_Classic.Map;

namespace Hypercube_Classic.Command {

    public struct BanCommand : Command {
        public string Command { get { return "/ban"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "General"; } }
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

            Core.Database.BanPlayer(args[0], BanReason, Client.CS.LoginName);

            foreach (NetworkClient c in Core.nh.Clients) {
                if (c.CS.LoginName.ToLower() == args[0].ToLower() && c.CS.LoggedIn) {
                    c.KickPlayer("&eBanned: " + BanReason);
                    break;
                }
            }


        }
    }
    public struct BindCommand : Command {
        public string Command { get { return "/bind"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "Build"; } }
        public string Help { get { return "&eChanges the block you have bound for using /material.<br>&eUsage: /bind [material] [build material]<br>&eEx. /bind Stone, or /bind Stone Fire"; } }

        public string ShowRanks { get { return "1,2"; } }
        public string UseRanks { get { return "1,2"; } }

        public void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            if (args.Length == 0) {
                Chat.SendClientChat(Client, "&eYour currently bound block is &f" + Client.CS.MyEntity.Boundblock + "&e.");
                Chat.SendClientChat(Client, "&eLooking for fCraft style bind? See /cmdhelp bind and /cmdhelp material.");
                return;
            } else if (args.Length == 1) {
                // -- Change the Bound block only.
                var newBlock = Core.Blockholder.GetBlock(args[0]);

                if (newBlock == null) {
                    Chat.SendClientChat(Client, "&4Error: &fCouldn't find a block called '" + args[0] + "'.");
                    return;
                }

                Client.CS.MyEntity.Boundblock = newBlock;
                Core.Database.SetDatabase(Client.CS.LoginName, "PlayerDB", "BoundBlock", newBlock.ID);
                Chat.SendClientChat(Client, "&eYour bound block is now " + newBlock.Name);

            } else if (args.Length == 2) {
                // -- Change the bound block and the current build material.
                var newBlock = Core.Blockholder.GetBlock(args[0]);

                if (newBlock == null) {
                    Chat.SendClientChat(Client, "&4Error: &fCouldn't find a block called '" + args[0] + "'.");
                    return;
                }

                var materialBlock = Core.Blockholder.GetBlock(args[1]);

                if (materialBlock == null) {
                    Chat.SendClientChat(Client, "&4Error: &fCouldn't find a block called '" + args[1] + "'.");
                    return;
                }

                Client.CS.MyEntity.Boundblock = newBlock;
                Core.Database.SetDatabase(Client.CS.LoginName, "PlayerDB", "BoundBlock", newBlock.ID);
                Chat.SendClientChat(Client, "&eYour bound block is now " + newBlock.Name);

                Client.CS.MyEntity.BuildMaterial = materialBlock;
                Chat.SendClientChat(Client, "&eYour build material is now " + materialBlock.Name);
            }
        }
    }
    public struct GetRankCommand : Command {
        public string Command { get { return "/getrank"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "Op"; } }
        public string Help { get { return "&eGives the rank of a player.<br>&eUsage: /getrank [Name]"; } }

        public string ShowRanks { get { return "1,2"; } }
        public string UseRanks { get { return "1,2"; } }

        public void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            if (args.Length == 0)
                return;

            args[0] = Core.Database.GetPlayerName(args[0]);

            if (args[0] == "") {
                Chat.SendClientChat(Client, "&4Error:&f Could not find player.");
                return;
            }

            var newRank = Core.Rankholder.GetRank(Core.Database.GetDatabaseInt(args[0], "PlayerDB", "Rank"));

            Chat.SendClientChat(Client, "&ePlayer " + newRank.Prefix + args[0] + newRank.Suffix + " is rank " + newRank.Prefix + newRank.Name + newRank.Suffix + "&e.");
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
    public struct MapCommand : Command {
        public string Command { get { return "/map"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "General"; } }
        public string Help { get { return "&eTeleports you in the selected map.<br>&eUsage: /map [Name]"; } }

        public string ShowRanks { get { return "1,2"; } }
        public string UseRanks { get { return "1,2"; } }

        public void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            foreach (HypercubeMap m in Core.Maps) {
                if (m.Map.MapName.ToLower() == args[0].ToLower()) {
                    if (m.JoinRanks.Contains(Client.CS.PlayerRank)) {
                        //TODO: Add vanish
                        Chat.SendMapChat(m, Core, "&ePlayer " + Client.CS.FormattedName + " &echanged to map &f" + m.Map.MapName + ".");
                        Chat.SendMapChat(Client.CS.CurrentMap, Core, "&ePlayer " + Client.CS.FormattedName + " &echanged to map &f" + m.Map.MapName + ".");

                        Client.CS.CurrentMap.Clients.Remove(Client);
                        Client.CS.CurrentMap.DeleteEntity(ref Client.CS.MyEntity);

                        Client.CS.CurrentMap = m;
                        m.SendMap(Client);
                        m.Clients.Add(Client);

                        Client.CS.MyEntity.X = (short)(m.Map.SpawnX * 32);
                        Client.CS.MyEntity.Y = (short)(m.Map.SpawnZ * 32);
                        Client.CS.MyEntity.Z = (short)((m.Map.SpawnY * 32) + 51);
                        Client.CS.MyEntity.Rot = m.Map.SpawnRotation;
                        Client.CS.MyEntity.Look = m.Map.SpawnLook;
                        Client.CS.MyEntity.Map = m;

                        m.SpawnEntity(Client.CS.MyEntity);
                        m.Entities.Add(Client.CS.MyEntity);
                        m.SendAllEntities(Client);
                        break;
                    } else {
                        Chat.SendClientChat(Client, "&4Error: &fYou are not allowed to join this map.");
                        return;
                    }
                }
            }
        }
    }
    public struct MapsCommand : Command {
        public string Command { get { return "/maps"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "General"; } }
        public string Help { get { return "&eGives a list of available maps."; } }

        public string ShowRanks { get { return "1,2"; } }
        public string UseRanks { get { return "1,2"; } }

        public void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            string MapString = "&eMaps:<br>";

            foreach (HypercubeMap m in Core.Maps) {
                if (m.ShowRanks.Contains(Client.CS.PlayerRank)) {
                    MapString += "&e" + m.Map.MapName + " &f| ";
                }
            }

            Chat.SendClientChat(Client, MapString);
        }
    }
    public struct MapInfoCommand : Command {
        public string Command { get { return "/mapinfo"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "Map"; } }
        public string Help { get { return "&eGives some information about a map."; } }

        public string ShowRanks { get { return "1,2"; } }
        public string UseRanks { get { return "1,2"; } }

        public void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            Chat.SendClientChat(Client, "&eMap Name: &f" + Client.CS.CurrentMap.Map.MapName);
            Chat.SendClientChat(Client, "&eSize: &f:" + Client.CS.CurrentMap.Map.SizeX.ToString() + " x " + Client.CS.CurrentMap.Map.SizeZ.ToString() + " x " + Client.CS.CurrentMap.Map.SizeY.ToString());
            Chat.SendClientChat(Client, "&eMemory Usage (Rough): &f" + ((Client.CS.CurrentMap.Map.SizeX * Client.CS.CurrentMap.Map.SizeY * Client.CS.CurrentMap.Map.SizeZ) / 2048).ToString() + " MB");
            Chat.SendClientChat(Client, "&eBuild Ranks:");
            Chat.SendClientChat(Client, "&eJoin Ranks:");
            Chat.SendClientChat(Client, "&eShow Ranks:");
            Chat.SendClientChat(Client, "&ePhysics Enabled: &f" + Client.CS.CurrentMap.HCSettings.Physics.ToString());
            Chat.SendClientChat(Client, "&eBuilding Enabled: &f" + Client.CS.CurrentMap.HCSettings.Building.ToString());
            Chat.SendClientChat(Client, "&eMapHistory Enabled: &f" + Client.CS.CurrentMap.HCSettings.History.ToString());
            Chat.SendClientChat(Client, "&eBlocksend-Queue: &f" + Client.CS.CurrentMap.BlockchangeQueue.Count.ToString());
            Chat.SendClientChat(Client, "&ePhysics-Queue: &f" + Client.CS.CurrentMap.PhysicsQueue.Count.ToString());

        }
    }
    public struct MapLoadCommand : Command {
        public string Command { get { return "/mapload"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "Map"; } }
        public string Help { get { return "&eLoads the map you are in.<br>&eUsage: /mapload <Name>"; } }

        public string ShowRanks { get { return "2"; } }
        public string UseRanks { get { return "2"; } }

        public void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            if (args.Length == 0) {
                Chat.SendClientChat(Client, "&4Error:&f This command requires 1 argument.<br>See /cmdhelp mapload.");
                return;
            }

            Client.CS.CurrentMap.LoadNewFile(args[0]);
            Client.CS.CurrentMap.ResendMap();
        }
    }
    public struct MapSaveCommand : Command {
        public string Command { get { return "/mapsave"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "Map"; } }
        public string Help { get { return "&eSaves the map you are in.<br>&eUsage: /mapsave <Name><br>&eName is not needed."; } }

        public string ShowRanks { get { return "2"; } }
        public string UseRanks { get { return "2"; } }

        public void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            if (args.Length == 0) 
                Client.CS.CurrentMap.SaveMap();
             else 
                Client.CS.CurrentMap.SaveMap(args[0]);
            
            Chat.SendClientChat(Client, "&eMap saved.");
        }
    }
    public struct MaterialCommand : Command {
        public string Command { get { return "/material"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "Build"; } }
        public string Help { get { return "&eChanges your building material. Build it with your bound block.<br>&eYou get a list of materials with /materials<br>&eUsage: /material [material]"; } }

        public string ShowRanks { get { return "1,2"; } }
        public string UseRanks { get { return "1,2"; } }

        public void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            if (args.Length == 0) {
                Chat.SendClientChat(Client, "&eYour build material has been reset.");
                Client.CS.MyEntity.BuildMaterial = Core.Blockholder.GetBlock("");
                return;
            }

            var newBlock = Core.Blockholder.GetBlock(args[0]);

            if (newBlock == null) {
                Chat.SendClientChat(Client, "&4Error: &fCouldn't find a block called '" + args[0] + "'.");
                return;
            }

            Client.CS.MyEntity.BuildMaterial = newBlock;
            Chat.SendClientChat(Client, "&eYour build material is now " + newBlock.Name);
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

            var MutedUntil = DateTime.UtcNow.AddMinutes((double)MuteDuration) - Hypercube.UnixEpoch;
            
            Core.Database.MutePlayer(args[0], (int)MutedUntil.TotalSeconds, muteReason);

            foreach (NetworkClient c in Core.nh.Clients) {
                if (c.CS.LoginName.ToLower() == args[0].ToLower() && c.CS.LoggedIn) {
                    c.CS.MuteTime = (int)MutedUntil.TotalSeconds;
                    break;
                }
            }
        }
    }
    public struct PinfoCommand : Command {
        public string Command { get { return "/pinfo"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "Op"; } }
        public string Help { get { return "&eGives some information about a player.<br>&eUsage: /pinfo [Name]"; } }

        public string ShowRanks { get { return "2"; } }
        public string UseRanks { get { return "2"; } }

        public void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            if (args.Length == 0) {
                Chat.SendClientChat(Client, "&4Error:&f Usage: /pinfo [name]");
                return;
            }

            args[0] = Core.Database.GetPlayerName(args[0]);

            if (args[0] == "") {
                Chat.SendClientChat(Client, "&4Error:&f Could not find player.");
                return;
            }

            string PlayerInfo = "&ePlayerinfo:<br>";

            var dt = Core.Database.GetDataTable("SELECT * FROM PlayerDB WHERE Name='" + args[0] + "' LIMIT 1");
            PlayerInfo += "&eNumber: " + Core.Database.GetDatabaseInt(args[0],"PlayerDB", "Number").ToString() + "<br>";
            PlayerInfo += "&eName: " + args[0] + "<br>";
            PlayerInfo += "&eRank: " + Core.Rankholder.GetRank(Core.Database.GetDatabaseInt(args[0], "PlayerDB", "Rank")).Name + "<br>";
            PlayerInfo += "&eIP: " + Core.Database.GetDatabaseString(args[0], "PlayerDB", "IP") + "<br>";
            PlayerInfo += "&eLogins: " + Core.Database.GetDatabaseInt(args[0], "PlayerDB", "LoginCounter").ToString() + "<br>";
            PlayerInfo += "&eKicks: " + Core.Database.GetDatabaseInt(args[0], "PlayerDB", "KickCounter").ToString() + ": " + Core.Database.GetDatabaseString(args[0], "PlayerDB", "KickMessage") + "<br>";

            if (Core.Database.GetDatabaseInt(args[0], "PlayerDB","Banned") > 0) 
                PlayerInfo += "&eBanned: " + Core.Database.GetDatabaseString(args[0], "PlayerDB","BanMessage") + " (" + Core.Database.GetDatabaseString(args[0], "PlayerDB","BannedBy") + ")<br>";
            
            if (Core.Database.GetDatabaseInt(args[0], "PlayerDB","Stopped") > 0)
                PlayerInfo += "&eStopped: " + Core.Database.GetDatabaseString(args[0], "PlayerDB", "StopMessage") + " (" + Core.Database.GetDatabaseString(args[0], "PlayerDB","StoppedBy") + ")<br>";

            if (Core.Database.GetDatabaseInt(args[0], "PlayerDB", "Time_Muted") > Hypercube.GetCurrentUnixTime())
                PlayerInfo += "&eMuted: "+ Core.Database.GetDatabaseString(args[0], "PlayerDB", "MuteMessage") + "<br>";

            Chat.SendClientChat(Client, PlayerInfo);
        }
    }
    public struct PlaceCommand : Command {
        public string Command { get { return "/place"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "Build"; } }
        public string Help { get { return "&ePlaces a block under you. The material is your last built<br>&eUsage: /place <material>"; } }

        public string ShowRanks { get { return "1,2"; } }
        public string UseRanks { get { return "1,2"; } }

        public void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            if (args.Length == 0) {
                Client.CS.CurrentMap.ClientChangeBlock(Client, (short)(Client.CS.MyEntity.X / 32), (short)(Client.CS.MyEntity.Y / 32), (short)((Client.CS.MyEntity.Z / 32) - 1), 1, Client.CS.MyEntity.Lastmaterial.OnClient);
                Chat.SendClientChat(Client, "&eBlock placed.");
            } else if (args.Length == 1) {
                var newBlock = Core.Blockholder.GetBlock(args[0]);

                if (newBlock == null) {
                    Chat.SendClientChat(Client, "&4Error: &fCouldn't find a block called '" + args[0] + "'.");
                    return;
                }

                Client.CS.MyEntity.Lastmaterial = newBlock;
                Client.CS.CurrentMap.ClientChangeBlock(Client, (short)(Client.CS.MyEntity.X / 32), (short)(Client.CS.MyEntity.Y / 32), (short)((Client.CS.MyEntity.Z / 32) - 1), 1, Client.CS.MyEntity.Lastmaterial.OnClient);
                Chat.SendClientChat(Client, "&eBlock placed.");
            }
        }
    }
    public struct PlayersCommand : Command {
        public string Command { get { return "/players"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "Op"; } }
        public string Help { get { return "&eBans a player.<br>&eUsage: /Ban [Name] <Reason>"; } }

        public string ShowRanks { get { return "1,2"; } }
        public string UseRanks { get { return "1,2"; } }

        public void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            if (Client.CS.CPEExtensions.ContainsKey("ExtPlayerList")) {
                Chat.SendClientChat(Client, "&eIt appears your client supports CPE ExtPlayerList.<br>&eTo see all online players and what map they are on, hold tab!");
                return;
            }

            string OnlineString = "&eOnline Players: " + Core.nh.Clients.Count.ToString() + "<br>";

            foreach (HypercubeMap hm in Core.Maps) {
                OnlineString += "&e" + hm.Map.MapName + "&f: ";

                foreach (NetworkClient c in hm.Clients) {
                    OnlineString += c.CS.FormattedName + " ";
                }
                OnlineString += "<br>";
            }

            Chat.SendClientChat(Client, OnlineString);
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
    public struct SetrankCommand : Command {
        public string Command { get { return "/setrank"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "Op"; } }
        public string Help { get { return "&eChanges the rank of a player.<br>&eUsage: /setrank [Name] [RankName]"; } }

        public string ShowRanks { get { return "2"; } }
        public string UseRanks { get { return "2"; } }

        public void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            if (args.Length < 2) {
                Chat.SendClientChat(Client, "&4Error: &fYou are missing some arguments. Look at /cmdhelp setrank.");
                return;
            }

            args[0] = Core.Database.GetPlayerName(args[0]);

            if (args[0] == "") {
                Chat.SendClientChat(Client, "&4Error:&f Could not find player.");
                return;
            }

            var newRank = Core.Rankholder.GetRank(args[1]);

            if (newRank == null) {
                Chat.SendClientChat(Client, "&4Error: &fCould not find the rank you specified.");
                return;
            }

            Core.Database.SetDatabase(args[0], "PlayerDB", "Rank", newRank.ID);

            foreach (NetworkClient c in Core.nh.Clients) {
                if (c.CS.LoginName.ToLower() == args[0]) {
                    Chat.SendClientChat(c, "&eYour rank has been changed to " + newRank.Prefix + newRank.Name + newRank.Suffix + ".");
                    c.CS.FormattedName = newRank.Prefix + c.CS.LoginName + newRank.Suffix;
                }
            }

            Chat.SendClientChat(Client, "&e" + args[0] + " is now ranked " + newRank.Name + ".");
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

            Core.Database.StopPlayer(args[0], StopReason, Client.CS.LoginName);

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

            Core.Database.UnbanPlayer(args[0]);
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

            Core.Database.UnmutePlayer(args[0]);

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

            Core.Database.UnstopPlayer(args[0]);

            foreach (NetworkClient c in Core.nh.Clients) {
                if (c.CS.LoginName.ToLower() == args[0].ToLower()) {
                    c.CS.Stopped = false;
                    break;
                }
            }

        }
    }
}
