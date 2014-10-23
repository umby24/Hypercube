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
    // -- Hypercube Classic Minecraft Software by Umby24
    // -- TODO List: (There may be additional TODOs scattered throughout the code, these are just big points)
    // -- 
    // -- TODO: Add physics on mapload
    // -- TODO: Add vanish -- Partially implemented
    // -- TODO: Make gui
    // -- TODO: [Low] Make line, box, sphere threaded.
    // -- TODO: Allow commands from console
    // -- TODO: Create more Lua events, and extend more server functions to Lua.
    // -- TODO: Make logging system a bit better, so having things from different threads doesn't cause conflicts.
    // -- BUG: Occationally, map history file is in use somewhere else (on map shutdown via console's 'end')

    public static class ServerCore {
        #region Variables
        /// <summary>
        /// Declares if the server is running or not.
        /// </summary>
        public static bool Running = false;
        /// <summary>
        /// The number of officially online players
        /// </summary>
        public static int OnlinePlayers = 0;
        public static ConcurrentQueue<MapAction> ActionQueue;
        private static Thread _actionThread;
        #region Server SettingsDictionary
        public static string ServerName, Motd, WelcomeMessage, MapMain;
        public static bool CompressHistory, ColoredConsole;
        public static int MaxBlockChanges = 33000, MaxHistoryEntries = 10, MaxUndoSteps = 1000, ClickDistance = 160;
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
        public static Logging Logger = new Logging();
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
        #endregion

        public static NetworkHandler Nh;
        public static Dictionary<string, HypercubeMap> Maps;
        #endregion
        
        /// <summary>
        /// Loads server settings, loads the database, and prepares the system for use.
        /// </summary>
        public static void Setup()
        {
            Settings = new PbSettingsLoader();
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

            Maps = new Dictionary<string, HypercubeMap>(StringComparer.InvariantCultureIgnoreCase);
            HypercubeMap.LoadMaps();

            HypercubeMap m;
            Maps.TryGetValue(MapMain, out m);

            if (m == null) {
                var mainMap = new HypercubeMap("Maps/" + MapMain + ".cw", MapMain, 128, 128, 128);
                Maps.Add(MapMain, mainMap);
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
            ActionQueue = new ConcurrentQueue<MapAction>();
        }
        
        /// <summary>
        /// Fills the stacks used for entity ids and ExtPlayerList Name IDs
        /// </summary>
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
            Nh.Start(); // -- Start up the server listening system
            Running = true; // -- Server is now classified as running (keeps all major loops running)

            TaskScheduler.TaskThread = new Thread(TaskScheduler.RunTasks); // -- Create a single thread to run smaller server tasks at registered intervals
            TaskScheduler.TaskThread.Start();

            Hb = new Heartbeat(); // -- Register server tasks
            TaskScheduler.CreateTask("File Reloading", new TimeSpan(0, 0, 1), Settings.SettingsMain);
            TaskScheduler.CreateTask("Lua file reloading", new TimeSpan(0, 0, 1), Luahandler.Main);

            foreach (var m in Maps.Values) {
                TaskScheduler.CreateTask("Memory Conservation (" + m.CWMap.MapName + ")", new TimeSpan(0, 0, 30), m.MapMain);
                TaskScheduler.CreateTask("Autosave (" + m.CWMap.MapName + ")", new TimeSpan(0,m.HCSettings.SaveInterval, 0), m.MapSave);

                m.BlockThread = new Thread(m.BlockQueueLoop); // -- Block queue and physics queue get their own threads, as they can be very intensive at times.
                m.BlockThread.Start();

                m.PhysicsThread = new Thread(m.PhysicsQueueLoop);
                m.PhysicsThread.Start();
            }
            // -- Create tasks for generating html files, and handling client actions.
            TaskScheduler.CreateTask("Client Actions", new TimeSpan(0, 0, 0, 0, 50), HypercubeMap.HandleActionQueue);
            TaskScheduler.CreateTask("Watchdog", new TimeSpan(0, 0, 30), Watchdog.GenHtml);

            _actionThread = new Thread(ProcessActions); // -- Create a thread to handle map actions (Fills, resizes, deletes).
            _actionThread.Start();

             // -- Server fully started
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

            foreach (var m in Maps.Values)
                m.Shutdown();

            _actionThread.Abort();

            DB.DBConnection.Close();

        }

        #region Main SettingsDictionary Loading
        /// <summary>
        /// Loads the core server settings from file.
        /// </summary>
        public static void ReadSystemSettings() {
            ServerName = Settings.ReadSetting(SysSettings, "Name", "Hypercube Server");
            Motd = Settings.ReadSetting(SysSettings, "MOTD", "Welcome to Hypercube!");
            MapMain = Settings.ReadSetting(SysSettings, "MainMap", "world");
            WelcomeMessage = Settings.ReadSetting(SysSettings, "Welcome Message", "&eWelcome to Hypercube!");

            RotateLogs = bool.Parse(Settings.ReadSetting(SysSettings, "RotateLogs", "true"));
            LogOutput = bool.Parse(Settings.ReadSetting(SysSettings, "LogOutput", "true"));
            CompressHistory = bool.Parse(Settings.ReadSetting(SysSettings, "CompressHistory", "true"));
            LogArguments = bool.Parse(Settings.ReadSetting(SysSettings, "LogArguments", "false"));
            ColoredConsole = bool.Parse(Settings.ReadSetting(SysSettings, "ColoredConsole", "true"));

            MaxBlockChanges = int.Parse(Settings.ReadSetting(SysSettings, "MaxBlocksSecond", "33000"));
            MaxHistoryEntries = int.Parse(Settings.ReadSetting(SysSettings, "MaxHistoryEntries", "10"));

            ClickDistance = int.Parse(Settings.ReadSetting(SysSettings, "Click Distance", "160"));

            DefaultRank = new Rank(Settings.ReadSetting(SysSettings, "DefaultRank", "Guest"), "Default", "", "", false, 0, "");

            if (Running)
                Logger.Log("Core", "System settings loaded.", LogType.Info);
        }
        
        /// <summary>
        /// Saves the core server settings to file
        /// </summary>
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
            Settings.SaveSetting(SysSettings, "Click Distance", ClickDistance.ToString());
            Settings.SaveSettings(SysSettings);
         }

        /// <summary>
        /// Loads the server rules list.
        /// </summary>
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
        /// <summary>
        /// Processes queued map actions (Fills, saves, deletes, resizes)
        /// </summary>
        public static void ProcessActions() {
            while (Running) {
                MapAction action;

                if (ActionQueue.TryDequeue(out action)) {
                    switch (action.Action) {
                        case MapActions.Delete:
                            if (action.Map.Clients.Any()) {
                                foreach (var c in action.Map.ClientsList) {
                                    c.ChangeMap(Maps[MapMain]);
                                }
                                action.Map.Shutdown();
                                TaskScheduler.ScheduledTasks.Remove("Memory Conservation (" + action.Map.CWMap.MapName +
                                                                    ")");
                                Maps.Remove(action.Map.CWMap.MapName);
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
        /// <summary>
        /// Timestamp of the Unix epoch.
        /// </summary>
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        /// <summary>
        /// Gets the current unix timestamp
        /// </summary>
        /// <returns>Unix-style timestamp of the current time.</returns>
        public static long GetCurrentUnixTime() {
            var timeSinceEpoch = (DateTime.UtcNow - UnixEpoch);
            return (long)timeSinceEpoch.TotalSeconds;
        }
    }
}
