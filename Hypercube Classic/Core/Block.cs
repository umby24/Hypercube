using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Hypercube_Classic.Core {

    public enum BlockType : byte {
        Air = 0,
        Stone = 1,
        Grass = 2,
        Dirt = 3,
        Cobblestone = 4,
        Wood = 5,
        Sapling = 6,
        Admincrete = 7,
        Water = 8,
        StillWater = 9,
        Lava = 10,
        StillLava = 11,
        Sand = 12,
        Gravel = 13,
        GoldOre = 14,
        IronOre = 15,
        Coal = 16,
        Log = 17,
        Leaves = 18,
        Sponge = 19,
        Glass = 20,

        Red = 21,
        Orange = 22,
        Yellow = 23,
        Lime = 24,
        Green = 25,
        Teal = 26,
        Aqua = 27,
        Cyan = 28,
        Blue = 29,
        Indigo = 30,
        Violet = 31,
        Magenta = 32,
        Pink = 33,
        Black = 34,
        Gray = 35,
        White = 36,

        YellowFlower = 37,
        RedFlower = 38,
        BrownMushroom = 39,
        RedMushroom = 40,

        Gold = 41,
        Iron = 42,
        DoubleSlab = 43,
        Slab = 44,
        Bricks = 45,
        TNT = 46,
        Books = 47,
        MossyCobble = 48,
        Obsidian = 49,
        CobblestoneSlab,
        Rope,
        Sandstone,
        Snow,
        Fire,
        LightPinkWool,
        ForestGreenWool,
        BrownWool,
        DeepBlue,
        Turquoise,
        Ice,
        CeramicTile,
        Magma,
        Pillar,
        Crate,
        StoneBrick
    }

    public class BlockContainer {
        public List<Block> Blocks = new List<Block>();
        Hypercube ServerCore;

        public BlockContainer(Hypercube Core) {
            ServerCore = Core;
        }     

        public void LoadBlocks() {
            Blocks.Clear();
            var dt = ServerCore.Database.GetDataTable("SELECT * FROM BlockDB");
            
            foreach (DataRow c in dt.Rows) {
                var newBlock = new Block();
                newBlock.ID = Convert.ToInt32(c["Number"]);
                newBlock.Name = (string)c["Name"];
                newBlock.OnClient = (byte)Convert.ToInt32(c["OnClient"]);
                newBlock.RanksPlace = RankContainer.SplitRanks(ServerCore, (string)c["PlaceRank"]);
                newBlock.RanksDelete = RankContainer.SplitRanks(ServerCore, (string)c["DeleteRank"]);
                newBlock.Physics = Convert.ToInt32(c["Physics"]);
                newBlock.PhysicsDelay = Convert.ToInt32(c["PhysicsDelay"]);
                newBlock.PhysicsRandom = Convert.ToInt32(c["PhysicsRandom"]);
                newBlock.PhysicsPlugin = (string)c["PhysicsPlugin"];
                newBlock.Kills = ((long)c["Kills"] > 0);
                newBlock.Color = Convert.ToInt32(c["Color"]);
                newBlock.CPELevel = Convert.ToInt32(c["CPELevel"]);
                newBlock.CPEReplace = Convert.ToInt32(c["CPEReplace"]);
                newBlock.Special = ((long)c["Special"] > 0);
                newBlock.ReplaceOnLoad = Convert.ToInt32(c["ReplaceOnLoad"]);

                Blocks.Add(newBlock);
            }

            Blocks.OrderBy(o => o.ID);
        }

        public void UpdateBlock(Block BlockToUpdate) {
            var MyValues = new Dictionary<string, string>();

            MyValues.Add("Name", BlockToUpdate.Name);
            MyValues.Add("OnClient", BlockToUpdate.OnClient.ToString());
            MyValues.Add("PlaceRank", string.Join(",", BlockToUpdate.RanksPlace));
            MyValues.Add("DeleteRank", string.Join(",", BlockToUpdate.RanksDelete));
            MyValues.Add("Physics", BlockToUpdate.Physics.ToString());
            MyValues.Add("PhysicsDelay", BlockToUpdate.PhysicsDelay.ToString());
            MyValues.Add("PhysicsRandom", BlockToUpdate.PhysicsRandom.ToString());
            MyValues.Add("PhysicsPlugin", BlockToUpdate.PhysicsPlugin);
            MyValues.Add("Kills", BlockToUpdate.Kills.ToString());
            MyValues.Add("Color", BlockToUpdate.Color.ToString());
            MyValues.Add("CPELevel", BlockToUpdate.CPELevel.ToString());
            MyValues.Add("CPEReplace", BlockToUpdate.CPEReplace.ToString());
            MyValues.Add("Special", BlockToUpdate.Special.ToString());
            MyValues.Add("ReplaceOnLoad", BlockToUpdate.ReplaceOnLoad.ToString());

            ServerCore.Database.Update("BlockDB", MyValues, "Name='" + BlockToUpdate.Name + "'");
        }

        public Block GetBlock(int ID) {
            Block ThisBlock = null;

            // -- Attempt a fast lookup first.
            if (Blocks[ID].ID == (ID + 1))
                return Blocks[ID];

            // -- Fallback otherwise
            foreach (Block b in Blocks) {
                if (b.ID == (ID + 1)) {
                    ThisBlock = b;
                    break;
                }
            }

            if (ThisBlock == null) {
                ThisBlock = new Block();
                ThisBlock.ID = ID;
                ThisBlock.Name = "Unknown";
                ThisBlock.OnClient = 46;
                ThisBlock.CPELevel = 0;
                ThisBlock.CPEReplace = 46;
                return ThisBlock;
            } else {
                return ThisBlock;
            }
        }

        public Block GetBlock(string Name) {
            Block ThisBlock = null;

            foreach (Block b in Blocks) {
                if (b.Name.ToLower() == Name.ToLower()) {
                    ThisBlock = b;
                    break;
                }
            }

            if (ThisBlock == null) {
                ThisBlock = new Block();
                ThisBlock.ID = 99;
                ThisBlock.Name = "Unknown";
                ThisBlock.OnClient = 46;
                ThisBlock.CPELevel = 0;
                ThisBlock.CPEReplace = 46;
                return ThisBlock;
            } else {
                return ThisBlock;
            }
        }

        public void AddBlock(string Name, byte OnClient, string PlaceRanks, string DeleteRanks, int Physics, int PhysicsDelay, int PhysicsRandom, string PhysicsPlugin, bool Kills, int Color, int CPELevel, int CPEReplace, bool Special, int ReplaceOnLoad) {
            if (ServerCore.Database.ContainsBlock(Name))
                return;

            var newBlock = new Block();
            newBlock.Name = Name;
            newBlock.OnClient = OnClient;
            newBlock.RanksPlace = RankContainer.SplitRanks(ServerCore, PlaceRanks);
            newBlock.RanksDelete = RankContainer.SplitRanks(ServerCore, DeleteRanks);
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

            Blocks.Add(newBlock);

            ServerCore.Database.CreateBlock(Name, OnClient, PlaceRanks, DeleteRanks, Physics, PhysicsDelay, PhysicsRandom, PhysicsPlugin, Kills, Color, CPELevel, CPEReplace, Special, ReplaceOnLoad);

            newBlock.ID = ServerCore.Database.GetDatabaseInt(Name, "BlockDB", "Number");
        }

        public void DeleteBlock(int ID) {
            Block ToDelete = null;

            foreach (Block b in Blocks) {
                if (b.ID == ID) {
                    ToDelete = b;
                    break;
                }
            }

            if (ToDelete != null) {
                Blocks.Remove(ToDelete);
                ServerCore.Database.Delete("BlockDB", "Number=" + ID.ToString());
            }
        }

        public void DeleteBlock(string Name) {
            Block ToDelete = null;

            foreach (Block b in Blocks) {
                if (b.Name.ToLower() == Name.ToLower()) {
                    ToDelete = b;
                    break;
                }
            }

            if (ToDelete != null) {
                Blocks.Remove(ToDelete);
                ServerCore.Database.Delete("BlockDB", "Name='" + Name + "'");
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
