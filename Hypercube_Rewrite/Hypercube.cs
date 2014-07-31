using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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
    // -- TODO: Add vanish
    // -- TODO: Make gui
    // -- TODO: Add Teleporters
    // -- TODO: Make line, box, sphere threaded.
    // -- TODO: Add auto-map saving
    // -- TODO: Fix map history
    // -- TODO: Make mapfills queued and threaded.
    // -- TODO: Finite water physics
    // -- BUG: text '@_@' kicks client (index error)

    public class Hypercube {
        #region Variables
        public bool Running = false;
        public int OnlinePlayers = 0;

        #region Server Settings
        public string ServerName, MOTD, WelcomeMessage, MapMain;
        public bool CompressHistory, ColoredConsole;
        public int MaxBlockChanges = 33000, MaxHistoryEntries = 10, MaxUndoSteps = 1000;
        public List<string> Rules;
        public Rank DefaultRank;

        // -- Log settings
        public string Logfile = "Log";
        public bool RotateLogs = true, LogArguments, LogOutput = false;
        #endregion

        #region Containers
        public PBSettingsLoader Settings;
        public ISettings SysSettings, Rulesfile;
        public Database DB;

        public Heartbeat HB;
        public Logging Logger;
        public Text TextFormats;

        public BuildMode BMContainer;
        public BlockContainer Blockholder;
        public PermissionContainer Permholder;
        public RankContainer Rankholder;
        public CommandHandler Commandholder;
        public FillContainer Fillholder;
        public HCLua Luahandler;
        #endregion

        #region Ids
        public short NextID = 0, FreeID = 0, ENext = 0, EFree = 0;
        public int MapIndex;
        #endregion

        public NetworkHandler nh;
        public List<HypercubeMap> Maps;
        #endregion

        public Hypercube() {
            Settings = new PBSettingsLoader(this);

            Logger = new Logging(this);
            TextFormats = new Text(this);

            SysSettings = Settings.RegisterFile("System.txt", true, new PBSettingsLoader.LoadSettings(ReadSystemSettings));
            Settings.ReadSettings(SysSettings);

            Rulesfile = Settings.RegisterFile("Rules.txt", false, new PBSettingsLoader.LoadSettings(ReadRules));
            Settings.ReadSettings(Rulesfile);

            if (RotateLogs)
                Logger.RotateLogs();
            
            Permholder = new PermissionContainer(this);
            Rankholder = new RankContainer(this);
            Blockholder = new BlockContainer(this);
            BMContainer = new BuildMode(this);

            DefaultRank = Rankholder.GetRank(DefaultRank.Name);

            DB = new Database();
            Logger.Log("Database", "Database loaded.", LogType.Info);

            nh = new NetworkHandler(this);

            Logger.Log("", "Core Initialized.", LogType.Info);

            Maps = new List<HypercubeMap>();
            HypercubeMap.LoadMaps(this);

            var found = false;

            for (int i = 0; i < Maps.Count; i++) {
                if (Maps[i].Path.Contains(MapMain + ".cw")) {
                    MapIndex = i;
                    found = true;
                    break;
                }
            }

            if (!found) {
                var MainMap = new HypercubeMap(this, "Maps/world.cw", "world", 128, 128, 128);
                Maps.Add(MainMap);
                MapIndex = Maps.Count - 1;
                Logger.Log("Core", "Main world not found, a new one has been created.", LogType.Warning);
            }

            Commandholder = new CommandHandler(this);
            Fillholder = new FillContainer(this);

            Luahandler = new HCLua(this);
            Luahandler.RegisterFunctions();
            Luahandler.LoadScripts();
            
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        public void Start() {
            nh.Start();
            Running = true;

            HB = new Heartbeat(this);
            Settings.ReadingThead = new Thread(Settings.SettingsMain);
            Settings.ReadingThead.Start();

            Luahandler.luaThread = new Thread(Luahandler.Main);
            Luahandler.luaThread.Start();

            foreach (HypercubeMap m in Maps) {
                m.ClientThread = new Thread(m.MapMain);
                m.ClientThread.Start();

                m.EntityThread = new Thread(m.EntityLocations);
                m.EntityThread.Start();

                m.BlockThread = new Thread(m.BlockQueueLoop);
                m.BlockThread.Start();

                m.PhysicsThread = new Thread(m.PhysicsQueueLoop);
                m.PhysicsThread.Start();
            }

            Logger.Log("Core", "Server started.", LogType.Info);
        }

        /// <summary>
        /// Stops the server and prepares it to be started again.
        /// </summary>
        public void Stop() {
            OnlinePlayers = 0;

            if (HB != null) {
                HB.Shutdown();
                HB = null;
            }

            nh.Stop();

            foreach (ISettings i in Settings.SettingsFiles) {
                if (i.Save)
                    Settings.SaveSettings(i);
            }

            if (Settings.ReadingThead != null)
                Settings.ReadingThead.Abort();

            if (Luahandler.luaThread != null)
                Luahandler.luaThread.Abort();

            foreach (HypercubeMap m in Maps)
                m.Shutdown();
        }

        #region Main Settings Loading
        public void ReadSystemSettings() {
            ServerName = Settings.ReadSetting(SysSettings, "Name", "Hypercube Server");
            MOTD = Settings.ReadSetting(SysSettings, "MOTD", "Welcome to Hypercube!");
            MapMain = Settings.ReadSetting(SysSettings, "MainMap", "world");
            WelcomeMessage = Settings.ReadSetting(SysSettings, "Welcome Message", "&eWelcome to Hypercube!");
            //DefaultRank = Rankholder.GetRank(Settings.ReadSetting(SysSettings, "Default Rank", "Guest"));

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

        public void SaveSystemSettings() {
            Settings.SaveSetting(SysSettings, "Name", ServerName);
            Settings.SaveSetting(SysSettings, "MOTD", MOTD);
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

        public void ReadRules() {
            if (Rules == null)
                Rules = new List<string>();

            Rules.Clear();

            using (var SR = new StreamReader("Settings/Rules.txt")) {
                while (!SR.EndOfStream)
                    Rules.Add(SR.ReadLine());
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

        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long GetCurrentUnixTime() {
            TimeSpan timeSinceEpoch = (DateTime.UtcNow - UnixEpoch);
            return (long)timeSinceEpoch.TotalSeconds;
        }
    }
}
