using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Hypercube.Client;
using Hypercube.Core;
using Hypercube.Mapfills;
using Hypercube.Map;

namespace Hypercube.Command {
    internal static class MapCommands {
        public static void Init(CommandHandler holder) {
            holder.AddCommand("/building", cBuilding);
            holder.AddCommand("/mapadd", cMapadd);
            holder.AddCommand("/mapfill", cMapfill);
            holder.AddCommand("/mapfills", cMapfills);
            holder.AddCommand("/mapinfo", cMapinfo);
            holder.AddCommand("/mapload", cMapload);
            holder.AddCommand("/mapresend", cMapresend);
            holder.AddCommand("/mapresize", cMapresize);
            holder.AddCommand("/history", cMaphistory);
            holder.AddCommand("/mapsave", cMapsave);
            holder.AddCommand("/setspawn", cSetSpawn);
            holder.AddCommand("/physics", cPhysics);
        }

        #region Mapadd
        static readonly Command cMapadd = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SAdds a new map.<br>§SUsage: /mapadd [Name]",
            Console = true,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
                new Permission {Fullname = "map.addmap", Group = "map", Perm = "addmap"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
                new Permission {Fullname = "map.addmap", Group = "map", Perm = "addmap"},
            },

            Handler = MapaddHandler,
        };

