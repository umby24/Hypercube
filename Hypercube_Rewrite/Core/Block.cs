using System;
using System.Collections.Generic;
using Hypercube.Libraries;

namespace Hypercube.Core {
    public class BlockContainer {
        public Block[] NumberList;
        public SortedDictionary<string, Block> NameList;

        public Block UnknownBlock;

        readonly Settings _blocksfile;

        public BlockContainer() {
            NumberList = new Block[255];
            NameList = new SortedDictionary<string, Block>(StringComparer.InvariantCultureIgnoreCase);
            _blocksfile = ServerCore.Settings.RegisterFile("Blocks.txt", true, LoadBlocks);
            
            UnknownBlock = new Block
                {
                    Id = 99,
                    Name = "Unknown",
                    OnClient = 46,
                    CPELevel = 0,
                    CPEReplace = 46,
                    DeletePermissions = PermissionContainer.SplitPermissions("player.op"),
                    Special = true,
                };

            ServerCore.Settings.ReadSettings(_blocksfile);
        }

        /// <summary>
        /// Parses the blocks ISettings object to load the blocks for the server.
        /// </summary>
        public void LoadBlocks() {
            NameList.Clear();
            FillBlocks();

            foreach (var id in _blocksfile.SettingsDictionary.Keys) {
                ServerCore.Settings.SelectGroup(_blocksfile, id);

                var newblock = new Block {
                    Id = int.Parse(id),
                    Name = ServerCore.Settings.ReadSetting(_blocksfile, "Name", ""),
                    OnClient = (byte) ServerCore.Settings.ReadSetting(_blocksfile, "OnClient", 46),
                    Physics = ServerCore.Settings.ReadSetting(_blocksfile, "Physics", 0),
                    PhysicsDelay = ServerCore.Settings.ReadSetting(_blocksfile, "PhysicsDelay", 0),
                    PhysicsRandom = ServerCore.Settings.ReadSetting(_blocksfile, "PhysicsRandom", 0),
                    PhysicsPlugin = ServerCore.Settings.ReadSetting(_blocksfile, "PhysicsPlugin", ""),
                    RepeatPhysics = bool.Parse(ServerCore.Settings.ReadSetting(_blocksfile, "RepeatPhysics", "false")),
                    Kills = bool.Parse(ServerCore.Settings.ReadSetting(_blocksfile, "Kills", "false")),
                    Color = ServerCore.Settings.ReadSetting(_blocksfile, "Color", 0),
                    CPELevel = ServerCore.Settings.ReadSetting(_blocksfile, "CPELevel", 0),
                    CPEReplace = ServerCore.Settings.ReadSetting(_blocksfile, "CPEReplace", 0),
                    Special = bool.Parse(ServerCore.Settings.ReadSetting(_blocksfile, "Special", "false")),
                    ReplaceOnLoad = ServerCore.Settings.ReadSetting(_blocksfile, "ReplaceOnLoad", -1),
                    PlacePermissions = PermissionContainer.SplitPermissions(ServerCore.Settings.ReadSetting(_blocksfile, "PlacePerms", "player.build")),
                    DeletePermissions = PermissionContainer.SplitPermissions(ServerCore.Settings.ReadSetting(_blocksfile, "DeletePerms", "player.delete")),
                };
                NumberList[newblock.Id] = newblock;
                NameList.Add(newblock.Name, newblock);
            }

            if (NameList.Count == 0)
                CreateBlocks();

            ServerCore.Logger.Log("BlockContainer", "Blocks loaded", LogType.Info);
        }

