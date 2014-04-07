using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression; // -- For GZip

namespace Hypercube_Classic.Map {
    class D3_Map {
        // -- D3 V1.0+ Map reader
        // -- Map size, spawn coords, and so on are stored in 'Config.txt'.
        // -- Actual block data is stored in Data-Layer.gz, which is in the format [byte]BlockID [byte]Metadata [short]Player 

        public int Size_X, Size_Y, Size_Z;
        public int SpawnX, SpawnY, SpawnZ;
        public byte SpawnRot, SpawnLook;
        public string MOTD;
        public short BuildRank, JoinRank, ShowRank, SideLevel;
        public short[] SkyColor;
        public short[] CloudColor;
        public short[] FogColor;
        public short[] AmbientColor;
        public short[] SunlightColor;
        public string TextureURL;
        public byte SideBlock, EdgeBlock;
        public byte[] mapData;
        public byte[] UUID; // -- In d3 this is a string normally..

        /// <summary>
        /// Loads the Config.txt file for a D3 map.
        /// </summary>
        /// <param name="fileName"></param>
        public void readConfig(string fileName) {
            StreamReader fileReader = new StreamReader(fileName);

            do {
                string line = fileReader.ReadLine();

                if (!line.Contains("="))
                    continue;
                string setting = line.Substring(0, line.IndexOf("=") - 1);
                string value = line.Substring(line.IndexOf("=") + 2, line.Length - (line.IndexOf("=") + 2));

                switch (setting) {
                    case "Unique_ID":
                        UUID = Encoding.ASCII.GetBytes(value);
                        break;

                    case "MOTD_Override":
                        MOTD = value;
                        break;

                    case "Rank_Build":
                        BuildRank = short.Parse(value);
                        break;

                    case "Rank_Show":
                        ShowRank = short.Parse(value);
                        break;

                    case "Rank_Join":
                        JoinRank = short.Parse(value);
                        break;

                    case "Size_X":
                        Size_X = int.Parse(value);
                        break;

                    case "Size_Y":
                        Size_Y = int.Parse(value);
                        break;

                    case "Size_Z":
                        Size_Z = int.Parse(value);
                        break;

                    case "Spawn_X":
                        SpawnX = int.Parse(value);
                        break;

                    case "Spawn_Y":
                        SpawnY = int.Parse(value);
                        break;

                    case "Spawn_Z":
                        SpawnZ = int.Parse(value);
                        break;

                    case "Spawn_Rot":
                        SpawnRot = (byte)int.Parse(value);
                        break;

                    case "Spawn_Look":
                        SpawnLook = (byte)int.Parse(value);
                        break;

                    case "Sky_Color":
                        if (int.Parse(value) != -1) {
                            string hexValue = int.Parse(value).ToString("X");
                            hexValue = hexValue.PadLeft(6, '0');
                            hexValue = hexValue.Substring(4, 2) + hexValue.Substring(2, 2) + hexValue.Substring(0, 2);
                            
                            // -- R, G, B.
                            SkyColor = new short[] { short.Parse(hexValue.Substring(0, 2)), short.Parse(hexValue.Substring(2, 2)), short.Parse(hexValue.Substring(4, 2)) };
                        }
                        break;

                    case "Cloud_Color":
                        if (int.Parse(value) != -1) {
                            string hexValue = int.Parse(value).ToString("X");
                            hexValue = hexValue.PadLeft(6, '0');
                            hexValue = hexValue.Substring(4, 2) + hexValue.Substring(2, 2) + hexValue.Substring(0, 2);

                            // -- R, G, B.
                            CloudColor = new short[] { short.Parse(hexValue.Substring(0, 2)), short.Parse(hexValue.Substring(2, 2)), short.Parse(hexValue.Substring(4, 2)) };
                        }
                        break;

                    case "Fog_Color":
                        if (int.Parse(value) != -1) {
                            string hexValue = int.Parse(value).ToString("X");
                            hexValue = hexValue.PadLeft(6, '0');
                            hexValue = hexValue.Substring(4, 2) + hexValue.Substring(2, 2) + hexValue.Substring(0, 2);

                            // -- R, G, B.
                            FogColor = new short[] { short.Parse(hexValue.Substring(0, 2)), short.Parse(hexValue.Substring(2, 2)), short.Parse(hexValue.Substring(4, 2)) };
                        }
                        break;

                    case "A_Light":
                        if (int.Parse(value) != -1) {
                            string hexValue = int.Parse(value).ToString("X");
                            hexValue = hexValue.PadLeft(6, '0');
                            hexValue = hexValue.Substring(4, 2) + hexValue.Substring(2, 2) + hexValue.Substring(0, 2);

                            // -- R, G, B.
                            AmbientColor = new short[] { short.Parse(hexValue.Substring(0, 2)), short.Parse(hexValue.Substring(2, 2)), short.Parse(hexValue.Substring(4, 2)) };
                        }
                        break;

                    case "D_Light":
                        if (int.Parse(value) != -1) {
                            string hexValue = int.Parse(value).ToString("X");
                            hexValue = hexValue.PadLeft(6, '0');
                            hexValue = hexValue.Substring(4, 2) + hexValue.Substring(2, 2) + hexValue.Substring(0, 2);

                            // -- R, G, B.
                            SunlightColor = new short[] { short.Parse(hexValue.Substring(0, 2)), short.Parse(hexValue.Substring(2, 2)), short.Parse(hexValue.Substring(4, 2)) };
                        }
                        break;
                    case "Custom_Texture_Url":
                        TextureURL = value;
                        break;
                    case "Custom_Side_Block":
                        SideBlock = (byte)int.Parse(value);
                        break;
                    case "Custom_Edge_Block":
                        EdgeBlock = (byte)int.Parse(value);
                        break;
                    case "Custom_Side_Level":
                        SideLevel = short.Parse(value);
                        break;
                    default:
                        continue;
                }
               
            } while (!fileReader.EndOfStream);
            fileReader.Close();
        }

        /// <summary>
        /// Gets a block from the D3 Data array.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns>The blockID at the requested coordinates.</returns>
        public int getBlockID(int x, int y, int z) {
            if ((x >= 0 && y >= 0 && z >= 0) && (Size_X > x && Size_Y > y && Size_Z > z)) {
                int index = (z * Size_Y + y) * Size_X + x; // -- (Y * Size_Z + Z) * Size_X + x
                return mapData[(index * 4)];
            } else {
                return 1;
            }
        }

    }
}
