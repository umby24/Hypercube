using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hypercube.Libraries;

namespace Hypercube.Core {
    public class BlockContainer {
        //public SortedDictionary<int, Block> numberList;
        public SortedDictionary<string, Block> nameList;

        //public List<Block> Blocks = new List<Block>();
        Hypercube Servercore;
        ISettings Blocksfile;

        public BlockContainer(Hypercube Core) {
            Servercore = Core;
            //numberList = new SortedDictionary<int, Block>();
            nameList = new SortedDictionary<string, Block>(StringComparer.InvariantCultureIgnoreCase);
            Blocksfile = Core.Settings.RegisterFile("Blocks.txt", true, new PBSettingsLoader.LoadSettings(LoadBlocks));
            Servercore.Settings.ReadSettings(Blocksfile);
        }

        /// <summary>
        /// Parses the blocks ISettings object to load the blocks for the server.
        /// </summary>
        public void LoadBlocks() {
            //Blocks.Clear();
            //numberList.Clear();
            nameList.Clear();

            foreach (string ID in Blocksfile.Settings.Keys) {
                Servercore.Settings.SelectGroup(Blocksfile, ID);

                var Newblock = new Block();
                Newblock.ID = int.Parse(ID);
                Newblock.Name = Servercore.Settings.ReadSetting(Blocksfile, "Name", "");
                Newblock.OnClient = (byte)Servercore.Settings.ReadSetting(Blocksfile, "OnClient", 46);
                Newblock.Physics = Servercore.Settings.ReadSetting(Blocksfile, "Physics", 0);
                Newblock.PhysicsDelay = Servercore.Settings.ReadSetting(Blocksfile, "PhysicsDelay", 0);
                Newblock.PhysicsRandom = Servercore.Settings.ReadSetting(Blocksfile, "PhysicsRandom", 0);
                Newblock.PhysicsPlugin = Servercore.Settings.ReadSetting(Blocksfile, "PhysicsPlugin", "");
                Newblock.Kills = bool.Parse(Servercore.Settings.ReadSetting(Blocksfile, "Kills", "false"));
                Newblock.Color = Servercore.Settings.ReadSetting(Blocksfile, "Color", 0);
                Newblock.CPELevel = Servercore.Settings.ReadSetting(Blocksfile, "CPELevel", 0);
                Newblock.CPEReplace = Servercore.Settings.ReadSetting(Blocksfile, "CPEReplace", 0);
                Newblock.Special = bool.Parse(Servercore.Settings.ReadSetting(Blocksfile, "Special", "false"));
                Newblock.ReplaceOnLoad = Servercore.Settings.ReadSetting(Blocksfile, "ReplaceOnLoad", -1);
                Newblock.RanksPlace = RankContainer.SplitRanks(Servercore, Servercore.Settings.ReadSetting(Blocksfile, "PlaceRank", "0,1,2,3"));
                Newblock.RanksDelete = RankContainer.SplitRanks(Servercore, Servercore.Settings.ReadSetting(Blocksfile, "DeleteRank", "0,1,2,3"));
                //Blocks.Add(Newblock);
                //numberList.Add(Newblock.ID, Newblock);
                nameList.Add(Newblock.ID.ToString(), Newblock);
                nameList.Add(Newblock.Name, Newblock);
            }

            if (nameList.Count == 0) 
                CreateBlocks();

            Servercore.Logger.Log("BlockContainer", "Blocks loaded", LogType.Info);
        }

