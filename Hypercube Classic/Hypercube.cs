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

// -- Hypercube Classic Minecraft Software by Umby24
// -- TODO List: (There may be additional TODOs scattered throughout the code, these are just big points)
//TODO: Fix localhost clients having a blank service. (This will also apply to non-name verified servers..)
//TODO: Give users the ability to have multiple ranks assigned to them.
//TODO: Fix physics (It's too fast!)
//TODO: Add physics time limitations.
//TODO: Add physics on map load.
//TODO: Add user undo (And save it with map data)
//TODO: Add user block tracking (Framework already in place, easy to do).
//TODO: Add more commands
//TODO: Intigrate Lua
//TODO: Add CPE
//TODO: Save and load command permissions, names, and aliases from file.

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
        public BlockContainer Blockholder;
        public Commands Commandholder;
        public bool Running = false;
        public int OnlinePlayers = 0;

        // -- NameID, EntityIDs. 
        public short NextID = 0, FreeID = 0, ENext = 0, EFree = 0;

        // -- System Settings
        public string ServerName, MOTD, WelcomeMessage, MapMain;
        public bool RotateLogs, LogOutput;
        public int MaxBlockChanges = 33000;
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
                Database.CreateRank("Guest", "Default", "&f", "", false, 50);
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

            // -- Initiate Commands
            Commandholder = new Commands();

            // -- Initiate Lua
            try {
                LuaHandler = new LuaWrapper(this);
            } catch (Exception e) {
                Logger._Log("Error", "Lua", "Failed to create Lua handler.");
                Logger._Log("debug", "Lua", e.Message);
            }

            Blockholder = new BlockContainer(this);
            Blockholder.LoadBlocks();

            if (Blockholder.Blocks.Count < 65) {
                Blockholder.AddBlock("Air", 0, "1,2", "1,2", 0, "", false, -1, 0, 0, false, -1);
                Blockholder.AddBlock("Stone", 1, "1,2", "1,2", 0, "", false, 6645093, 0, 1, false, -1);
                Blockholder.AddBlock("Grass", 2, "1,2", "1,2", 0, "", false, 4960630, 0, 0, false, -1);
                Blockholder.AddBlock("Dirt", 3, "1,2", "1,2", 0, "", false, 3624555, 0, 0, false, -1);
                Blockholder.AddBlock("Cobblestone", 4, "1,2", "1,2", 0, "", false, 6250336, 0, 0, false, -1);
                Blockholder.AddBlock("Planks", 5, "1,2", "1,2", 0, "", false, 4220797, 0, 0, false, -1);
                Blockholder.AddBlock("Sapling", 6, "1,2", "1,2", 0, "", false, 11401600, 0, 0, false, -1);
                Blockholder.AddBlock("Solid", 7, "2", "2", 0, "", false, 4539717, 0, 0, true, -1);
                Blockholder.AddBlock("Water", 8, "1,2", "1,2", 21, "", false, 10438957, 0, 0, false, -1);
                Blockholder.AddBlock("Still Water", 9, "1,2", "1,2", 0, "", false, 10438957, 0, 0, true, -1);
                Blockholder.AddBlock("Lava", 10, "1,2", "1,2", 21, "", false, 1729750, 0, 0, false, -1);
                Blockholder.AddBlock("Still Lava", 11, "1,2", "1,2", 0, "", false, 1729750, 0, 0, true, -1);
                Blockholder.AddBlock("Sand", 12, "1,2", "1,2", 11, "", false, 8431790, 0, 0, false, -1);
                Blockholder.AddBlock("Gravel", 13, "1,2", "1,2", 10, "", false, 6710894, 0, 0, false, -1);
                Blockholder.AddBlock("Gold ore", 14, "1,2", "1,2", 0, "", false, 6648180, 0, 0, false, -1);
                Blockholder.AddBlock("Iron ore", 15, "1,2", "1,2", 0, "", false, -1, 0, 0, false, -1);
                Blockholder.AddBlock("Coal", 16, "1,2", "1,2", 0, "", false, 6118749, 0, 0, false, -1);
                Blockholder.AddBlock("Log", 17, "1,2", "1,2", 0, "", false, 2703954, 0, 0, false, -1);
                Blockholder.AddBlock("Leaves", 18, "1,2", "1,2", 0, "", false, 2535736, 0, 0, false, -1);
                Blockholder.AddBlock("Sponge", 19, "1,2", "1,2", 0, "Lua:SpongePhysics", false, 3117714, 0, 0, false, -1);
                Blockholder.AddBlock("Glass", 20, "1,2", "1,2", 0, "", false, 16118490, 0, 0, false, -1);
                Blockholder.AddBlock("Red Cloth", 21, "1,2", "1,2", 0, "", false, 2763442, 0, 0, false, -1);
                Blockholder.AddBlock("Orange Cloth", 22, "1,2", "1,2", 0, "", false, 2780594, 0, 0, false, -1);
                Blockholder.AddBlock("Yellow Cloth", 23, "1,2", "1,2", 0, "", false, 2798258, 0, 0, false, -1);
                Blockholder.AddBlock("Light Green Cloth", 24, "1,2", "1,2", 0, "", false, 2798189, 0, 0, false, -1);
                Blockholder.AddBlock("Green Cloth", 25, "1,2", "1,2", 0, "", false, 2798122, 0, 0, false, -1);
                Blockholder.AddBlock("Aqua Cloth", 26, "1,2", "1,2", 0, "", false, 7254570, 0, 0, false, -1);
                Blockholder.AddBlock("Cyan Cloth", 27, "1,2", "1,2", 0, "", false, 11711018, 0, 0, false, -1);
                Blockholder.AddBlock("Light Blue Cloth", 28, "1,2", "1,2", 0, "", false, 11699029, 0, 0, false, -1);
                Blockholder.AddBlock("Blue", 29, "1,2", "1,2", 0, "", false, 11690337, 0, 0, false, -1);
                Blockholder.AddBlock("Purple", 30, "1,2", "1,2", 0, "", false, 11676269, 0, 0, false, -1);
                Blockholder.AddBlock("Light Purple Cloth", 31, "1,2", "1,2", 0, "", false, 11680908, 0, 0, false, -1);
                Blockholder.AddBlock("Pink Cloth", 32, "1,2", "1,2", 0, "", false, 11676338, 0, 0, false, -1);
                Blockholder.AddBlock("Dark Pink Cloth", 33, "1,2", "1,2", 0, "", false, 7154354, 0, 0, false, -1);
                Blockholder.AddBlock("Dark Grey Cloth", 34, "1,2", "1,2", 0, "", false, 4144959, 0, 0, false, -1);
                Blockholder.AddBlock("Light Grey Cloth", 35, "1,2", "1,2", 0, "", false, 7566195, 0, 0, false, -1);
                Blockholder.AddBlock("White Cloth", 36, "1,2", "1,2", 0, "", false, 11711154, 0, 0, false, -1);
                Blockholder.AddBlock("Yellow Flower", 37, "1,2", "1,2", 0, "", false, 8454143, 0, 0, false, -1);
                Blockholder.AddBlock("Red Flower", 38, "1,2", "1,2", 0, "", false, 255, 0, 0, false, -1);
                Blockholder.AddBlock("Brown Mushroom", 39, "1,2", "1,2", 0, "", false, 2565927, 0, 0, false, -1);
                Blockholder.AddBlock("Red Mushroom", 40, "1,2", "1,2", 0, "", false, 2631720, 0, 0, false, -1);
                Blockholder.AddBlock("Gold Block", 41, "1,2", "1,2", 0, "", false, 2590138, 0, 0, false, -1);
                Blockholder.AddBlock("Iron Block", 42, "1,2", "1,2", 0, "", false, -1, 0, 0, false, -1);
                Blockholder.AddBlock("Double Stair", 43, "1,2", "1,2", 0, "", false, 2829099, 0, 0, false, -1);
                Blockholder.AddBlock("Stair", 44, "1,2", "1,2", 0, "", false, 2894892, 0, 0, false, -1);
                Blockholder.AddBlock("Bricks", 45, "1,2", "1,2", 0, "", false, 4282014, 0, 0, false, -1);
                Blockholder.AddBlock("TNT", 46, "1,2", "1,2", 0, "", false, 3951751, 0, 0, false, -1);
                Blockholder.AddBlock("Bookcase", 47, "1,2", "1,2", 0, "", false, 3098197, 0, 0, false, -1);
                Blockholder.AddBlock("Mossy Cobblestone", 48, "1,2", "1,2", 0, "", false, 4806729, 0, 0, false, -1);
                Blockholder.AddBlock("Obsidian", 49, "1,2", "1,2", 0, "", false, 1708562, 0, 0, false, -1);

                // -- CPE Blocks
                Blockholder.AddBlock("Cobblestone Slab", 50, "1,2", "1,2", 0, "", false, 8421504, 1, 44, false, -1);
                Blockholder.AddBlock("Rope", 51, "1,2", "1,2", 0, "", false, 4220797, 1, 39, false, -1);
                Blockholder.AddBlock("Sandstone", 52, "1,2", "1,2", 0, "", false, 8431790, 1, 12, false, -1);
                Blockholder.AddBlock("Snow", 53, "1,2", "1,2", 0, "", false, 15461355, 1, 0, false, -1);
                Blockholder.AddBlock("Fire", 54, "1,2", "1,2", 0, "", false, 33023, 1, 10, false, -1);
                Blockholder.AddBlock("Light Pink Wool", 55, "1,2", "1,2", 0, "", false, 16744703, 1, 33, false, -1);
                Blockholder.AddBlock("Forest Green Wool", 56, "1,2", "1,2", 0, "", false, 16384, 1, 25, false, -1);
                Blockholder.AddBlock("Brown Wool", 57, "1,2", "1,2", 0, "", false, 4019043, 1, 3, false, -1);
                Blockholder.AddBlock("Deep Blue Wool", 58, "1,2", "1,2", 0, "", false, 16711680, 1, 29, false, -1);
                Blockholder.AddBlock("Turquoise Wool", 59, "1,2", "1,2", 0, "", false, 16744448, 1, 28, false, -1);
                Blockholder.AddBlock("Ice", 60, "1,2", "1,2", 0, "", false, 16777139, 1, 20, false, -1);
                Blockholder.AddBlock("Ceramic Tile", 61, "1,2", "1,2", 0, "", false, 12632256, 1, 42, false, -1);
                Blockholder.AddBlock("Magma", 62, "1,2", "1,2", 0, "", false, 128, 1, 49, false, -1);
                Blockholder.AddBlock("Pillar", 63, "1,2", "1,2", 0, "", false, 12632256, 1, 36, false, -1);
                Blockholder.AddBlock("Crate", 64, "1,2", "1,2", 0, "", false, 4220797, 1, 5, false, -1);
                Blockholder.AddBlock("Stone Brick", 65, "1,2", "1,2", 0, "", false, 12632256, 1, 1, false, -1);
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

                m.BlockChangeThread = new Thread(m.Blockchanger);
                m.BlockChangeThread.Start();

                m.PhysicsThread = new Thread(m.PhysicCompleter);
                m.PhysicsThread.Start();
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
