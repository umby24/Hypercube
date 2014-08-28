using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using Hypercube.Network;
using Hypercube.Libraries;
using Hypercube.Core;
using Hypercube.Map;
using Hypercube.Command;
using Hypercube.Mapfills;

namespace Hypercube
{
    // -- ServerCore Classic Minecraft Software by Umby24
    // -- TODO List: (There may be additional TODOs scattered throughout the code, these are just big points)
    // -- 
    // -- TODO: Add physics on mapload
    // -- TODO: Add vanish
    // -- TODO: Make gui
    // -- TODO: Add Teleporters
    // -- TODO: Make line, box, sphere threaded.
    // -- TODO: Add auto-map saving
    // -- TODO: Fix map history
    // -- TODO: Make mapfills queued and threaded.
    // -- BUG: text '@_@' kicks client (index error)

    public static class ServerCore {
        #region Variables
        public static bool Running = false;
        public static int OnlinePlayers = 0;
        public static ConcurrentQueue<MapAction> ActionQueue;
        private static Thread actionThread;
        #region Server SettingsDictionary
        public static string ServerName, Motd, WelcomeMessage, MapMain;
        public static bool CompressHistory, ColoredConsole;
        public static int MaxBlockChanges = 33000, MaxHistoryEntries = 10, MaxUndoSteps = 1000;
        public static List<string> Rules;
        public static Rank DefaultRank;

        // -- Log settings
        public static string Logfile = "Log";
        public static bool RotateLogs = true, LogArguments, LogOutput = false;
        #endregion

        #region Containers
        public static PbSettingsLoader Settings;
        public static Settings SysSettings, Rulesfile;
        public static Database DB;

        public static Heartbeat Hb;
        public static Logging Logger;
        public static Text TextFormats;

        public static BuildMode BmContainer;
        public static BlockContainer Blockholder;
        public static PermissionContainer Permholder;
        public static RankContainer Rankholder;
        public static CommandHandler Commandholder;
        public static FillContainer Fillholder;
        public static HCLua Luahandler;
        #endregion

        #region Ids
        public readonly static Stack<short> FreeEids = new Stack<short>(1000); 
        public readonly static Stack<short> FreeIds = new Stack<short>(1000); 
        //public static short NextId = 0, FreeId = 0, ENext = 0, EFree = 0;
        public static int MapIndex;
        #endregion

        public static NetworkHandler Nh;
        public static List<HypercubeMap> Maps;
        #endregion

        public static void Setup()
        {
            Settings = new PbSettingsLoader();

            Logger = new Logging();
            TextFormats = new Text();

            SysSettings = Settings.RegisterFile("System.txt", true, ReadSystemSettings);
            Settings.ReadSettings(SysSettings);

            Rulesfile = Settings.RegisterFile("Rules.txt", false, ReadRules);
            Settings.ReadSettings(Rulesfile);

            if (RotateLogs)
                Logger.RotateLogs();

            Permholder = new PermissionContainer();
            Rankholder = new RankContainer();
            Blockholder = new BlockContainer();
            BmContainer = new BuildMode();

            DefaultRank = Rankholder.GetRank(DefaultRank.Name);

            DB = new Database();
            Logger.Log("Database", "Database loaded.", LogType.Info);

            Nh = new NetworkHandler();
            Logger.Log("", "Core Initialized.", LogType.Info);

            Maps = new List<HypercubeMap>();
            HypercubeMap.LoadMaps();

            var found = false;

            for (var i = 0; i < Maps.Count; i++) {
                if (Maps[i].Path.Contains(MapMain + ".cw")) {
                    MapIndex = i;
                    found = true;
                    break;
                }
            }

            if (!found) {
                var mainMap = new HypercubeMap("Maps/world.cw", "world", 128, 128, 128);
                Maps.Add(mainMap);
                MapIndex = Maps.Count - 1;
                Logger.Log("Core", "Main world not found, a new one has been created.", LogType.Warning);
            }

            Commandholder = new CommandHandler();
            Fillholder = new FillContainer();

            Luahandler = new HCLua();
            Luahandler.RegisterFunctions();
            Luahandler.LoadScripts();

            foreach (var i in Settings.SettingsFiles) {
                if (i.Save)
                    Settings.SaveSettings(i);
            }

            FillStacks();
            TaskScheduler.TaskThread = new Thread(TaskScheduler.RunTasks);
            ActionQueue = new ConcurrentQueue<MapAction>();
        }