        /// <summary>
        /// Creates all of the default blocktypes in the case of them not being present.
        /// </summary>
        public void CreateBlocks() {
            CreateBlock("Air", 0, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, -1, 0, 0, false, -1);
            CreateBlock("Stone", 1, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 6645093, 0, 1, false, -1);
            CreateBlock("Grass", 2, "0,1,2,3", "0,1,2,3", 0, 1200, 1200, "", false, 4960630, 0, 0, false, -1);
            CreateBlock("Dirt", 3, "0,1,2,3", "0,1,2,3", 0, 1200, 1200, "", false, 3624555, 0, 0, false, -1);
            CreateBlock("Cobblestone", 4, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 6250336, 0, 0, false, -1);
            CreateBlock("Planks", 5, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 4220797, 0, 0, false, -1);
            CreateBlock("Sapling", 6, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 11401600, 0, 0, false, -1);
            CreateBlock("Solid", 7, "3,4", "3,4", 0, 0, 0, "", false, 4539717, 0, 0, true, -1);
            CreateBlock("Water", 8, "0,1,2,3", "0,1,2,3", 20, 100, 100, "", false, 10438957, 0, 0, false, -1);
            CreateBlock("Still Water", 9, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 10438957, 0, 0, true, -1);
            CreateBlock("Lava", 10, "0,1,2,3", "0,1,2,3", 21, 500, 100, "", false, 1729750, 0, 0, false, -1);
            CreateBlock("Still Lava", 11, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 1729750, 0, 0, true, -1);
            CreateBlock("Sand", 12, "0,1,2,3", "0,1,2,3", 11, 200, 100, "", false, 8431790, 0, 0, false, -1);
            CreateBlock("Gravel", 13, "0,1,2,3", "0,1,2,3", 10, 200, 100, "", false, 6710894, 0, 0, false, -1);
            CreateBlock("Gold ore", 14, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 6648180, 0, 0, false, -1);
            CreateBlock("Iron ore", 15, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, -1, 0, 0, false, -1);
            CreateBlock("Coal", 16, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 6118749, 0, 0, false, -1);
            CreateBlock("Log", 17, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 2703954, 0, 0, false, -1);
            CreateBlock("Leaves", 18, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 2535736, 0, 0, false, -1);
            CreateBlock("Sponge", 19, "0,1,2,3", "0,1,2,3", 0, 0, 0, "Lua:SpongePhysics", false, 3117714, 0, 0, false, -1);
            CreateBlock("Glass", 20, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 16118490, 0, 0, false, -1);
            CreateBlock("Red Cloth", 21, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 2763442, 0, 0, false, -1);
            CreateBlock("Orange Cloth", 22, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 2780594, 0, 0, false, -1);
            CreateBlock("Yellow Cloth", 23, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 2798258, 0, 0, false, -1);
            CreateBlock("Light Green Cloth", 24, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 2798189, 0, 0, false, -1);
            CreateBlock("Green Cloth", 25, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 2798122, 0, 0, false, -1);
            CreateBlock("Aqua Cloth", 26, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 7254570, 0, 0, false, -1);
            CreateBlock("Cyan Cloth", 27, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 11711018, 0, 0, false, -1);
            CreateBlock("Light Blue Cloth", 28, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 11699029, 0, 0, false, -1);
            CreateBlock("Blue", 29, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 11690337, 0, 0, false, -1);
            CreateBlock("Purple", 30, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 11676269, 0, 0, false, -1);
            CreateBlock("Light Purple Cloth", 31, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 11680908, 0, 0, false, -1);
            CreateBlock("Pink Cloth", 32, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 11676338, 0, 0, false, -1);
            CreateBlock("Dark Pink Cloth", 33, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 7154354, 0, 0, false, -1);
            CreateBlock("Dark Grey Cloth", 34, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 4144959, 0, 0, false, -1);
            CreateBlock("Light Grey Cloth", 35, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 7566195, 0, 0, false, -1);
            CreateBlock("White Cloth", 36, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 11711154, 0, 0, false, -1);
            CreateBlock("Yellow Flower", 37, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 8454143, 0, 0, false, -1);
            CreateBlock("Red Flower", 38, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 255, 0, 0, false, -1);
            CreateBlock("Brown Mushroom", 39, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 2565927, 0, 0, false, -1);
            CreateBlock("Red Mushroom", 40, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 2631720, 0, 0, false, -1);
            CreateBlock("Gold Block", 41, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 2590138, 0, 0, false, -1);
            CreateBlock("Iron Block", 42, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, -1, 0, 0, false, -1);
            CreateBlock("Double Stair", 43, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 2829099, 0, 0, false, -1);
            CreateBlock("Stair", 44, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 2894892, 0, 0, false, -1);
            CreateBlock("Bricks", 45, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 4282014, 0, 0, false, -1);
            CreateBlock("TNT", 46, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 3951751, 0, 0, false, -1);
            CreateBlock("Bookcase", 47, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 3098197, 0, 0, false, -1);
            CreateBlock("Mossy Cobblestone", 48, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 4806729, 0, 0, false, -1);
            CreateBlock("Obsidian", 49, "0,1,2,3", "0,1,2,3", 0, 0, 0, "", false, 1708562, 0, 0, false, -1);

            // -- CPE Blocks
            CreateBlock("Cobblestone Slab", 50, "1,2", "1,2", 0, 0, 0, "", false, 8421504, 1, 44, false, -1);
            CreateBlock("Rope", 51, "1,2", "1,2", 0, 0, 0, "", false, 4220797, 1, 39, false, -1);
            CreateBlock("Sandstone", 52, "1,2", "1,2", 0, 0, 0, "", false, 8431790, 1, 12, false, -1);
            CreateBlock("Snow", 53, "1,2", "1,2", 0, 0, 0, "", false, 15461355, 1, 0, false, -1);
            CreateBlock("Fire", 54, "1,2", "1,2", 0, 0, 0, "", false, 33023, 1, 10, false, -1);
            CreateBlock("Light Pink Wool", 55, "1,2", "1,2", 0, 0, 0, "", false, 16744703, 1, 33, false, -1);
            CreateBlock("Forest Green Wool", 56, "1,2", "1,2", 0, 0, 0, "", false, 16384, 1, 25, false, -1);
            CreateBlock("Brown Wool", 57, "1,2", "1,2", 0, 0, 0, "", false, 4019043, 1, 3, false, -1);
            CreateBlock("Deep Blue Wool", 58, "1,2", "1,2", 0, 0, 0, "", false, 16711680, 1, 29, false, -1);
            CreateBlock("Turquoise Wool", 59, "1,2", "1,2", 0, 0, 0, "", false, 16744448, 1, 28, false, -1);
            CreateBlock("Ice", 60, "1,2", "1,2", 0, 0, 0, "", false, 16777139, 1, 20, false, -1);
            CreateBlock("Ceramic Tile", 61, "1,2", "1,2", 0, 0, 0, "", false, 12632256, 1, 42, false, -1);
            CreateBlock("Magma", 62, "1,2", "1,2", 0, 0, 0, "", false, 128, 1, 49, false, -1);
            CreateBlock("Pillar", 63, "1,2", "1,2", 0, 0, 0, "", false, 12632256, 1, 36, false, -1);
            CreateBlock("Crate", 64, "1,2", "1,2", 0, 0, 0, "", false, 4220797, 1, 5, false, -1);
            CreateBlock("Stone Brick", 65, "1,2", "1,2", 0, 0, 0, "", false, 12632256, 1, 1, false, -1);
        }