        /// <summary>
        /// Creates all of the default blocktypes in the case of them not being present.
        /// </summary>
        public void CreateBlocks() {
            CreateBlock("Air", 0, "player.build", "player.delete", 0, 0, 0, "", false, false, -1, 0, 0, false, -1);
            CreateBlock("Stone", 1, "player.build", "player.delete", 0, 0, 0, "", false, false, 6645093, 0, 1, false, -1);
            CreateBlock("Grass", 2, "player.build", "player.delete", 0, 1200, 1200, "", false, false, 4960630, 0, 0, false, -1);
            CreateBlock("Dirt", 3, "player.build", "player.delete", 0, 1200, 1200, "", false, false, 3624555, 0, 0, false, -1);
            CreateBlock("Cobblestone", 4, "player.build", "player.delete", 0, 0, 0, "", false, false, 6250336, 0, 0, false, -1);
            CreateBlock("Planks", 5, "player.build", "player.delete", 0, 0, 0, "", false, false, 4220797, 0, 0, false, -1);
            CreateBlock("Sapling", 6, "player.build", "player.delete", 0, 0, 0, "", false, false, 11401600, 0, 0, false, -1);
            CreateBlock("Solid", 7, "player.op,player.build", "player.op,player.delete", 0, 0, 0, "", false, false, 4539717, 0, 0, false, -1);
            CreateBlock("Water", 8, "player.op,player.build", "player.op,player.delete", 20, 100, 100, "", false, false, 10438957, 0, 0, false, -1);
            CreateBlock("Still Water", 9, "player.build", "player.delete", 21, 100, 100, "", false, false, 10438957, 0, 0, true, -1);
            CreateBlock("Lava", 10, "player.op,player.build", "player.op,player.delete", 20, 500, 100, "", false, false, 1729750, 0, 0, false, -1);
            CreateBlock("Still Lava", 11, "player.build", "player.delete", 21, 500, 100, "", false, false, 1729750, 0, 0, true, -1);
            CreateBlock("Sand", 12, "player.build", "player.delete", 11, 200, 100, "", false, false, 8431790, 0, 0, false, -1);
            CreateBlock("Gravel", 13, "player.build", "player.delete", 10, 200, 100, "", false, false, 6710894, 0, 0, false, -1);
            CreateBlock("Gold ore", 14, "player.build", "player.delete", 0, 0, 0, "", false, false, 6648180, 0, 0, false, -1);
            CreateBlock("Iron ore", 15, "player.build", "player.delete", 0, 0, 0, "", false, false, -1, 0, 0, false, -1);
            CreateBlock("Coal", 16, "player.build", "player.delete", 0, 0, 0, "", false, false, 6118749, 0, 0, false, -1);
            CreateBlock("Log", 17, "player.build", "player.delete", 0, 0, 0, "", false, false, 2703954, 0, 0, false, -1);
            CreateBlock("Leaves", 18, "player.build", "player.delete", 0, 0, 0, "", false, false, 2535736, 0, 0, false, -1);
            CreateBlock("Sponge", 19, "player.build", "player.delete", 0, 0, 0, "", false, false, 3117714, 0, 0, false, -1);
            CreateBlock("Glass", 20, "player.build", "player.delete", 0, 0, 0, "", false, false, 16118490, 0, 0, false, -1);
            CreateBlock("Red Cloth", 21, "player.build", "player.delete", 0, 0, 0, "", false, false, 2763442, 0, 0, false, -1);
            CreateBlock("Orange Cloth", 22, "player.build", "player.delete", 0, 0, 0, "", false, false, 2780594, 0, 0, false, -1);
            CreateBlock("Yellow Cloth", 23, "player.build", "player.delete", 0, 0, 0, "", false, false, 2798258, 0, 0, false, -1);
            CreateBlock("Light Green Cloth", 24, "player.build", "player.delete", 0, 0, 0, "", false, false, 2798189, 0, 0, false, -1);
            CreateBlock("Green Cloth", 25, "player.build", "player.delete", 0, 0, 0, "", false, false, 2798122, 0, 0, false, -1);
            CreateBlock("Aqua Cloth", 26, "player.build", "player.delete", 0, 0, 0, "", false, false, 7254570, 0, 0, false, -1);
            CreateBlock("Cyan Cloth", 27, "player.build", "player.delete", 0, 0, 0, "", false, false, 11711018, 0, 0, false, -1);
            CreateBlock("Light Blue Cloth", 28, "player.build", "player.delete", 0, 0, 0, "", false, false, 11699029, 0, 0, false, -1);
            CreateBlock("Blue", 29, "player.build", "player.delete", 0, 0, 0, "", false, false, 11690337, 0, 0, false, -1);
            CreateBlock("Purple", 30, "player.build", "player.delete", 0, 0, 0, "", false, false, 11676269, 0, 0, false, -1);
            CreateBlock("Light Purple Cloth", 31, "player.build", "player.delete", 0, 0, 0, "", false, false, 11680908, 0, 0, false, -1);
            CreateBlock("Pink Cloth", 32, "player.build", "player.delete", 0, 0, 0, "", false, false, 11676338, 0, 0, false, -1);
            CreateBlock("Dark Pink Cloth", 33, "player.build", "player.delete", 0, 0, 0, "", false, false, 7154354, 0, 0, false, -1);
            CreateBlock("Dark Grey Cloth", 34, "player.build", "player.delete", 0, 0, 0, "", false, false, 4144959, 0, 0, false, -1);
            CreateBlock("Light Grey Cloth", 35, "player.build", "player.delete", 0, 0, 0, "", false, false, 7566195, 0, 0, false, -1);
            CreateBlock("White Cloth", 36, "player.build", "player.delete", 0, 0, 0, "", false, false, 11711154, 0, 0, false, -1);
            CreateBlock("Yellow Flower", 37, "player.build", "player.delete", 0, 0, 0, "", false, false, 8454143, 0, 0, false, -1);
            CreateBlock("Red Flower", 38, "player.build", "player.delete", 0, 0, 0, "", false, false, 255, 0, 0, false, -1);
            CreateBlock("Brown Mushroom", 39, "player.build", "player.delete", 0, 0, 0, "", false, false, 2565927, 0, 0, false, -1);
            CreateBlock("Red Mushroom", 40, "player.build", "player.delete", 0, 0, 0, "", false, false, 2631720, 0, 0, false, -1);
            CreateBlock("Gold Block", 41, "player.build", "player.delete", 0, 0, 0, "", false, false, 2590138, 0, 0, false, -1);
            CreateBlock("Iron Block", 42, "player.build", "player.delete", 0, 0, 0, "", false, false, -1, 0, 0, false, -1);
            CreateBlock("Double Stair", 43, "player.build", "player.delete", 0, 0, 0, "", false, false, 2829099, 0, 0, false, -1);
            CreateBlock("Stair", 44, "player.build", "player.delete", 0, 0, 0, "", false, false, 2894892, 0, 0, false, -1);
            CreateBlock("Bricks", 45, "player.build", "player.delete", 0, 0, 0, "", false, false, 4282014, 0, 0, false, -1);
            CreateBlock("TNT", 46, "player.build", "player.delete", 0, 0, 0, "", false, false, 3951751, 0, 0, false, -1);
            CreateBlock("Bookcase", 47, "player.build", "player.delete", 0, 0, 0, "", false, false, 3098197, 0, 0, false, -1);
            CreateBlock("Mossy Cobblestone", 48, "player.build", "player.delete", 0, 0, 0, "", false, false, 4806729, 0, 0, false, -1);
            CreateBlock("Obsidian", 49, "player.build", "player.delete", 0, 0, 0, "", false, false, 1708562, 0, 0, false, -1);

            // -- CPE Blocks
            CreateBlock("Cobblestone Slab", 50, "player.build", "player.delete", 0, 0, 0, "", false, false, 8421504, 1, 44, false, -1);
            CreateBlock("Rope", 51, "player.build", "player.delete", 0, 0, 0, "", false, false, 4220797, 1, 39, false, -1);
            CreateBlock("Sandstone", 52, "player.build", "player.delete", 0, 0, 0, "", false, false, 8431790, 1, 12, false, -1);
            CreateBlock("Snow", 53, "player.build", "player.delete", 22, 200, 50, "", false, false, 15461355, 1, 0, false, -1);
            CreateBlock("Fire", 54, "player.build", "player.delete", 0, 0, 0, "", false, false, 33023, 1, 10, false, -1);
            CreateBlock("Light Pink Wool", 55, "player.build", "player.delete", 0, 0, 0, "", false, false, 16744703, 1, 33, false, -1);
            CreateBlock("Forest Green Wool", 56, "player.build", "player.delete", 0, 0, 0, "", false, false, 16384, 1, 25, false, -1);
            CreateBlock("Brown Wool", 57, "player.build", "player.delete", 0, 0, 0, "", false, false, 4019043, 1, 3, false, -1);
            CreateBlock("Deep Blue Wool", 58, "player.build", "player.delete", 0, 0, 0, "", false, false, 16711680, 1, 29, false, -1);
            CreateBlock("Turquoise Wool", 59, "player.build", "player.delete", 0, 0, 0, "", false, false, 16744448, 1, 28, false, -1);
            CreateBlock("Ice", 60, "player.build", "player.delete", 0, 0, 0, "", false, false, 16777139, 1, 20, false, -1);
            CreateBlock("Ceramic Tile", 61, "player.build", "player.delete", 0, 0, 0, "", false, false, 12632256, 1, 42, false, -1);
            CreateBlock("Magma", 62, "player.build", "player.delete", 0, 0, 0, "", false, false, 128, 1, 49, false, -1);
            CreateBlock("Pillar", 63, "player.build", "player.delete", 0, 0, 0, "", false, false, 12632256, 1, 36, false, -1);
            CreateBlock("Crate", 64, "player.build", "player.delete", 0, 0, 0, "", false, false, 4220797, 1, 5, false, -1);
            CreateBlock("Stone Brick", 65, "player.build", "player.delete", 0, 0, 0, "", false, false, 12632256, 1, 1, false, -1);
        }