        public static void FillStacks() {
            for (var i = 0; i < 1000; i++) {
                FreeEids.Push((short)i);
                FreeIds.Push((short)i);
            }
        }
        /// <summary>
        /// Starts the server.
        /// </summary>
        public static void Start() {
            Nh.Start();
            Running = true;
            TaskScheduler.TaskThread.Start();

            Hb = new Heartbeat();
            TaskScheduler.CreateTask("File Reloading", new TimeSpan(0, 0, 3), Settings.SettingsMain);
            TaskScheduler.CreateTask("Lua file reloading", new TimeSpan(0, 0, 3), Luahandler.Main);

            foreach (var m in Maps) {
                TaskScheduler.CreateTask("Memory Conservation (" + m.CWMap.MapName + ")", new TimeSpan(0, 0, 30), m.MapMain);

                m.BlockThread = new Thread(m.BlockQueueLoop);
                m.BlockThread.Start();

                m.PhysicsThread = new Thread(m.PhysicsQueueLoop);
                m.PhysicsThread.Start();
            }

            TaskScheduler.CreateTask("Watchdog", new TimeSpan(0, 0, 30), Watchdog.GenHtml);

            actionThread = new Thread(ProcessActions);
            actionThread.Start();

            Logger.Log("Core", "Server started.", LogType.Info);
        }

        /// <summary>
        /// Stops the server and prepares it to be started again.
        /// </summary>
        public static void Stop() {
            OnlinePlayers = 0;

            TaskScheduler.TaskThread.Abort();
            TaskScheduler.ScheduledTasks.Clear();

            Hb = null;

            Nh.Stop();

            foreach (var i in Settings.SettingsFiles) {
                if (i.Save)
                    Settings.SaveSettings(i);
            }

            foreach (var m in Maps)
                m.Shutdown();

            actionThread.Abort();

            DB.DBConnection.Close();

        }

        #region Main SettingsDictionary Loading
        public static void ReadSystemSettings() {
            ServerName = Settings.ReadSetting(SysSettings, "Name", "ServerCore Server");
            Motd = Settings.ReadSetting(SysSettings, "MOTD", "Welcome to ServerCore!");
            MapMain = Settings.ReadSetting(SysSettings, "MainMap", "world");
            WelcomeMessage = Settings.ReadSetting(SysSettings, "Welcome Message", "&eWelcome to ServerCore!");
            //DefaultRank = Rankholder.GetRank(SettingsDictionary.ReadSetting(SysSettings, "Default Rank", "Guest"));

            RotateLogs = bool.Parse(Settings.ReadSetting(SysSettings, "RotateLogs", "true"));
            LogOutput = bool.Parse(Settings.ReadSetting(SysSettings, "LogOutput", "true"));
            CompressHistory = bool.Parse(Settings.ReadSetting(SysSettings, "CompressHistory", "true"));
            LogArguments = bool.Parse(Settings.ReadSetting(SysSettings, "LogArguments", "false"));
            ColoredConsole = bool.Parse(Settings.ReadSetting(SysSettings, "ColoredConsole", "true"));

            MaxBlockChanges = int.Parse(Settings.ReadSetting(SysSettings, "MaxBlocksSecond", "33000"));
            MaxHistoryEntries = int.Parse(Settings.ReadSetting(SysSettings, "MaxHistoryEntries", "10"));

            DefaultRank = new Rank(Settings.ReadSetting(SysSettings, "DefaultRank", "Guest"), "Default", "", "", false, 0, "");

            if (Running)
                Logger.Log("Core", "System settings loaded.", LogType.Info);
        }