        /// <summary>
        /// Updates the settings for a certain block, and saves it to file.
        /// </summary>
        /// <param name="toUpdate">The block to be updated on file.</param>
        public void UpdateBlock(Block toUpdate) {
            Servercore.Settings.SelectGroup(Blocksfile, toUpdate.ID.ToString());

            Blocksfile.Settings[toUpdate.ID.ToString()]["Name"] = toUpdate.Name;
            Blocksfile.Settings[toUpdate.ID.ToString()]["OnClient"] = toUpdate.OnClient.ToString();
            Blocksfile.Settings[toUpdate.ID.ToString()]["Physics"] = toUpdate.Physics.ToString();
            Blocksfile.Settings[toUpdate.ID.ToString()]["PhysicsDelay"] = toUpdate.PhysicsDelay.ToString();
            Blocksfile.Settings[toUpdate.ID.ToString()]["PhysicsRandom"] = toUpdate.PhysicsRandom.ToString();
            Blocksfile.Settings[toUpdate.ID.ToString()]["PhysicsPlugin"] = toUpdate.PhysicsPlugin;
            Blocksfile.Settings[toUpdate.ID.ToString()]["Kills"] = toUpdate.Kills.ToString();
            Blocksfile.Settings[toUpdate.ID.ToString()]["Color"] = toUpdate.Color.ToString();
            Blocksfile.Settings[toUpdate.ID.ToString()]["CPELevel"] = toUpdate.CPELevel.ToString();
            Blocksfile.Settings[toUpdate.ID.ToString()]["CPEReplace"] = toUpdate.CPEReplace.ToString();
            Blocksfile.Settings[toUpdate.ID.ToString()]["Special"] = toUpdate.Special.ToString();
            Blocksfile.Settings[toUpdate.ID.ToString()]["ReplaceOnLoad"] = toUpdate.ReplaceOnLoad.ToString();

            Servercore.Settings.SaveSettings(Blocksfile);
        }

