using System;
using System.IO;
using ClassicWorld.NET;
using Hypercube.Core;
using Hypercube.Libraries;

namespace Hypercube.Map {
    public class D3Map {
        #region Variables
        public byte[] Blockdata;
        public Vector3S Mapsize, Spawn;
        public byte SpawnRot, SpawnLook;
        public bool PhysicsStopped;
        public string Motd;
        #endregion

        public void LoadMap(string directory, string mapName) {
            if (!File.Exists(directory + "/Data-Layer.gz") || !File.Exists(directory + "/Config.txt"))
                return; // -- Not a valid map.
            //File.Copy(directory + "/Config.txt", "Settings/TempD3Config.txt");

            // -- Load the Config data first..
            var configfile = ServerCore.Settings.RegisterFile("TempD3Config.txt", directory, false, LoadConfig);
            ServerCore.Settings.ReadSettings(configfile);

            Mapsize = new Vector3S {
                X = (short) ServerCore.Settings.ReadSetting(configfile, "Size_X", 128),
                Y = (short) ServerCore.Settings.ReadSetting(configfile, "Size_Y", 128),
                Z = (short) ServerCore.Settings.ReadSetting(configfile, "Size_Z", 128)
            };

            Spawn = new Vector3S {
                X = (short) float.Parse(ServerCore.Settings.ReadSetting(configfile, "Spawn_X", "1.0")),
                Y = (short) float.Parse(ServerCore.Settings.ReadSetting(configfile, "Spawn_Y", "1.0")),
                Z = (short) float.Parse(ServerCore.Settings.ReadSetting(configfile, "Spawn_Z", "1.0"))
            };

            SpawnRot = (byte)ServerCore.Settings.ReadSetting(configfile, "Spawn_Rot", 0);
            SpawnLook = (byte)ServerCore.Settings.ReadSetting(configfile, "Spawn_Look", 0);

            Motd = ServerCore.Settings.ReadSetting(configfile, "MOTD_Override", "");
            PhysicsStopped = Convert.ToBoolean(ServerCore.Settings.ReadSetting(configfile, "Physic_Stopped", 0));

            // -- Load the block data
            GZip.DecompressFile(directory + "/Data-Layer.gz");
            byte[] allData;

            using (var br = new BinaryReader(new FileStream(directory + "/Data-Layer.gz", FileMode.Open))) 
                allData = br.ReadBytes((int)br.BaseStream.Length);

            GZip.CompressFile(directory + "/Data-Layer.gz");

            if (allData.Length != (Mapsize.X * Mapsize.Y * Mapsize.Z) * 4) {
                // -- Size error..
                return;
            }

            Blockdata = new byte[Mapsize.X * Mapsize.Y * Mapsize.Z];

            for (var x = 0; x < Mapsize.X; x++) {
                for (var y = 0; y < Mapsize.Y; y++) {
                    for (var z = 0; z < Mapsize.Z; z++) 
                        Blockdata[GetIndex(x, y, z)] = allData[GetBlock(x, y, z)];
                }
            }

            // -- Now, Block data will be properly oriented for use in ClassicWorld maps, and we have all the data we need to create a classicworld map.

            var cwMap = new Classicworld(Mapsize.X, Mapsize.Z, Mapsize.Y) {
                BlockData = Blockdata,
                SpawnX = Spawn.X,
                SpawnY = Spawn.Z,
                SpawnZ = Spawn.Y,
                SpawnRotation = SpawnRot,
                SpawnLook = SpawnLook,
                MapName = mapName
            }; // -- Classicworld is in notchian Coordinates.

            cwMap.Save("Maps/" + mapName + ".cw");
            Blockdata = null;
            cwMap.BlockData = null;
            GC.Collect();
            // -- Conversion Complete.
            File.Delete("Settings/TempD3Config.txt");
        }

        /// <summary>
        /// Retreives the index of block ID in a D3 v1+ array.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public int GetBlock(int x, int y, int z) {
            if ((x >= 0 && y >= 0 && z >= 0) && (Mapsize.X > x && Mapsize.Y > y && Mapsize.Z > z)) 
                return ((z * Mapsize.Y + y) * Mapsize.X + x) * 4;
             
            return 1;
        }

        /// <summary>
        /// Retreives the index for a notcian array.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public int GetIndex(int x, int z, int y) {
            return (y * Mapsize.Y + z) * Mapsize.X + x;
        }

        public void LoadConfig() {
            // -- just a placeholder
        }
    }
}
