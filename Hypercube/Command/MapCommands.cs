using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Hypercube.Client;
using Hypercube.Core;
using Hypercube.Libraries;
using Hypercube.Map;
using Hypercube.Network;

namespace Hypercube.Command {
    internal static class MapCommands {
        public static void Init(CommandHandler holder) {
            holder.AddCommand("/building", CBuilding);
            holder.AddCommand("/createtp", CCreateTp);
            holder.AddCommand("/mapadd", CMapadd);
            holder.AddCommand("/mapfill", CMapfill);
            holder.AddCommand("/mapfills", CMapfills);
            holder.AddCommand("/mapinfo", CMapinfo);
            holder.AddCommand("/mapload", CMapload);
            holder.AddCommand("/mapresend", CMapresend);
            holder.AddCommand("/mapresize", CMapresize);
            holder.AddCommand("/history", CMaphistory);
            holder.AddCommand("/mapsave", CMapsave);
            holder.AddCommand("/setspawn", CSetSpawn);
            holder.AddCommand("/physics", CPhysics);

            holder.AddCommand("/texture", CTexture);
            holder.AddCommand("/setedge", CSetEdge);
            holder.AddCommand("/setweather", CSetWeather);
            holder.AddCommand("/setcolors", CSetColors);
        }

        #region CreateTP
        static readonly Command CCreateTp = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SAdds a teleporter to where you are standing.<br>§SUsage: /CreateTP [name]",
            Console = true,
            AllPerms = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
            },

