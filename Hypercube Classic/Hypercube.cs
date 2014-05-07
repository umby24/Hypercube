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
using Hypercube_Classic.Command;
using Hypercube_Classic.Network;

// -- Hypercube Classic Minecraft Software by Umby24
// -- TODO List: (There may be additional TODOs scattered throughout the code, these are just big points)
//TODO: Add physics on map load.
//TODO: Add more commands
//BUG: Changing rank doesn't change active color in chat or in player list.

namespace Hypercube_Classic
{
    public struct SystemSettings : ISettings {
        public string Filename { get; set; }
        public string CurrentGroup { get; set; }
        public DateTime LastModified { get; set; }
        public Dictionary<string, Dictionary<string, string>> Settings { get; set; }
        public object LoadSettings { get; set; }
        public bool Save { get; set; }
    }

    /// <summary>
    /// The main class for the Hypercube server
    /// </summary>
    public class Hypercube {
        #region Variables
        public PBSettingsLoader Settings;
        public Logging Logger;
        public LuaWrapper LuaHandler;
        public NetworkHandler nh;
        public List<HypercubeMap> Maps;
        public SystemSettings SysSettings;
        public SystemSettings RulesSettings;
        public Database Database;
        public RankContainer Rankholder;
        public BlockContainer Blockholder;
        public Commands Commandholder;
        public BuildMode BMContainer;
        public FillContainer MapFills;

        public bool Running = false;
        public int OnlinePlayers = 0;

        // -- NameID, EntityIDs. 
        public short NextID = 0, FreeID = 0, ENext = 0, EFree = 0;

        // -- System Settings
        public string ServerName, MOTD, WelcomeMessage, MapMain;
        public bool RotateLogs, LogOutput, CompressHistory, LogArguments, ColoredConsole;
        public int MaxBlockChanges = 33000, MaxHistoryEntries = 10, MaxUndoSteps = 1000;
        public HypercubeMap MainMap;
        public Rank DefaultRank;
        public Heartbeat ClassicubeHeartbeat;
        public List<string> Rules;

        // -- Non-Public stuff
        Thread LuaThread;
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
                Database.CreateRank("Guest", "Default", "&f", "", 0, 50);
                Database.CreateRank("Op", "Staff", "&9", "", 1);
                Rankholder.LoadRanks(this);
            } else 
                Rankholder.LoadRanks(this);
            

            Logger._Log("Core", "Database Initilized.", LogType.Info);

            // -- Load and initiate basic server settings and rules.

            Settings = new PBSettingsLoader(this);

            SysSettings = new SystemSettings();
            SysSettings.Filename = "System.txt";
            SysSettings.CurrentGroup = "";
            SysSettings.Settings = new Dictionary<string, Dictionary<string, string>>();
            SysSettings.LoadSettings = new PBSettingsLoader.LoadSettings(ReadSystemSettings);
            SysSettings.Save = true;

            RulesSettings = new SystemSettings();
            RulesSettings.Filename = "Rules.txt";
            RulesSettings.CurrentGroup = "";
            RulesSettings.Settings = new Dictionary<string, Dictionary<string, string>>();
            RulesSettings.LoadSettings = new PBSettingsLoader.LoadSettings(ReadRules);
            RulesSettings.Save = false;

            Settings.ReadSettings(RulesSettings);
            Settings.ReadSettings(SysSettings);
            Settings.SettingsFiles.Add(SysSettings);
            Settings.SettingsFiles.Add(RulesSettings);

            if (RotateLogs)
                Logger.RotateLogs();

            // -- Initiate Commands

            Commandholder = new Commands(this);

            // -- Initiate Lua

            try {
                LuaHandler = new LuaWrapper(this);
            } catch (Exception e) {
                Logger._Log("Lua", "Failed to create Lua handler.", LogType.Error);
                Logger._Log("Lua", e.Message, LogType.Debug);
            }