        public static void SaveSystemSettings() {
            Settings.SaveSetting(SysSettings, "Name", ServerName);
            Settings.SaveSetting(SysSettings, "MOTD", Motd);
            Settings.SaveSetting(SysSettings, "MainMap", MapMain);
            Settings.SaveSetting(SysSettings, "Welcome Message", WelcomeMessage);
            Settings.SaveSetting(SysSettings, "RotateLogs", RotateLogs.ToString());
            Settings.SaveSetting(SysSettings, "LogOutput", LogOutput.ToString());
            Settings.SaveSetting(SysSettings, "CompressHistory", CompressHistory.ToString());
            Settings.SaveSetting(SysSettings, "LogArguments", LogArguments.ToString());
            Settings.SaveSetting(SysSettings, "ColoredConsole", ColoredConsole.ToString());
            Settings.SaveSetting(SysSettings, "MaxBlocksSecond", MaxBlockChanges.ToString());
            Settings.SaveSetting(SysSettings, "MaxHistoryEntries", MaxHistoryEntries.ToString());
            Settings.SaveSetting(SysSettings, "DefaultRank", DefaultRank.Name);
         }

        public static void ReadRules() {
            if (Rules == null)
                Rules = new List<string>();

            Rules.Clear();

            using (var sr = new StreamReader("Settings/Rules.txt")) {
                while (!sr.EndOfStream)
                    Rules.Add(sr.ReadLine());
            }

            if (Rules.Count == 0) {
                Rules.Add("&cYou do not have any rules defined. Please edit Rules.txt");
                Rules.Add("&cto add your own rules here.");
                File.WriteAllText("Settings/Rules.txt", "&cYou do not have any rules defined. Please edit Rules.txt\n&cto add your own rules here.");
                return;
            }

            Logger.Log("Rules", "Rules loaded.", LogType.Info);
        }
        #endregion
        #region Map Actions

        public static void ProcessActions() {
            while (Running) {
                MapAction action;

                if (ActionQueue.TryDequeue(out action)) {
                    switch (action.Action) {
                        case MapActions.Delete:
                            if (action.Map.Clients.Any()) {
                                foreach (var c in action.Map.ClientsList) {
                                    c.ChangeMap(Maps[MapIndex]);
                                }
                                action.Map.Shutdown();
                                TaskScheduler.ScheduledTasks.Remove("Memory Conservation (" + action.Map.CWMap.MapName +
                                                                    ")");
                                Maps.Remove(action.Map);
                                action.Map = null;
                            }
                            break;
                        case MapActions.Fill:
                            var temp = action.Arguments.ToList();
                            temp.RemoveAt(0);
                            Fillholder.FillMap(action.Map, action.Arguments[0], temp.ToArray());
                            break;
                        case MapActions.Load:
                            action.Map.Load(action.Arguments[0]);
                            break;
                        case MapActions.Resize:
                            action.Map.Resize(short.Parse(action.Arguments[0]), short.Parse(action.Arguments[1]), short.Parse(action.Arguments[2]));
                            break;
                        case MapActions.Save:
                            if (action.Arguments.Any())
                                action.Map.Save(action.Arguments[0]);
                            else
                                action.Map.Save();
                            break;
                        default:
                            Logger.Log("MapAction", "Unknown action type: " + action.Action, LogType.Warning);
                            break;
                    }
                }
                Thread.Sleep(1);
            }
        }
        #endregion
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long GetCurrentUnixTime() {
            var timeSinceEpoch = (DateTime.UtcNow - UnixEpoch);
            return (long)timeSinceEpoch.TotalSeconds;
        }
    }
}