        /// <summary>
        /// Fills unused IDs in the number list with the unknown block type.
        /// </summary>
        public void FillBlocks() {
            for (var i = 0; i < 255; i++) {
                if (NumberList[i] == null)
                    NumberList[i] = UnknownBlock;
            }
        }
        
        /// <summary>
        /// Updates the settings for a certain block, and saves it to file.
        /// </summary>
        /// <param name="toUpdate">The block to be updated on file.</param>
        public void UpdateBlock(Block toUpdate) {
            ServerCore.Settings.SelectGroup(_blocksfile, toUpdate.Id.ToString());

            _blocksfile.SettingsDictionary[toUpdate.Id.ToString()]["Name"] = toUpdate.Name;
            _blocksfile.SettingsDictionary[toUpdate.Id.ToString()]["OnClient"] = toUpdate.OnClient.ToString();
            _blocksfile.SettingsDictionary[toUpdate.Id.ToString()]["Physics"] = toUpdate.Physics.ToString();
            _blocksfile.SettingsDictionary[toUpdate.Id.ToString()]["PhysicsDelay"] = toUpdate.PhysicsDelay.ToString();
            _blocksfile.SettingsDictionary[toUpdate.Id.ToString()]["PhysicsRandom"] = toUpdate.PhysicsRandom.ToString();
            _blocksfile.SettingsDictionary[toUpdate.Id.ToString()]["PhysicsPlugin"] = toUpdate.PhysicsPlugin;
            _blocksfile.SettingsDictionary[toUpdate.Id.ToString()]["Kills"] = toUpdate.Kills.ToString();
            _blocksfile.SettingsDictionary[toUpdate.Id.ToString()]["Color"] = toUpdate.Color.ToString();
            _blocksfile.SettingsDictionary[toUpdate.Id.ToString()]["CPELevel"] = toUpdate.CPELevel.ToString();
            _blocksfile.SettingsDictionary[toUpdate.Id.ToString()]["CPEReplace"] = toUpdate.CPEReplace.ToString();
            _blocksfile.SettingsDictionary[toUpdate.Id.ToString()]["Special"] = toUpdate.Special.ToString();
            _blocksfile.SettingsDictionary[toUpdate.Id.ToString()]["ReplaceOnLoad"] = toUpdate.ReplaceOnLoad.ToString();
            _blocksfile.SettingsDictionary[toUpdate.Id.ToString()]["PlacePerms"] =
                PermissionContainer.PermissionsToString(toUpdate.PlacePermissions);
            _blocksfile.SettingsDictionary[toUpdate.Id.ToString()]["DeletePerms"] =
                PermissionContainer.PermissionsToString(toUpdate.DeletePermissions);

            ServerCore.Settings.SaveSettings(_blocksfile);
        }

