using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Hypercube_Classic.Core {
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

        public void AddBlock(string Name, byte OnClient, string PlaceRanks, string DeleteRanks, int Physics, string PhysicsPlugin, bool Kills, int Color, int CPELevel, int CPEReplace, bool Special, int ReplaceOnLoad) {
            if (ServerCore.Database.ContainsBlock(Name))
                return;

            var newBlock = new Block();
            newBlock.Name = Name;
            newBlock.OnClient = OnClient;
            newBlock.RanksPlace = RankContainer.SplitRanks(ServerCore, PlaceRanks);
            newBlock.RanksDelete = RankContainer.SplitRanks(ServerCore, DeleteRanks);
            newBlock.Physics = Physics;
            newBlock.PhysicsPlugin = PhysicsPlugin;
            newBlock.Kills = Kills;
            newBlock.Color = Color;
            newBlock.CPELevel = CPELevel;
            newBlock.CPEReplace = CPEReplace;
            newBlock.Special = Special;
            newBlock.ReplaceOnLoad = ReplaceOnLoad;

            Blocks.Add(newBlock);

            ServerCore.Database.CreateBlock(Name, OnClient, PlaceRanks, DeleteRanks, Physics, PhysicsPlugin, Kills, Color, CPELevel, CPEReplace, Special, ReplaceOnLoad);

            newBlock.ID = ServerCore.Database.GetDatabaseInt(Name, "BlockDB", "ID");
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
        public int ID, Physics, Color, CPELevel, CPEReplace, ReplaceOnLoad;
        public byte OnClient;
        public string Name, PhysicsPlugin;
        public bool Kills, Special;
        public List<Rank> RanksPlace = new List<Rank>();
        public List<Rank> RanksDelete = new List<Rank>();
    }
}