            try {
                LuaHandler.RegisterFunctions(); // -- Exposes server functions to lua scripts.
                LuaHandler.LoadLuaScripts(); // -- Load all lua scripts
                LuaThread = new Thread(LuaHandler.LuaMain); // -- Start a thread that watches for changes in each lua script.
            } catch {
                Logger._Log("Lua", "Lua failed to initilize, will continue with base server functionality.", LogType.Error);
            }

            Blockholder = new BlockContainer(this);
            Blockholder.LoadBlocks();

            #region Blocks
            if (Blockholder.Blocks.Count < 65) {
                Blockholder.AddBlock("Air", 0, "1,2", "1,2", 0, 0, 0, "", false, -1, 0, 0, false, -1);
                Blockholder.AddBlock("Stone", 1, "1,2", "1,2", 0, 0, 0, "", false, 6645093, 0, 1, false, -1);
                Blockholder.AddBlock("Grass", 2, "1,2", "1,2", 0, 1200, 1200, "", false, 4960630, 0, 0, false, -1);
                Blockholder.AddBlock("Dirt", 3, "1,2", "1,2", 0, 1200, 1200, "", false, 3624555, 0, 0, false, -1);
                Blockholder.AddBlock("Cobblestone", 4, "1,2", "1,2", 0, 0 ,0, "", false, 6250336, 0, 0, false, -1);
                Blockholder.AddBlock("Planks", 5, "1,2", "1,2", 0, 0, 0, "", false, 4220797, 0, 0, false, -1);
                Blockholder.AddBlock("Sapling", 6, "1,2", "1,2", 0, 0, 0, "", false, 11401600, 0, 0, false, -1);
                Blockholder.AddBlock("Solid", 7, "2", "2", 0, 0, 0, "", false, 4539717, 0, 0, true, -1);
                Blockholder.AddBlock("Water", 8, "1,2", "1,2", 20, 100, 100, "", false, 10438957, 0, 0, false, -1);
                Blockholder.AddBlock("Still Water", 9, "1,2", "1,2", 0, 0, 0, "", false, 10438957, 0, 0, true, -1);
                Blockholder.AddBlock("Lava", 10, "1,2", "1,2", 21, 500, 100, "", false, 1729750, 0, 0, false, -1);
                Blockholder.AddBlock("Still Lava", 11, "1,2", "1,2", 0, 0, 0, "", false, 1729750, 0, 0, true, -1);
                Blockholder.AddBlock("Sand", 12, "1,2", "1,2", 11, 200, 100, "", false, 8431790, 0, 0, false, -1);
                Blockholder.AddBlock("Gravel", 13, "1,2", "1,2", 10, 200, 100, "", false, 6710894, 0, 0, false, -1);
                Blockholder.AddBlock("Gold ore", 14, "1,2", "1,2", 0, 0, 0, "", false, 6648180, 0, 0, false, -1);
                Blockholder.AddBlock("Iron ore", 15, "1,2", "1,2", 0, 0, 0, "", false, -1, 0, 0, false, -1);
                Blockholder.AddBlock("Coal", 16, "1,2", "1,2", 0, 0, 0, "", false, 6118749, 0, 0, false, -1);
                Blockholder.AddBlock("Log", 17, "1,2", "1,2", 0, 0, 0, "", false, 2703954, 0, 0, false, -1);
                Blockholder.AddBlock("Leaves", 18, "1,2", "1,2", 0, 0, 0, "", false, 2535736, 0, 0, false, -1);
                Blockholder.AddBlock("Sponge", 19, "1,2", "1,2", 0, 0, 0, "Lua:SpongePhysics", false, 3117714, 0, 0, false, -1);
                Blockholder.AddBlock("Glass", 20, "1,2", "1,2", 0, 0, 0, "", false, 16118490, 0, 0, false, -1);
                Blockholder.AddBlock("Red Cloth", 21, "1,2", "1,2", 0, 0, 0, "", false, 2763442, 0, 0, false, -1);
                Blockholder.AddBlock("Orange Cloth", 22, "1,2", "1,2", 0, 0, 0, "", false, 2780594, 0, 0, false, -1);
                Blockholder.AddBlock("Yellow Cloth", 23, "1,2", "1,2", 0, 0, 0, "", false, 2798258, 0, 0, false, -1);
                Blockholder.AddBlock("Light Green Cloth", 24, "1,2", "1,2", 0, 0, 0, "", false, 2798189, 0, 0, false, -1);
                Blockholder.AddBlock("Green Cloth", 25, "1,2", "1,2", 0, 0, 0, "", false, 2798122, 0, 0, false, -1);
                Blockholder.AddBlock("Aqua Cloth", 26, "1,2", "1,2", 0, 0, 0, "", false, 7254570, 0, 0, false, -1);
                Blockholder.AddBlock("Cyan Cloth", 27, "1,2", "1,2", 0, 0, 0, "", false, 11711018, 0, 0, false, -1);
                Blockholder.AddBlock("Light Blue Cloth", 28, "1,2", "1,2", 0, 0, 0, "", false, 11699029, 0, 0, false, -1);
                Blockholder.AddBlock("Blue", 29, "1,2", "1,2", 0, 0, 0, "", false, 11690337, 0, 0, false, -1);
                Blockholder.AddBlock("Purple", 30, "1,2", "1,2", 0, 0, 0, "", false, 11676269, 0, 0, false, -1);
                Blockholder.AddBlock("Light Purple Cloth", 31, "1,2", "1,2", 0, 0, 0, "", false, 11680908, 0, 0, false, -1);
                Blockholder.AddBlock("Pink Cloth", 32, "1,2", "1,2", 0, 0, 0, "", false, 11676338, 0, 0, false, -1);
                Blockholder.AddBlock("Dark Pink Cloth", 33, "1,2", "1,2", 0, 0, 0, "", false, 7154354, 0, 0, false, -1);
                Blockholder.AddBlock("Dark Grey Cloth", 34, "1,2", "1,2", 0, 0, 0, "", false, 4144959, 0, 0, false, -1);
                Blockholder.AddBlock("Light Grey Cloth", 35, "1,2", "1,2", 0, 0, 0, "", false, 7566195, 0, 0, false, -1);
                Blockholder.AddBlock("White Cloth", 36, "1,2", "1,2", 0, 0, 0, "", false, 11711154, 0, 0, false, -1);
                Blockholder.AddBlock("Yellow Flower", 37, "1,2", "1,2", 0, 0, 0, "", false, 8454143, 0, 0, false, -1);
                Blockholder.AddBlock("Red Flower", 38, "1,2", "1,2", 0, 0, 0, "", false, 255, 0, 0, false, -1);
                Blockholder.AddBlock("Brown Mushroom", 39, "1,2", "1,2", 0, 0, 0, "", false, 2565927, 0, 0, false, -1);
                Blockholder.AddBlock("Red Mushroom", 40, "1,2", "1,2", 0, 0, 0, "", false, 2631720, 0, 0, false, -1);
                Blockholder.AddBlock("Gold Block", 41, "1,2", "1,2", 0, 0, 0, "", false, 2590138, 0, 0, false, -1);
                Blockholder.AddBlock("Iron Block", 42, "1,2", "1,2", 0, 0, 0, "", false, -1, 0, 0, false, -1);
                Blockholder.AddBlock("Double Stair", 43, "1,2", "1,2", 0, 0, 0, "", false, 2829099, 0, 0, false, -1);
                Blockholder.AddBlock("Stair", 44, "1,2", "1,2", 0, 0, 0, "", false, 2894892, 0, 0, false, -1);
                Blockholder.AddBlock("Bricks", 45, "1,2", "1,2", 0, 0, 0, "", false, 4282014, 0, 0, false, -1);
                Blockholder.AddBlock("TNT", 46, "1,2", "1,2", 0, 0, 0, "", false, 3951751, 0, 0, false, -1);
                Blockholder.AddBlock("Bookcase", 47, "1,2", "1,2", 0, 0, 0, "", false, 3098197, 0, 0, false, -1);
                Blockholder.AddBlock("Mossy Cobblestone", 48, "1,2", "1,2", 0, 0, 0, "", false, 4806729, 0, 0, false, -1);
                Blockholder.AddBlock("Obsidian", 49, "1,2", "1,2", 0, 0, 0, "", false, 1708562, 0, 0, false, -1);

                // -- CPE Blocks
                Blockholder.AddBlock("Cobblestone Slab", 50, "1,2", "1,2", 0, 0, 0, "", false, 8421504, 1, 44, false, -1);
                Blockholder.AddBlock("Rope", 51, "1,2", "1,2", 0, 0, 0, "", false, 4220797, 1, 39, false, -1);
                Blockholder.AddBlock("Sandstone", 52, "1,2", "1,2", 0, 0, 0, "", false, 8431790, 1, 12, false, -1);
                Blockholder.AddBlock("Snow", 53, "1,2", "1,2", 0, 0, 0, "", false, 15461355, 1, 0, false, -1);
                Blockholder.AddBlock("Fire", 54, "1,2", "1,2", 0, 0, 0, "", false, 33023, 1, 10, false, -1);
                Blockholder.AddBlock("Light Pink Wool", 55, "1,2", "1,2", 0, 0, 0, "", false, 16744703, 1, 33, false, -1);
                Blockholder.AddBlock("Forest Green Wool", 56, "1,2", "1,2", 0, 0, 0, "", false, 16384, 1, 25, false, -1);
                Blockholder.AddBlock("Brown Wool", 57, "1,2", "1,2", 0, 0, 0, "", false, 4019043, 1, 3, false, -1);
                Blockholder.AddBlock("Deep Blue Wool", 58, "1,2", "1,2", 0, 0, 0, "", false, 16711680, 1, 29, false, -1);
                Blockholder.AddBlock("Turquoise Wool", 59, "1,2", "1,2", 0, 0, 0, "", false, 16744448, 1, 28, false, -1);
                Blockholder.AddBlock("Ice", 60, "1,2", "1,2", 0, 0, 0, "", false, 16777139, 1, 20, false, -1);
                Blockholder.AddBlock("Ceramic Tile", 61, "1,2", "1,2", 0, 0, 0, "", false, 12632256, 1, 42, false, -1);
                Blockholder.AddBlock("Magma", 62, "1,2", "1,2", 0, 0, 0, "", false, 128, 1, 49, false, -1);
                Blockholder.AddBlock("Pillar", 63, "1,2", "1,2", 0, 0, 0, "", false, 12632256, 1, 36, false, -1);
                Blockholder.AddBlock("Crate", 64, "1,2", "1,2", 0, 0, 0, "", false, 4220797, 1, 5, false, -1);
                Blockholder.AddBlock("Stone Brick", 65, "1,2", "1,2", 0, 0, 0, "", false, 12632256, 1, 1, false, -1);
            }
            #endregion