        static void MapaddHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length == 0) {
                Chat.SendClientChat(Client, "§EThis command requires 1 argument. See /cmdhelp mapadd for usage.");
                return;
            }

            if (File.Exists("Maps/" + args[0] + ".cw") || File.Exists("Maps/" + args[0] + ".cwu")) {
                Chat.SendClientChat(Client, "§EA file with the name " + args[0] + " already exists.");
                return;
            }

            var NewMap = new HypercubeMap(Client.ServerCore, "Maps/" + args[0] + ".cw", args[0], 64, 64, 64);
            Client.ServerCore.Maps.Add(NewMap);

            Chat.SendClientChat(Client, "§SMap added successfully.");
        }
        #endregion
        #region Mapfill
        static readonly Command cMapfill = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SFills the map you are in.<br>§SUsage: /mapfill [Script] <Arguments>",
            Console = false,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
                new Permission {Fullname = "map.fillmap", Group = "map", Perm = "fillmap"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
                new Permission {Fullname = "map.fillmap", Group = "map", Perm = "fillmap"},
            },

            Handler = MapfillHandler,
        };

        static void MapfillHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length == 0) {
                Chat.SendClientChat(Client, "§EThis command requires 1 or more arguments.<br>See /cmdhelp mapfill.");
                return;
            }

            if (!Client.ServerCore.Fillholder.Mapfills.ContainsKey(args[0])) {
                Chat.SendClientChat(Client, "§EMapfill '" + args[0] + "' not found. See /mapfills.");
                return;
            }

            Chat.SendClientChat(Client, "§SFill added to queue...");
            Client.ServerCore.Fillholder.FillMap(Client.CS.CurrentMap, args[0]);
        }
        #endregion
        #region MapFills
        static readonly Command cMapfills = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SShow available mapfills. Use them with /Mapfill.",
            Console = false,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
                new Permission {Fullname = "map.fillmap", Group = "map", Perm = "fillmap"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
                new Permission {Fullname = "map.fillmap", Group = "map", Perm = "fillmap"},
            },

            Handler = MapfillsHandler,
        };

        static void MapfillsHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            string MapFillString = "§D";

            foreach (KeyValuePair<string, Fill> value in Client.ServerCore.Fillholder.Mapfills)
                MapFillString += " §S" + value.Key + " §D";

            Chat.SendClientChat(Client, "§SMapFills:");
            Chat.SendClientChat(Client, MapFillString);
        }
        #endregion
        #region Mapinfo
        static readonly Command cMapinfo = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SGives some information about a map.",
            Console = false,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"},
            },

            ShowPermissions = new List<Permission> {
                new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"},
            },

            Handler = MapinfoHandler,
        };

        static void MapinfoHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            Chat.SendClientChat(Client, "§SMap Name: &f" + Client.CS.CurrentMap.CWMap.MapName);
            Chat.SendClientChat(Client, "§SSize:&f" + Client.CS.CurrentMap.CWMap.SizeX.ToString() + " x " + Client.CS.CurrentMap.CWMap.SizeZ.ToString() + " x " + Client.CS.CurrentMap.CWMap.SizeY.ToString());
            Chat.SendClientChat(Client, "§SMemory Usage (Rough): &f" + ((Client.CS.CurrentMap.CWMap.SizeX * Client.CS.CurrentMap.CWMap.SizeY * Client.CS.CurrentMap.CWMap.SizeZ) / 2048).ToString() + " MB");
            Chat.SendClientChat(Client, "§SPhysics Enabled: &f" + Client.CS.CurrentMap.HCSettings.Physics.ToString());
            Chat.SendClientChat(Client, "§SBuilding Enabled: &f" + Client.CS.CurrentMap.HCSettings.Building.ToString());
            Chat.SendClientChat(Client, "§SMapHistory Enabled: &f" + Client.CS.CurrentMap.HCSettings.History.ToString());
            Chat.SendClientChat(Client, "§SBlocksend-Queue: &f" + Client.CS.CurrentMap.BlockchangeQueue.Count.ToString());
            Chat.SendClientChat(Client, "§SPhysics-Queue: &f" + Client.CS.CurrentMap.PhysicsQueue.Count.ToString());
        }
        #endregion
        #region Mapload
        static readonly Command cMapload = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SLoads a different map file to the map you are in.<br>§SUsage: /mapload <Name>",
            Console = false,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
                new Permission {Fullname = "map.fillmap", Group = "map", Perm = "fillmap"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
                new Permission {Fullname = "map.fillmap", Group = "map", Perm = "fillmap"},
            },

            Handler = MaploadHandler,
        };

        static void MaploadHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length == 0 || args.Length > 1) {
                Chat.SendClientChat(Client, "§EIncorrect usage. Please see /cmdhelp mapload.");
                return;
            }

            if (!File.Exists("Maps/" + args[0] + ".cw") && !File.Exists("Maps/" + args[0] + ".cwu")) {
                Chat.SendClientChat(Client, "§EFile '" + args[0] + "' not found.");
                return;
            }

            Chat.SendClientChat(Client, "§SLoading map...");
            Client.CS.CurrentMap.Load("Maps/" + args[0] + ".cw");
            Client.CS.CurrentMap.Resend();
        }
        #endregion
        #region Mapresend
        static readonly Command cMapresend = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SResends the map you are in to everyone on it.",
            Console = false,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            Handler = MapresendHandler,
        };

        static void MapresendHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            Client.CS.CurrentMap.BlockchangeQueue = new System.Collections.Concurrent.ConcurrentQueue<QueueItem>();
            Client.CS.CurrentMap.Resend();
        }

        #endregion
        #region Mapresize
        static readonly Command cMapresize = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SResizes the map you are in.<br>§SUsage: /mapresize [X] [Y] [Z]<br>&cDont make smaller maps than 16x16x16, the client can crash!",
            Console = false,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
                new Permission {Fullname = "map.fillmap", Group = "map", Perm = "fillmap"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
                new Permission {Fullname = "map.fillmap", Group = "map", Perm = "fillmap"},
            },

            Handler = MapresizeHandler,
        };

        static void MapresizeHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length != 3) {
                Chat.SendClientChat(Client, "§EIncorrect usage. Please see /cmdhelp mapresize.");
                return;
            }

            short x = 16, y = 16, z = 16;
            short.TryParse(args[0], out x);
            short.TryParse(args[1], out y);
            short.TryParse(args[2], out z);

            Client.CS.CurrentMap.Resize(x, y, z);
            Chat.SendClientChat(Client, "§SMap resized.");
        }
        #endregion
        #region Maphistory
        static readonly Command cMaphistory = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SEnables or disables block history on the map you are on.<br>§SUsage: /history [on/off]",
            Console = false,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            Handler = HistoryHandler,
        };

        static void HistoryHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length == 0) {
                // -- Toggle
                if (Client.CS.CurrentMap.HCSettings.History) {
                    Chat.SendMapChat(Client.CS.CurrentMap, Client.ServerCore, "§SBlock history Disabled.", 0, true);
                    Client.CS.CurrentMap.HCSettings.History = false;

                    if (Client.CS.CurrentMap.History != null)
                        Client.CS.CurrentMap.History.UnloadHistory();
                } else {
                    Chat.SendMapChat(Client.CS.CurrentMap, Client.ServerCore, "§SBlock history Enabled.", 0, true);
                    Client.CS.CurrentMap.HCSettings.History = true;

                    if (Client.CS.CurrentMap.History != null)
                        Client.CS.CurrentMap.History.ReloadHistory();
                    else 
                        Client.CS.CurrentMap.History = new MapHistory(Client.CS.CurrentMap);
                    
                }
            } else if (args.Length == 1) {
                if (args[0].ToLower() == "true" || args[0].ToLower() == "on") {
                    Chat.SendMapChat(Client.CS.CurrentMap, Client.ServerCore, "§SBlock history Enabled.", 0, true);
                    Client.CS.CurrentMap.HCSettings.History = true;

                    if (Client.CS.CurrentMap.History != null)
                        Client.CS.CurrentMap.History.ReloadHistory();
                    else 
                        Client.CS.CurrentMap.History = new MapHistory(Client.CS.CurrentMap);
                    
                } else if (args[0].ToLower() == "false" || args[0].ToLower() == "off") {
                    Chat.SendMapChat(Client.CS.CurrentMap, Client.ServerCore, "§SBlock history Disabled.", 0, true);
                    Client.CS.CurrentMap.HCSettings.History = false;

                    if (Client.CS.CurrentMap.History != null)
                        Client.CS.CurrentMap.History.UnloadHistory();
                } else
                    Chat.SendClientChat(Client, "§EIncorrect command usage. See /cmdhelp history.");
            } else
                Chat.SendClientChat(Client, "§EIncorrect command usage. See /cmdhelp history.");
        }
        #endregion
        #region Mapsave
        static readonly Command cMapsave = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SSaves the map you are in.<br>§SUsage: /mapsave <Name><br>§SName is optional.",
            Console = false,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            Handler = MapsaveHandler,
        };

        static void MapsaveHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length == 0)
                Client.CS.CurrentMap.Save();
            else if (args.Length == 1)
                Client.CS.CurrentMap.Save("Maps/" + args[0] + ".cw");
            else {
                Chat.SendClientChat(Client, "§EIncorrect number of arguments. See /cmdhelp mapsave.");
                return;
            }

            Chat.SendClientChat(Client, "§SMap saved.");
        }
        #endregion
        #region Setspawn
        static readonly Command cSetSpawn = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SChanges the spawnpoint of the map to where you are standing.",
            Console = false,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            Handler = SetspawnHandler,
        };

        static void SetspawnHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            Client.CS.CurrentMap.CWMap.SpawnX = (short)(Client.CS.MyEntity.X / 32);
            Client.CS.CurrentMap.CWMap.SpawnY = (short)(Client.CS.MyEntity.Z / 32);
            Client.CS.CurrentMap.CWMap.SpawnZ = (short)(Client.CS.MyEntity.Y / 32);
            Client.CS.CurrentMap.CWMap.SpawnLook = Client.CS.MyEntity.Look;
            Client.CS.CurrentMap.CWMap.SpawnRotation = Client.CS.MyEntity.Rot;
            Client.CS.CurrentMap.Save();

            Chat.SendClientChat(Client, "§SSpawnpoint set.");
        }
        #endregion
        #region Physics
        static readonly Command cPhysics = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SEnables or disables physics on the map you are on.<br>§SUsage: /physics [on/off]",
            Console = false,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            Handler = PhysicsHandler,
        };

        static void PhysicsHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length == 0) {
                // -- Toggle
                if (Client.CS.CurrentMap.HCSettings.Physics) {
                    Chat.SendMapChat(Client.CS.CurrentMap, Client.ServerCore, "§SPhysics Disabled.", 0, true);
                    Client.CS.CurrentMap.HCSettings.Physics = false;
                } else {
                    Chat.SendMapChat(Client.CS.CurrentMap, Client.ServerCore, "§SPhysics Enabled.", 0, true);
                    Client.CS.CurrentMap.HCSettings.Physics = true;
                }
            } else if (args.Length == 1) {
                if (args[0].ToLower() == "true" || args[0].ToLower() == "on") {
                    Chat.SendMapChat(Client.CS.CurrentMap, Client.ServerCore, "§SPhysics Enabled.", 0, true);
                    Client.CS.CurrentMap.HCSettings.Physics = true;
                } else if (args[0].ToLower() == "false" || args[0].ToLower() == "off") {
                    Chat.SendMapChat(Client.CS.CurrentMap, Client.ServerCore, "§SPhysics Disabled.", 0, true);
                    Client.CS.CurrentMap.HCSettings.Physics = false;
                } else {
                    Chat.SendClientChat(Client, "§EIncorrect command usage. See /cmdhelp physics.");
                }
            } else 
                Chat.SendClientChat(Client, "§EIncorrect command usage. See /cmdhelp physics.");
        }
        #endregion
        #region Building
        static readonly Command cBuilding = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SEnables or disables building on the map you are on.<br>§SUsage: /building [on/off]",
            Console = false,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
                new Permission {Fullname = "player.op", Group = "player", Perm = "op"},
            },

            Handler = BuildingHandler,
        };

        static void BuildingHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length == 0) {
                // -- Toggle
                if (Client.CS.CurrentMap.HCSettings.Physics) {
                    Chat.SendMapChat(Client.CS.CurrentMap, Client.ServerCore, "§SBuilding Disabled.", 0, true);
                    Client.CS.CurrentMap.HCSettings.Building = false;
                } else {
                    Chat.SendMapChat(Client.CS.CurrentMap, Client.ServerCore, "§SBuilding Enabled.", 0, true);
                    Client.CS.CurrentMap.HCSettings.Building = true;
                }
            } else if (args.Length == 1) {
                if (args[0].ToLower() == "true" || args[0].ToLower() == "on") {
                    Chat.SendMapChat(Client.CS.CurrentMap, Client.ServerCore, "§SBuilding Enabled.", 0, true);
                    Client.CS.CurrentMap.HCSettings.Building = true;
                } else if (args[0].ToLower() == "false" || args[0].ToLower() == "off") {
                    Chat.SendMapChat(Client.CS.CurrentMap, Client.ServerCore, "§SBuilding Disabled.", 0, true);
                    Client.CS.CurrentMap.HCSettings.Building = false;
                } else {
                    Chat.SendClientChat(Client, "§EIncorrect command usage. See /cmdhelp building.");
                }
            } else
                Chat.SendClientChat(Client, "§EIncorrect command usage. See /cmdhelp building.");
        }
        #endregion
    }
}