        /// <summary>
        /// Returns the block for a given ID.
        /// </summary>
        /// <param name="id">The block ID for the block you want.</param>
        /// <returns>Block object</returns>
        public Block GetBlock(int id) {
            if (id > 254)
                return UnknownBlock;

            return NumberList[id];
        }

        /// <summary>
        /// Performs a slow, name based lookup for a block.
        /// </summary>
        /// <param name="name">The name of the block you wish to be returned.</param>
        /// <returns>The block object that was found. The name will be "Unknown" if the block could not be found.</returns>
        public Block GetBlock(string name) {
            Block mBlock;
            return NameList.TryGetValue(name, out mBlock) ? mBlock : UnknownBlock;
        }

        /// <summary>
        /// Gets the next availiable internal ID for a block to use. If one is not available, 257 is returned.
        /// </summary>
        /// <returns></returns>
        internal int GetNextId() {
            for (var i = 0; i < 256; i++) {
                if (GetBlock(i).Name == "Unknown")
                    return i;
            }

            return 257;
        }

        /// <summary>
        /// Creates a new block and saves it to file.
        /// </summary>
        /// <param name="name">Name of the block.</param>
        /// <param name="onClient">The block ID to send to the client.</param>
        /// <param name="deletePerms">Comma seperated list of permissions required to delete this block.</param>
        /// <param name="physics">The physics type to be processed for this block.</param>
        /// <param name="physicsDelay">The amount of time between physics ticks for this block.</param>
        /// <param name="physicsRandom">A random time added to the base physics delay.</param>
        /// <param name="physicsPlugin">The plugin that will be called to handle physics.</param>
        /// <param name="replacePhysics">If the block should be re-added to the physics queue after physics has completed.</param>
        /// <param name="kills">True if a player will be killed upon contact with this block.</param>
        /// <param name="color">The color code for this block.</param>
        /// <param name="cpeLevel">The CustomBlocks level that this block is in.</param>
        /// <param name="cpeReplace">The block to replace this block with if the client doesn't support the above CPE Level.</param>
        /// <param name="special">True to show this block on the custom materials list.</param>
        /// <param name="replaceOnLoad">-1 for none. Replaces this block with another on map load.</param>
        /// <param name="placePerms">Comma seperated list of permissions required to delete this block.</param>
        public void CreateBlock(string name, byte onClient, string placePerms, string deletePerms, int physics, int physicsDelay, int physicsRandom, string physicsPlugin, bool replacePhysics, bool kills, int color, int cpeLevel, int cpeReplace, bool special, int replaceOnLoad) {
            if (GetBlock(name).Name != "Unknown") // -- Block already exists, do not overwrite.
                return;

            var newBlock = new Block {
                Id = GetNextId(),
                Name = name,
                OnClient = onClient,
                Physics = physics,
                PhysicsDelay = physicsDelay,
                PhysicsRandom = physicsRandom,
                PhysicsPlugin = physicsPlugin,
                RepeatPhysics = replacePhysics,
                Kills = kills,
                Color = color,
                CPELevel = cpeLevel,
                CPEReplace = cpeReplace,
                Special = special,
                ReplaceOnLoad = replaceOnLoad,
                PlacePermissions = PermissionContainer.SplitPermissions(placePerms),
                DeletePermissions = PermissionContainer.SplitPermissions(deletePerms),
            };

            NumberList[newBlock.Id] = newBlock;
            NameList.Add(newBlock.Name, newBlock);

            ServerCore.Settings.SelectGroup(_blocksfile, newBlock.Id.ToString());
            ServerCore.Settings.SaveSetting(_blocksfile, "Name", newBlock.Name);
            ServerCore.Settings.SaveSetting(_blocksfile, "OnClient", newBlock.OnClient.ToString());
            ServerCore.Settings.SaveSetting(_blocksfile, "Physics", newBlock.Physics.ToString());
            ServerCore.Settings.SaveSetting(_blocksfile, "PhysicsDelay", newBlock.PhysicsDelay.ToString());
            ServerCore.Settings.SaveSetting(_blocksfile, "PhysicsRandom", newBlock.PhysicsRandom.ToString());
            ServerCore.Settings.SaveSetting(_blocksfile, "PhysicsPlugin", newBlock.PhysicsPlugin);
            ServerCore.Settings.SaveSetting(_blocksfile, "RepeatPhysics", newBlock.RepeatPhysics.ToString());
            ServerCore.Settings.SaveSetting(_blocksfile, "Kills", newBlock.Kills.ToString());
            ServerCore.Settings.SaveSetting(_blocksfile, "Color", newBlock.Color.ToString());
            ServerCore.Settings.SaveSetting(_blocksfile, "CPELevel", newBlock.CPELevel.ToString());
            ServerCore.Settings.SaveSetting(_blocksfile, "CPEReplace", newBlock.CPEReplace.ToString());
            ServerCore.Settings.SaveSetting(_blocksfile, "Special", newBlock.Special.ToString());
            ServerCore.Settings.SaveSetting(_blocksfile, "ReplaceOnLoad", newBlock.ReplaceOnLoad.ToString());
            ServerCore.Settings.SaveSetting(_blocksfile, "PlacePerms", PermissionContainer.PermissionsToString(newBlock.PlacePermissions));
            ServerCore.Settings.SaveSetting(_blocksfile, "DeletePerms", PermissionContainer.PermissionsToString(newBlock.DeletePermissions));
        }