            // -- Load the maps.

            Maps = new List<HypercubeMap>();
            MapWatcher.Watch(this);

            bool found = false;
            
            foreach (HypercubeMap Map in Maps) {
                if (Map.Path.Contains(MapMain)) {
                    MainMap = Map;
                    found = true;
                    break;
                }
            }

            if (!found) {
                MainMap = new HypercubeMap(this, "Maps/world.cw", "world", 128, 128, 128);
                Maps.Add(MainMap);
                Logger._Log("Core", "Main world not found, a new one has been created.", LogType.Warning);
            }

            // -- Initiate Networking

            nh = new NetworkHandler(this);
            nh.Clients = new List<Client.NetworkClient>();
            
            // -- Initiate BuildModes

            BMContainer = new BuildMode(this);

            // -- Initiate Mapfills

            MapFills = new FillContainer(this);

            #region Default Mapfills
            // -- This registers all of the mapfills built into the server.
            var FlatGen = new MapGenerators.FlatgrassFill();
            FlatGen.Name = "Flatgrass";
            FlatGen.Script = "";
            FlatGen.GenerateNew = new FillContainer.FillNew(MapGenerators.Flatgrass.GenerateFlatGrassNew);
            FlatGen.GenerateExisting = new FillContainer.Fill(MapGenerators.Flatgrass.GenerateFlatGrass);
            MapFills.RegisgerFill("Flatgrass", FlatGen);