        /// <summary>
        /// Returns the block for a given ID.
        /// </summary>
        /// <param name="ID">The block ID for the block you want.</param>
        /// <returns>Block object</returns>
        public Block GetBlock(int ID) {
            //if (ID == 1)
            //    ID = 1;

            //if (!numberList.ContainsKey(ID)) {
            //    Block myBlock = new Block();
            //    myBlock.ID = 99;
            //    myBlock.Name = "Unknown";
            //    myBlock.OnClient = 46;
            //    myBlock.CPELevel = 0;
            //    myBlock.CPEReplace = 46;
            //    myBlock.RanksDelete = RankContainer.SplitRanks(Servercore, "2,3");
            //    return myBlock;
            //}

            return GetBlock(ID.ToString());
        }

        /// <summary>
        /// Performs a slow, name based lookup for a block.
        /// </summary>
        /// <param name="Name">The name of the block you wish to be returned.</param>
        /// <returns>The block object that was found. The name will be "Unknown" if the block could not be found.</returns>
        public Block GetBlock(string Name) {
            if (Name == "1")
                Name = "Stone";

            if (!nameList.ContainsKey(Name)) {
                Block myBlock = new Block();
                myBlock.ID = 99;
                myBlock.Name = "Unknown";
                myBlock.OnClient = 46;
                myBlock.CPELevel = 0;
                myBlock.CPEReplace = 46;
                myBlock.RanksDelete = RankContainer.SplitRanks(Servercore, "2,3");
                return myBlock;
            }

            return nameList[Name];
        }

        /// <summary>
        /// Gets the next availiable internal ID for a block to use. If one is not available, 257 is returned.
        /// </summary>
        /// <returns></returns>
        internal int GetNextID() {
            for (int i = 0; i < 256; i++) {
                if (GetBlock(i).Name == "Unknown") 
                    return i;
            }

            return 257;
        }

