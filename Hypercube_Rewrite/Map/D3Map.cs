using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Hypercube.Core;
using Hypercube.Libraries;

namespace Hypercube.Map {
    public class D3Map {
        #region Variables
        public byte[] Blockdata;
        public Vector3S Mapsize, Spawn;
        public byte SpawnRot, SpawnLook;
        public bool PhysicsStopped;
        public string MOTD;

        Hypercube Servercore;
        #endregion

        public D3Map(Hypercube Core) {
            Servercore = Core;
        }

        public void LoadMap(string Directory, string MapName) {
            if (!File.Exists(Directory + "/Data-Layer.gz") || !File.Exists(Directory + "/Config.txt"))
                return; // -- Not a valid map.
            File.Copy(Directory + "/Config.txt", "Settings/TempD3Config.txt");

            // -- Load the Config data first..
            var Configfile = Servercore.Settings.RegisterFile("TempD3Config.txt", false, new PBSettingsLoader.LoadSettings(LoadConfig));
            Servercore.Settings.ReadSettings(Configfile);

            Mapsize = new Vector3S();
            Mapsize.X = (short)Servercore.Settings.ReadSetting(Configfile, "Size_X", 128);
            Mapsize.Y = (short)Servercore.Settings.ReadSetting(Configfile, "Size_Y", 128);
            Mapsize.Z = (short)Servercore.Settings.ReadSetting(Configfile, "Size_Z", 128);

            Spawn = new Vector3S();
            Spawn.X = (short)float.Parse(Servercore.Settings.ReadSetting(Configfile, "Spawn_X", "1.0"));
            Spawn.Y = (short)float.Parse(Servercore.Settings.ReadSetting(Configfile, "Spawn_Y", "1.0"));
            Spawn.Z = (short)float.Parse(Servercore.Settings.ReadSetting(Configfile, "Spawn_Z", "1.0"));

            SpawnRot = (byte)Servercore.Settings.ReadSetting(Configfile, "Spawn_Rot", 0);
            SpawnLook = (byte)Servercore.Settings.ReadSetting(Configfile, "Spawn_Look", 0);

            MOTD = Servercore.Settings.ReadSetting(Configfile, "MOTD_Override", "");
            PhysicsStopped = Convert.ToBoolean(Servercore.Settings.ReadSetting(Configfile, "Physic_Stopped", 0));

            // -- Load the block data
            GZip.DecompressFile(Directory + "/Data-Layer.gz");
            byte[] AllData;

            using (var BR = new BinaryReader(new FileStream(Directory + "/Data-Layer.gz", FileMode.Open))) 
                AllData = BR.ReadBytes((int)BR.BaseStream.Length);

            GZip.CompressFile(Directory + "/Data-Layer.gz");

            if (AllData.Length != (Mapsize.X * Mapsize.Y * Mapsize.Z) * 4) {
                // -- Size error..
                AllData = null;
                return;
            }

            Blockdata = new byte[Mapsize.X * Mapsize.Y * Mapsize.Z];

            for (int x = 0; x < Mapsize.X; x++) {
                for (int y = 0; y < Mapsize.Y; y++) {
                    for (int z = 0; z < Mapsize.Z; z++) 
                        Blockdata[GetIndex(x, y, z)] = AllData[GetBlock(x, y, z)];
                }
            }

            AllData = null;
            // -- Now, Block data will be properly oriented for use in ClassicWorld maps, and we have all the data we need to create a classicworld map.

            var CWMap = new ClassicWorld_NET.ClassicWorld(Mapsize.X, Mapsize.Z, Mapsize.Y); // -- Classicworld is in notchian Coordinates.
            CWMap.BlockData = Blockdata;
            CWMap.SpawnX = Spawn.X;
            CWMap.SpawnY = Spawn.Z;
            CWMap.SpawnZ = Spawn.Y;
            CWMap.SpawnRotation = SpawnRot;
            CWMap.SpawnLook = SpawnLook;
            CWMap.MapName = MapName;
            CWMap.Save("Maps/" + MapName + ".cw");
            Blockdata = null;
            CWMap.BlockData = null;
            CWMap = null;
            GC.Collect();
            // -- Conversion Complete.
            File.Delete("Settings/TempD3Config.txt");
        }

        /// <summary>
        /// Retreives the index of block ID in a D3 v1+ array.
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        public int GetBlock(int X, int Y, int Z) {
            if ((X >= 0 && Y >= 0 && Z >= 0) && (Mapsize.X > X && Mapsize.Y > Y && Mapsize.Z > Z)) 
                return ((Z * Mapsize.Y + Y) * Mapsize.X + X) * 4;
             else 
                return 1;
        }

        /// <summary>
        /// Retreives the index for a notcian array.
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        public int GetIndex(int X, int Z, int Y) {
            return (Y * Mapsize.Y + Z) * Mapsize.X + X;
        }

        public void LoadConfig() {
            // -- just a placeholder
        }
    }
}