            Handler = CreateTPHandler,
        };

        static void CreateTPHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length == 0) {
                Chat.SendClientChat(client, "§EThis command requires 1 argument. See /cmdhelp createtp for usage.");
                return;
            }

            client.CS.MyEntity.SetBuildmode("CreateTP");
            Chat.SendClientChat(client, "§SBuildmode: Teleporter started.");
            Chat.SendClientChat(client, "§SPlace two blocks to describe the teleporter.");
            client.CS.MyEntity.BuildState = 0;

            var loc = client.CS.MyEntity.GetBlockLocation(); // -- Set the buildmode variables
            client.CS.MyEntity.ClientState.SetCoord(loc.X, loc.Y, loc.Z, 0);
            client.CS.MyEntity.ClientState.SetString(args[0], 0);
            client.CS.MyEntity.ClientState.SetString(client.CS.CurrentMap.CWMap.MapName, 1);
            client.CS.MyEntity.ClientState.SetInt(client.CS.MyEntity.Rot, 0);
            client.CS.MyEntity.ClientState.SetInt(client.CS.MyEntity.Look, 1);
        }

        #endregion
        #region Mapadd
        static readonly Command CMapadd = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SAdds a new map.<br>§SUsage: /mapadd [Name]",
            Console = true,
            AllPerms = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
                {"map.addmap", new Permission { Fullname = "map.addmap", Group = "map", Perm = "addmap"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
            },

            Handler = MapaddHandler,
        };

        static void MapaddHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length == 0) {
                Chat.SendClientChat(client, "§EThis command requires 1 argument. See /cmdhelp mapadd for usage.");
                return;
            }

            if (File.Exists("Maps/" + args[0] + ".cw") || File.Exists("Maps/" + args[0] + ".cwu")) {
                Chat.SendClientChat(client, "§EA file with the name " + args[0] + " already exists.");
                return;
            }

            var newMap = new HypercubeMap("Maps/" + args[0] + ".cw", args[0], 64, 64, 64);
            ServerCore.Maps.Add(args[0], newMap);
            newMap.BlockThread = new Thread(newMap.BlockQueueLoop);
            newMap.PhysicsThread = new Thread(newMap.PhysicsQueueLoop);
            newMap.BlockThread.Start();
            newMap.PhysicsThread.Start();
            TaskScheduler.CreateTask("Memory Conservation (" + newMap.CWMap.MapName + ")", new TimeSpan(0, 0, 30), newMap.MapMain);
            TaskScheduler.CreateTask("Autosave (" + newMap.CWMap.MapName + ")", new TimeSpan(0, newMap.HCSettings.SaveInterval, 0), newMap.MapSave);
            newMap.Teleporters = new TeleporterContainer(newMap.CWMap.MapName);

            Chat.SendClientChat(client, "§SMap added successfully.");
        }
        #endregion
        #region Mapfill
        static readonly Command CMapfill = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SFills the map you are in.<br>§SUsage: /mapfill [Script] <Arguments>",
            Console = false,
            AllPerms = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
                {"map.fillmap", new Permission { Fullname = "map.fillmap", Group = "map", Perm = "fillmap"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
                {"map.fillmap", new Permission { Fullname = "map.fillmap", Group = "map", Perm = "fillmap"}},
            },

            Handler = MapfillHandler,
        };

        static void MapfillHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length == 0) {
                Chat.SendClientChat(client, "§EThis command requires 1 or more arguments.<br>See /cmdhelp mapfill.");
                return;
            }

            if (!ServerCore.Fillholder.Mapfills.ContainsKey(args[0])) {
                Chat.SendClientChat(client, "§EMapfill '" + args[0] + "' not found. See /mapfills.");
                return;
            }

            Chat.SendClientChat(client, "§SFill added to queue...");
            ServerCore.ActionQueue.Enqueue(new MapAction {Action = MapActions.Fill, Arguments = args, Map = client.CS.CurrentMap});
            client.CS.CurrentMap.CWMap.CreatingService = "Classicube";
            client.CS.CurrentMap.CWMap.CreatingUsername = client.CS.LoginName;
        }
        #endregion
        #region MapFills
        static readonly Command CMapfills = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SShow available mapfills. Use them with /Mapfill.",
            Console = false,
            AllPerms = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
                {"map.fillmap", new Permission { Fullname = "map.fillmap", Group = "map", Perm = "fillmap"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
                {"map.fillmap", new Permission { Fullname = "map.fillmap", Group = "map", Perm = "fillmap"}},
            },

            Handler = MapfillsHandler,
        };

        static void MapfillsHandler(NetworkClient client, string[] args, string text1, string text2) {
            var mapFillString = "§D";

            foreach (var value in ServerCore.Fillholder.Mapfills)
                mapFillString += " §S" + value.Key + " §D";

            Chat.SendClientChat(client, "§SMapFills:");
            Chat.SendClientChat(client, mapFillString);
        }
        #endregion
        #region Mapinfo
        static readonly Command CMapinfo = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SGives some information about a map.",
            Console = false,
            AllPerms = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.chat", new Permission { Fullname = "player.chat", Group = "player", Perm = "chat"}},
            },

            Handler = MapinfoHandler,
        };

        static void MapinfoHandler(NetworkClient client, string[] args, string text1, string text2) {
            Chat.SendClientChat(client, "§SMap Name: &f" + client.CS.CurrentMap.CWMap.MapName);
            Chat.SendClientChat(client, "§SSize:&f" + client.CS.CurrentMap.CWMap.SizeX + " x " + client.CS.CurrentMap.CWMap.SizeZ + " x " + client.CS.CurrentMap.CWMap.SizeY);
            Chat.SendClientChat(client, "§SMemory Usage (Rough): &f" + ((client.CS.CurrentMap.CWMap.SizeX * client.CS.CurrentMap.CWMap.SizeY * client.CS.CurrentMap.CWMap.SizeZ) / 2048) + " MB");
            Chat.SendClientChat(client, "§SPhysics Enabled: &f" + client.CS.CurrentMap.HCSettings.Physics);
            Chat.SendClientChat(client, "§SBuilding Enabled: &f" + client.CS.CurrentMap.HCSettings.Building);
            Chat.SendClientChat(client, "§SMapHistory Enabled: &f" + client.CS.CurrentMap.HCSettings.History);
            Chat.SendClientChat(client, "§SBlocksend-Queue: &f" + client.CS.CurrentMap.BlockchangeQueue.Count);
            Chat.SendClientChat(client, "§SPhysics-Queue: &f" + client.CS.CurrentMap.PhysicsQueue.Count);
        }
        #endregion
        #region Mapload
        static readonly Command CMapload = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SLoads a different map file to the map you are in.<br>§SUsage: /mapload <Name>",
            Console = false,
            AllPerms = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
                {"map.fillmap", new Permission { Fullname = "map.fillmap", Group = "map", Perm = "fillmap"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
                {"map.fillmap", new Permission { Fullname = "map.fillmap", Group = "map", Perm = "fillmap"}},
            },

            Handler = MaploadHandler,
        };

        static void MaploadHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length == 0 || args.Length > 1) {
                Chat.SendClientChat(client, "§EIncorrect usage. Please see /cmdhelp mapload.");
                return;
            }

            if (!File.Exists("Maps/" + args[0] + ".cw") && !File.Exists("Maps/" + args[0] + ".cwu")) {
                Chat.SendClientChat(client, "§EFile '" + args[0] + "' not found.");
                return;
            }

            Chat.SendClientChat(client, "§SLoading map...");
            ServerCore.ActionQueue.Enqueue(new MapAction { Action = MapActions.Load, Arguments = args, Map = client.CS.CurrentMap });
            //client.CS.CurrentMap.Load("Maps/" + args[0] + ".cw");
            client.CS.CurrentMap.Resend();
        }
        #endregion
        #region Mapresend
        static readonly Command CMapresend = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SResends the map you are in to everyone on it.",
            Console = false,
            AllPerms = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
            },

            Handler = MapresendHandler,
        };

        static void MapresendHandler(NetworkClient client, string[] args, string text1, string text2) {
            client.CS.CurrentMap.BlockchangeQueue = new ConcurrentQueue<QueueItem>();
            client.CS.CurrentMap.Resend();
        }

        #endregion
        #region Mapresize
        static readonly Command CMapresize = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SResizes the map you are in.<br>§SUsage: /mapresize [X] [Y] [Z]<br>&cDont make smaller maps than 16x16x16, the client can crash!",
            Console = false,
            AllPerms = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
                {"map.fillmap", new Permission { Fullname = "map.fillmap", Group = "map", Perm = "fillmap"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
                {"map.fillmap", new Permission { Fullname = "map.fillmap", Group = "map", Perm = "fillmap"}},
            },

            Handler = MapresizeHandler,
        };

        static void MapresizeHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length != 3) {
                Chat.SendClientChat(client, "§EIncorrect usage. Please see /cmdhelp mapresize.");
                return;
            }

            short x, y, z;
            short.TryParse(args[0], out x);
            short.TryParse(args[1], out y);
            short.TryParse(args[2], out z);

            //client.CS.CurrentMap.Resize(x, y, z);
            ServerCore.ActionQueue.Enqueue(new MapAction { Action = MapActions.Resize, Arguments = args, Map = client.CS.CurrentMap });
            Chat.SendClientChat(client, "§SResize added to queue.");
        }
        #endregion
        #region Maphistory
        static readonly Command CMaphistory = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SEnables or disables block history on the map you are on.<br>§SUsage: /history [on/off]",
            Console = false,
            AllPerms = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
            },

            Handler = HistoryHandler,
        };

        static void HistoryHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length == 0) {
                // -- Toggle
                if (client.CS.CurrentMap.HCSettings.History) {
                    Chat.SendMapChat(client.CS.CurrentMap, "§SBlock history Disabled.", 0, true);
                    client.CS.CurrentMap.HCSettings.History = false;

                    if (client.CS.CurrentMap.History != null)
                        client.CS.CurrentMap.History.UnloadHistory();
                } else {
                    Chat.SendMapChat(client.CS.CurrentMap, "§SBlock history Enabled.", 0, true);
                    client.CS.CurrentMap.HCSettings.History = true;

                    if (client.CS.CurrentMap.History != null)
                        client.CS.CurrentMap.History.ReloadHistory();
                    else 
                        client.CS.CurrentMap.History = new MapHistory(client.CS.CurrentMap);
                    
                }
            } else if (args.Length == 1) {
                if (args[0].ToLower() == "true" || args[0].ToLower() == "on") {
                    Chat.SendMapChat(client.CS.CurrentMap, "§SBlock history Enabled.", 0, true);
                    client.CS.CurrentMap.HCSettings.History = true;

                    if (client.CS.CurrentMap.History != null)
                        client.CS.CurrentMap.History.ReloadHistory();
                    else 
                        client.CS.CurrentMap.History = new MapHistory(client.CS.CurrentMap);
                    
                } else if (args[0].ToLower() == "false" || args[0].ToLower() == "off") {
                    Chat.SendMapChat(client.CS.CurrentMap, "§SBlock history Disabled.", 0, true);
                    client.CS.CurrentMap.HCSettings.History = false;

                    if (client.CS.CurrentMap.History != null)
                        client.CS.CurrentMap.History.UnloadHistory();
                } else
                    Chat.SendClientChat(client, "§EIncorrect command usage. See /cmdhelp history.");
            } else
                Chat.SendClientChat(client, "§EIncorrect command usage. See /cmdhelp history.");
        }
        #endregion
        #region Mapsave
        static readonly Command CMapsave = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SSaves the map you are in.<br>§SUsage: /mapsave <Name><br>§SName is optional.",
            Console = false,
            AllPerms = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
            },

            Handler = MapsaveHandler,
        };

        static void MapsaveHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length == 0 || args.Length == 1)
                ServerCore.ActionQueue.Enqueue(new MapAction { Action = MapActions.Save, Arguments = args, Map = client.CS.CurrentMap });
            else {
                Chat.SendClientChat(client, "§EIncorrect number of arguments. See /cmdhelp mapsave.");
                return;
            }

            Chat.SendClientChat(client, "§SSave added to queue.");
        }
        #endregion
        #region Setspawn
        static readonly Command CSetSpawn = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SChanges the spawnpoint of the map to where you are standing.",
            Console = false,
            AllPerms = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
            },

            Handler = SetspawnHandler,
        };

        static void SetspawnHandler(NetworkClient client, string[] args, string text1, string text2) {
            var blockCoord = client.CS.MyEntity.GetBlockLocation();
            client.CS.CurrentMap.CWMap.SpawnX = blockCoord.X;
            client.CS.CurrentMap.CWMap.SpawnY = blockCoord.Z;
            client.CS.CurrentMap.CWMap.SpawnZ = blockCoord.Y;
            client.CS.CurrentMap.CWMap.SpawnLook = client.CS.MyEntity.Look;
            client.CS.CurrentMap.CWMap.SpawnRotation = client.CS.MyEntity.Rot;
            client.CS.CurrentMap.Save();

            Chat.SendClientChat(client, "§SSpawnpoint set.");
        }
        #endregion
        #region Physics
        static readonly Command CPhysics = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SEnables or disables physics on the map you are on.<br>§SUsage: /physics [on/off]",
            Console = false,
            AllPerms = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
            },

            Handler = PhysicsHandler,
        };

        static void PhysicsHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length == 0) {
                // -- Toggle
                if (client.CS.CurrentMap.HCSettings.Physics) {
                    Chat.SendMapChat(client.CS.CurrentMap, "§SPhysics Disabled.", 0, true);
                    client.CS.CurrentMap.HCSettings.Physics = false;
                } else {
                    Chat.SendMapChat(client.CS.CurrentMap, "§SPhysics Enabled.", 0, true);
                    client.CS.CurrentMap.HCSettings.Physics = true;
                }
            } else if (args.Length == 1) {
                if (args[0].ToLower() == "true" || args[0].ToLower() == "on") {
                    Chat.SendMapChat(client.CS.CurrentMap, "§SPhysics Enabled.", 0, true);
                    client.CS.CurrentMap.HCSettings.Physics = true;
                } else if (args[0].ToLower() == "false" || args[0].ToLower() == "off") {
                    Chat.SendMapChat(client.CS.CurrentMap, "§SPhysics Disabled.", 0, true);
                    client.CS.CurrentMap.HCSettings.Physics = false;
                } else {
                    Chat.SendClientChat(client, "§EIncorrect command usage. See /cmdhelp physics.");
                }
            } else 
                Chat.SendClientChat(client, "§EIncorrect command usage. See /cmdhelp physics.");
        }
        #endregion
        #region Building
        static readonly Command CBuilding = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SEnables or disables building on the map you are on.<br>§SUsage: /building [on/off]",
            Console = false,
            AllPerms = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.build", new Permission { Fullname = "player.build", Group = "player", Perm = "build"}},
                {"player.delete", new Permission { Fullname = "player.delete", Group = "player", Perm = "delete"}},
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
            },

            Handler = BuildingHandler,
        };

        static void BuildingHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length == 0) {
                // -- Toggle
                if (client.CS.CurrentMap.HCSettings.Physics) {
                    Chat.SendMapChat(client.CS.CurrentMap, "§SBuilding Disabled.", 0, true);
                    client.CS.CurrentMap.HCSettings.Building = false;
                } else {
                    Chat.SendMapChat(client.CS.CurrentMap,  "§SBuilding Enabled.", 0, true);
                    client.CS.CurrentMap.HCSettings.Building = true;
                }
            } else if (args.Length == 1) {
                if (args[0].ToLower() == "true" || args[0].ToLower() == "on") {
                    Chat.SendMapChat(client.CS.CurrentMap, "§SBuilding Enabled.", 0, true);
                    client.CS.CurrentMap.HCSettings.Building = true;
                } else if (args[0].ToLower() == "false" || args[0].ToLower() == "off") {
                    Chat.SendMapChat(client.CS.CurrentMap, "§SBuilding Disabled.", 0, true);
                    client.CS.CurrentMap.HCSettings.Building = false;
                } else {
                    Chat.SendClientChat(client, "§EIncorrect command usage. See /cmdhelp building.");
                }
            } else
                Chat.SendClientChat(client, "§EIncorrect command usage. See /cmdhelp building.");
        }
        #endregion

        // -- CPE Commands
        #region Texture
        static readonly Command CTexture = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SSets the texture for the map you are in.<br>§SUsage: /texture [url]",
            Console = false,
            AllPerms = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
                {"map.fillmap", new Permission { Fullname = "map.fillmap", Group = "map", Perm = "fillmap"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
                {"map.fillmap", new Permission { Fullname = "map.fillmap", Group = "map", Perm = "fillmap"}},
            },

            Handler = TextureHandler,
        };

        private static void TextureHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length != 1) {
                Chat.SendClientChat(client, "§EInvalid number of arguments. See /cmdhelp texture");
                return;
            }

            if (!args[0].Contains("http://")) {
                Chat.SendClientChat(client, "§EInvalid texture url.");
                return;
            }

            client.CS.CurrentMap.CPESettings.TextureUrl = args[0];
            Chat.SendClientChat(client, "§SChanged applied.");
            client.CS.CurrentMap.ResendCPE();
        }

        #endregion
        #region SetEgde
        static readonly Command CSetEdge = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SSets the edge blocks for the map you are in.<br>§SUsage: /SetEdge [SideBlock] [EdgeBlock] [SideLevel].",
            Console = false,
            AllPerms = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
                {"map.fillmap", new Permission { Fullname = "map.fillmap", Group = "map", Perm = "fillmap"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
                {"map.fillmap", new Permission { Fullname = "map.fillmap", Group = "map", Perm = "fillmap"}},
            },

            Handler = SetEdgeHandler,
        };

        private static void SetEdgeHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length > 3 || args.Length <= 0) {
                Chat.SendClientChat(client, "§EInvalid number of arguments. See /cmdhelp setedge");
                return;
            }

            Chat.SendClientChat(client, "§SChanged applied.");

            int testint;

            if (int.TryParse(args[0], out testint)) {
                if (testint <= 0) { // -- Reset to default.
                    testint = 7;
                }
                var sideBlock = ServerCore.Blockholder.GetBlock(testint);

                if (sideBlock.Name != "Unknown" && CPE.AppearanceAllowed(sideBlock))
                    client.CS.CurrentMap.CPESettings.SideBlock = sideBlock.OnClient;
                else
                    Chat.SendClientChat(client, "§EBlock ID " + args[0] + " is invalid.");
            }
            else {
                var sideBlock = ServerCore.Blockholder.GetBlock(args[0]);

                if (sideBlock.Name != "Unknown" && CPE.AppearanceAllowed(sideBlock))
                    client.CS.CurrentMap.CPESettings.SideBlock = sideBlock.OnClient;
                else
                    Chat.SendClientChat(client, "§EBlock " + args[0] + " is invalid.");
            }

            if (args.Length < 2) {
                client.CS.CurrentMap.ResendCPE();
                return;
            }

            if (int.TryParse(args[1], out testint)) {
                if (testint <= 0) { // -- Reset to default.
                    testint = 8;
                }
                var sideBlock = ServerCore.Blockholder.GetBlock(testint);

                if (sideBlock.Name != "Unknown" && CPE.AppearanceAllowed(sideBlock))
                    client.CS.CurrentMap.CPESettings.EdgeBlock = sideBlock.OnClient;
                else
                    Chat.SendClientChat(client, "§EBlock ID " + args[1] + " is invalid.");
            } else {
                var sideBlock = ServerCore.Blockholder.GetBlock(args[1]);

                if (sideBlock.Name != "Unknown" && CPE.AppearanceAllowed(sideBlock))
                    client.CS.CurrentMap.CPESettings.EdgeBlock = sideBlock.OnClient;
                else
                    Chat.SendClientChat(client, "§EBlock " + args[1] + " is invalid.");
            }

            if (args.Length < 3) {
                client.CS.CurrentMap.ResendCPE();
                return;
            }

            if (!int.TryParse(args[2], out testint)) {
                Chat.SendClientChat(client, "§EInvalid argument: " + args[2] + ". Must be a number.");
                return;
            }

            if (!(testint <= client.CS.CurrentMap.CWMap.SizeY)) {
                Chat.SendClientChat(client, "§EInvalid side level.");
                return;
            }

            if (testint < 0)
                testint = client.CS.CurrentMap.CWMap.SizeY/2;

            client.CS.CurrentMap.CPESettings.SideLevel = (short) testint;
            client.CS.CurrentMap.ResendCPE();
        }
        #endregion
        #region SetWeather
        static readonly Command CSetWeather = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SSets the weather for the map you are in.<br>§SUsage: /weather [type]<br>§STypes are 0, 1, 2 (None, Rain, Snow)",
            Console = false,
            AllPerms = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
                {"map.fillmap", new Permission { Fullname = "map.fillmap", Group = "map", Perm = "fillmap"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
                {"map.fillmap", new Permission { Fullname = "map.fillmap", Group = "map", Perm = "fillmap"}},
            },

            Handler = SetWeatherHandler,
        };

        private static void SetWeatherHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length != 1) {
                Chat.SendClientChat(client, "§EInvalid number of arguments. See /cmdhelp setweather");
                return;
            }

            switch (args[0].ToLower()) {
                case "0":
                case "none":
                case "sunny":
                    client.CS.CurrentMap.CPESettings.Weather = 0;
                    break;
                case "1":
                case "rain":
                case "rainy":
                    client.CS.CurrentMap.CPESettings.Weather = 1;
                    break;
                case "2":
                case "snow":
                case "snowy":
                    client.CS.CurrentMap.CPESettings.Weather = 2;
                    break;
                default:
                    Chat.SendClientChat(client, "§EInvalid argument. See /cmdhelp setweather");
                    break;

            }
            
            Chat.SendClientChat(client, "§SChanged applied.");
            client.CS.CurrentMap.ResendCPE();
        }

        #endregion
        #region SetColors
        static readonly Command CSetColors = new Command {
            Plugin = "",
            Group = "Map",
            Help = "§SSets the enviroment colors for the map you're in.<br>§SUsage: /setcolors [r] [g] [b] [type] <br>§STypes are 0, 1, 2, 3, 4 (Sky, cloud, fog, ambient, sun)",
            Console = false,
            AllPerms = true,

            UsePermissions = new SortedDictionary<string, Permission> {
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
                {"map.fillmap", new Permission { Fullname = "map.fillmap", Group = "map", Perm = "fillmap"}},
            },

            ShowPermissions = new SortedDictionary<string, Permission> {
                {"player.op", new Permission { Fullname = "player.op", Group = "player", Perm = "op"}},
                {"map.fillmap", new Permission { Fullname = "map.fillmap", Group = "map", Perm = "fillmap"}},
            },

            Handler = SetColorsHandler,
        };

        private static void SetColorsHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length != 4) {
                Chat.SendClientChat(client, "§EInvalid number of arguments. See /cmdhelp setcolors");
                return;
            }

            short r, g, b;

            if (!short.TryParse(args[0], out r)) {
                Chat.SendClientChat(client, "§EField 'R' Must be a number.");
                return;
            }

            if (!short.TryParse(args[1], out g)) {
                Chat.SendClientChat(client, "§EField 'G' Must be a number.");
                return;
            }

            if (!short.TryParse(args[2], out b)) {
                Chat.SendClientChat(client, "§EField 'B' Must be a number.");
                return;
            }

            if (r < 0) {
                r = -1;
            }

            if (g < 0) {
                g = -1;
            }

            if (b < 0) {
                b = -1;
            }

            if (client.CS.CurrentMap.CPESettings.EnvColorsVersion == 0) {
                client.CS.CurrentMap.CPESettings.EnvColorsVersion = 1;
                client.CS.CurrentMap.CPESettings.SkyColor = new short[] {-1, -1, -1};
                client.CS.CurrentMap.CPESettings.CloudColor = new short[] { -1, -1, -1 };
                client.CS.CurrentMap.CPESettings.FogColor = new short[] { -1, -1, -1 };
                client.CS.CurrentMap.CPESettings.AmbientColor = new short[] { -1, -1, -1 };
                client.CS.CurrentMap.CPESettings.SunlightColor = new short[] { -1, -1, -1 };
            }

            switch (args[3].ToLower()) {
                case "0":
                case "sky":
                    client.CS.CurrentMap.CPESettings.SkyColor = new[] {r, g, b};
                    break;
                case "1":
                case "cloud":
                    client.CS.CurrentMap.CPESettings.CloudColor = new[] {r, g, b};
                    break;
                case "2":
                case "fog":
                    client.CS.CurrentMap.CPESettings.FogColor = new[] {r, g, b};
                    break;
                case "3":
                case "ambient":
                case "amb":
                    client.CS.CurrentMap.CPESettings.AmbientColor = new[] {r, g, b};
                    break;
                case "4":
                case "sun":
                case "sunlight":
                    client.CS.CurrentMap.CPESettings.SunlightColor = new[] {r, g, b};
                    break;
                default:
                    Chat.SendClientChat(client, "§EInvalid color type.");
                    return;
            }

            Chat.SendClientChat(client, "§SChanges applied.");
            client.CS.CurrentMap.ResendCPE();
        }

        #endregion
    }
}