        /// <summary>
        /// Creates a new block and saves it to file.
        /// </summary>
        /// <param name="Name">Name of the block.</param>
        /// <param name="OnClient">The block ID to send to the client.</param>
        /// <param name="PlaceRanks">Comma seperated list of ranks that can place this block.</param>
        /// <param name="DeleteRanks">Comma seperated list of ranks that can delete this block.</param>
        /// <param name="Physics">The physics type to be processed for this block.</param>
        /// <param name="PhysicsDelay">The amount of time between physics ticks for this block.</param>
        /// <param name="PhysicsRandom">A random time added to the base physics delay.</param>
        /// <param name="PhysicsPlugin">The plugin that will be called to handle physics.</param>
        /// <param name="Kills">True if a player will be killed upon contact with this block.</param>
        /// <param name="Color">The color code for this block.</param>
        /// <param name="CPELevel">The CustomBlocks level that this block is in.</param>
        /// <param name="CPEReplace">The block to replace this block with if the client doesn't support the above CPE Level.</param>
        /// <param name="Special">True to show this block on the custom materials list.</param>
        /// <param name="ReplaceOnLoad">-1 for none. Replaces this block with another on map load.</param>
        public void CreateBlock(string Name, byte OnClient, string PlaceRanks, string DeleteRanks, int Physics, int PhysicsDelay, int PhysicsRandom, string PhysicsPlugin, bool Kills, int Color, int CPELevel, int CPEReplace, bool Special, int ReplaceOnLoad) {
            if (GetBlock(Name).Name != "Unknown") // -- Block already exists, do not overwrite.
                return;

            var newBlock = new Block();
            newBlock.ID = GetNextID();
            newBlock.Name = Name;
            newBlock.OnClient = OnClient;
            newBlock.Physics = Physics;
            newBlock.PhysicsDelay = PhysicsDelay;
            newBlock.PhysicsRandom = PhysicsRandom;
            newBlock.PhysicsPlugin = PhysicsPlugin;
            newBlock.Kills = Kills;
            newBlock.Color = Color;
            newBlock.CPELevel = CPELevel;
            newBlock.CPEReplace = CPEReplace;
            newBlock.Special = Special;
            newBlock.ReplaceOnLoad = ReplaceOnLoad;

            //numberList.Add(newBlock.ID, newBlock);
            nameList.Add(newBlock.ID.ToString(), newBlock);
            nameList.Add(newBlock.Name, newBlock);
            //Blocks.Add(newBlock);

            Servercore.Settings.SelectGroup(Blocksfile, newBlock.ID.ToString());
            Servercore.Settings.SaveSetting(Blocksfile, "Name", newBlock.Name);
            Servercore.Settings.SaveSetting(Blocksfile, "OnClient", newBlock.OnClient.ToString());
            Servercore.Settings.SaveSetting(Blocksfile, "Physics", newBlock.Physics.ToString());
            Servercore.Settings.SaveSetting(Blocksfile, "PhysicsDelay", newBlock.PhysicsDelay.ToString());
            Servercore.Settings.SaveSetting(Blocksfile, "PhysicsRandom", newBlock.PhysicsRandom.ToString());
            Servercore.Settings.SaveSetting(Blocksfile, "PhysicsPlugin", newBlock.PhysicsPlugin);
            Servercore.Settings.SaveSetting(Blocksfile, "Kills", newBlock.Kills.ToString());
            Servercore.Settings.SaveSetting(Blocksfile, "Color", newBlock.Color.ToString());
            Servercore.Settings.SaveSetting(Blocksfile, "CPELevel", newBlock.CPELevel.ToString());
            Servercore.Settings.SaveSetting(Blocksfile, "CPEReplace", newBlock.CPEReplace.ToString());
            Servercore.Settings.SaveSetting(Blocksfile, "Special", newBlock.Special.ToString());
            Servercore.Settings.SaveSetting(Blocksfile, "ReplaceOnLoad", newBlock.ReplaceOnLoad.ToString());
        }

        /// <summary>
        /// Removes a block from the server.
        /// </summary>
        /// <param name="ID">ID of the block to remove.</param>
        public void DeleteBlock(int ID) {
            DeleteBlock(ID.ToString());
            //var toDelete = GetBlock(ID);

            //if (toDelete != null) {
            //    //Blocks.Remove(toDelete);
            //    nameList.Remove(toDelete.Name);
            //    nameList.Remove(toDelete.ID.ToString());
            //    //numberList.Remove(toDelete.ID);
            //    Blocksfile.Settings.Remove(toDelete.ID.ToString());
            //    Servercore.Settings.SaveSettings(Blocksfile);
            //}
        }

        /// <summary>
        /// Removes a block from the server.
        /// </summary>
        /// <param name="Name">Name of the block to remove.</param>
        public void DeleteBlock(string Name) {
            var toDelete = GetBlock(Name);

            if (toDelete != null) {
                nameList.Remove(toDelete.Name);
                nameList.Remove(toDelete.ID.ToString());
                //numberList.Remove(toDelete.ID);
                Blocksfile.Settings.Remove(toDelete.ID.ToString());
                Servercore.Settings.SaveSettings(Blocksfile);
            }
        }
    }

    public class Block {
        public int ID, Physics, Color, CPELevel, CPEReplace, ReplaceOnLoad, PhysicsDelay, PhysicsRandom;
        public byte OnClient;
        public string Name, PhysicsPlugin;
        public bool Kills, Special;
        public List<Rank> RanksPlace = new List<Rank>();
        public List<Rank> RanksDelete = new List<Rank>();
    }
}
