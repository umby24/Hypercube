using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Threading;

using Hypercube_Classic.Core;
using Hypercube_Classic.Libraries;
using Hypercube_Classic.Map;
using Hypercube_Classic.Client;

namespace Hypercube_Classic
{
    public struct SystemSettings : ISettings {
        public string Filename { get; set; }
        public DateTime LastModified { get; set; }
        public Dictionary<string, string> Settings { get; set; }
        public object LoadSettings { get; set; }
    }

    /// <summary>
    /// The main class for the Hypercube server
    /// </summary>
    public class Hypercube {
        #region Variables
        public SettingsReader Settings;
        public Logging Logger;
        public LuaWrapper LuaHandler;
        public NetworkHandler nh;
        public List<HypercubeMap> Maps;
        public SystemSettings SysSettings;
        public Database Database;
        public RankContainer Rankholder;
        public bool Running = false;
        public int OnlinePlayers = 0;

        // -- NameID, EntityIDs. 
        public short NextID = 0, FreeID = 0, ENext = 0, EFree = 0;

        // -- System Settings
        public string ServerName, MOTD, WelcomeMessage, MapMain;
        public bool RotateLogs, LogOutput;
        public HypercubeMap MainMap;
        public Rank DefaultRank;
        public Heartbeat ClassicubeHeartbeat;
        // -- Non-Public stuff
        
        #endregion

        /// <summary>
        /// Initiates a new Hypercube server.
        /// </summary>
        public Hypercube() {
            // -- Initiate logging
            Logger = new Logging("Log", false, false); // -- Initially, we will not log anything. This will be left up to user option.

            // -- Load settings
            if (!Directory.Exists("Settings"))
                Directory.CreateDirectory("Settings");

            Database = new Database();

            Rankholder = new RankContainer(this);

            if (Rankholder.Ranks.Count == 0) {
                Database.CreateRank("Guest", "Default", "", "", false, 50);
                Database.CreateRank("Op", "Staff", "&9", "", true);
                Rankholder.LoadRanks(this);
            }

            Logger._Log("Info", "Core", "Database Initilized.");

            Settings = new SettingsReader(this);

            SysSettings = new SystemSettings();
            SysSettings.Filename = "System.txt";
            SysSettings.Settings = new Dictionary<string, string>();
            SysSettings.LoadSettings = new SettingsReader.LoadSettings(ReadSystemSettings);

            Settings.ReadSettings(SysSettings);
            Settings.SettingsFiles.Add(SysSettings);

            if (RotateLogs)
                Logger.RotateLogs();

            // -- Initiate Lua
            try {
                LuaHandler = new LuaWrapper(this);
            } catch (Exception e) {
                Logger._Log("Error", "Lua", "Failed to create Lua handler.");
                Logger._Log("debug", "Lua", e.Message);
            }


            // -- Load the maps.
            Maps = new List<HypercubeMap>();
            MapWatcher.Watch(this);

            bool found = false;
            
            foreach (HypercubeMap Map in Maps) {
                if (Map.Map.MapName == MapMain) {
                    MainMap = Map;
                    found = true;
                    break;
                }
            }

            if (!found) {
                MainMap = new HypercubeMap(this, "Maps/world.cw", "world", 128, 128, 128);
                Maps.Add(MainMap);
                Logger._Log("Info", "Core", "Main world not found, a new one has been created.");
            }

            // -- Initiate Networking

            nh = new NetworkHandler(this);
            nh.Clients = new List<Client.NetworkClient>();

        }

        /// <summary>
        /// Reads the system settings from file.
        /// </summary>
        public void ReadSystemSettings() {
            ServerName = Settings.ReadSetting(SysSettings, "Name", "Hypercube Server");
            MOTD = Settings.ReadSetting(SysSettings, "MOTD", "Welcome to Hypercube!");
            MapMain = Settings.ReadSetting(SysSettings, "MainMap", "world");
            WelcomeMessage = Settings.ReadSetting(SysSettings, "Welcome Message", "&eWelcome to Hypercube!");
            DefaultRank = Rankholder.GetRank(Settings.ReadSetting(SysSettings, "Default Rank", "Guest"));

            RotateLogs = bool.Parse(Settings.ReadSetting(SysSettings, "RotateLogs", "true"));
            LogOutput = bool.Parse(Settings.ReadSetting(SysSettings, "LogOutput", "true"));

            Logger.FileLogging = LogOutput;

            if (Running)
                Logger._Log("Info", "Core", "System settings loaded.");
        }

        /// <summary>
        /// Starts the Hypercube server.
        /// </summary>
        public void Start() {
            nh.Start();

            // -- Start stuff!
            ClassicubeHeartbeat = new Heartbeat(this);

            // -- Start the map entity senders..
            foreach (HypercubeMap m in Maps) {
                m.EntityThread = new Thread(m.MapEntities);
                m.EntityThread.Start();
            }

            Logger._Log("Info", "Core", "Server started.");
        }

        /// <summary>
        /// Stops the hypercube server.
        /// </summary>
        public void Stop() {
            nh.Stop();

            // -- Shutdown things we started.
            ClassicubeHeartbeat.Shutdown();

            foreach (HypercubeMap m in Maps)
                m.Shutdown();

            foreach (ISettings i in Settings.SettingsFiles)
                Settings.SaveSettings(i);
        }
    }
}