            var WhiteGen = new MapGenerators.WhiteMapFill();
            WhiteGen.Name = "White";
            WhiteGen.Script = "";
            WhiteGen.GenerateNew = new FillContainer.FillNew(MapGenerators.White.GenerateWhiteNew);
            WhiteGen.GenerateExisting = new FillContainer.Fill(MapGenerators.White.GenerateWhite);
            MapFills.RegisgerFill("White", WhiteGen);

            #endregion
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
            CompressHistory = bool.Parse(Settings.ReadSetting(SysSettings, "CompressHistory", "true"));
            LogArguments = bool.Parse(Settings.ReadSetting(SysSettings, "LogArguments", "false"));
            ColoredConsole = bool.Parse(Settings.ReadSetting(SysSettings, "ColoredConsole", "true"));

            MaxBlockChanges = int.Parse(Settings.ReadSetting(SysSettings, "MaxBlocksSecond", "33000"));
            MaxHistoryEntries = int.Parse(Settings.ReadSetting(SysSettings, "MaxHistoryEntries", "10"));

            Logger.FileLogging = LogOutput;
            Logger.ColoredOutput = ColoredConsole;

            if (Running)
                Logger._Log("Core", "System settings loaded.", LogType.Info);
        }

        /// <summary>
        /// Loads server rules from rules.txt
        /// </summary>
        public void ReadRules() {
            if (Rules == null)
                Rules = new List<string>();

            if (!File.Exists("Settings/Rules.txt")) {
                Rules.Add("&cYou do not have any rules defined. Please edit Rules.txt");
                Rules.Add("&cto add your own rules here.");
                File.WriteAllText("Settings/Rules.txt", "&cYou do not have any rules defined. Please edit Rules.txt\n&cto add your own rules here.");
                return;
            }

            Rules.Clear();

            using (var SR = new StreamReader("Settings/Rules.txt")) {
                while (!SR.EndOfStream)
                    Rules.Add(SR.ReadLine());
            }

            Logger._Log("Rules", "Rules loaded.", LogType.Info);
        }

        /// <summary>
        /// Starts the Hypercube server.
        /// </summary>
        public void Start() {
            nh.Start();
            LuaThread.Start();

            // -- Start stuff!
            ClassicubeHeartbeat = new Heartbeat(this);
            Settings.ReadingThead = new Thread(Settings.SettingsMain);
            Settings.ReadingThead.Start();

            // -- Start the map entity senders..
            foreach (HypercubeMap m in Maps) {
                m.ClientThread = new Thread(m.MapMain); // -- Memory conservation thread
                m.ClientThread.Start();

                m.EntityThread = new Thread(m.MapEntities); // -- Entity movement thread
                m.EntityThread.Start();

                m.BlockChangeThread = new Thread(m.Blockchanger); // -- Block change thread
                m.BlockChangeThread.Start();

                m.PhysicsThread = new Thread(m.PhysicCompleter); // -- Block physics thread
                m.PhysicsThread.Start();
            }

            Logger._Log("Core", "Server started.", LogType.Info);
        }

        /// <summary>
        /// Stops the hypercube server.
        /// </summary>
        public void Stop() {
            nh.Stop();
            LuaThread.Abort();

            // -- Shutdown things we started.
            ClassicubeHeartbeat.Shutdown();

            if (Settings.ReadingThead != null)
                Settings.ReadingThead.Abort();

            foreach (HypercubeMap m in Maps)
                m.Shutdown();

            foreach (ISettings i in Settings.SettingsFiles)
                Settings.SaveSettings(i);
        }

        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long GetCurrentUnixTime() {
            TimeSpan timeSinceEpoch = (DateTime.UtcNow - UnixEpoch);
            return (long)timeSinceEpoch.TotalSeconds;
        }
    }
}