        /// <summary>
        /// Removes a block from the server.
        /// </summary>
        /// <param name="id">ID of the block to remove.</param>
        public void DeleteBlock(int id) {
            var toDelete = GetBlock(id);

            if (toDelete == null) 
                return;

            NameList.Remove(toDelete.Name);
            NumberList[toDelete.Id] = null;
            _blocksfile.SettingsDictionary.Remove(toDelete.Id.ToString());
            ServerCore.Settings.SaveSettings(_blocksfile);
        }

        /// <summary>
        /// Removes a block from the server.
        /// </summary>
        /// <param name="name">Name of the block to remove.</param>
        public void DeleteBlock(string name) {
            var toDelete = GetBlock(name);

            if (toDelete == null) 
                return;

            NameList.Remove(toDelete.Name);
            NameList.Remove(toDelete.Id.ToString());
            NumberList[toDelete.Id] = null;
            _blocksfile.SettingsDictionary.Remove(toDelete.Id.ToString());
            ServerCore.Settings.SaveSettings(_blocksfile);
        }
    }

    public class Block {
        public int Id, Physics, Color, CPELevel, CPEReplace, ReplaceOnLoad, PhysicsDelay, PhysicsRandom;
        public byte OnClient;
        public string Name, PhysicsPlugin;
        public bool Kills, Special, RepeatPhysics;
        public SortedDictionary<string, Permission> PlacePermissions;
        public SortedDictionary<string, Permission> DeletePermissions;
    }
}
