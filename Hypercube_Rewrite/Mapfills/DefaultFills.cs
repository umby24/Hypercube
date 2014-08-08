using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Hypercube.Core;
using Hypercube.Map;

namespace Hypercube.Mapfills {
    static internal class DefaultFills {
        public static void Init(FillContainer container) {
            container.RegisterFill("Flatgrass", fFlatgrass);
            container.RegisterFill("White", fWhite);
            container.RegisterFill("Bedrock", fBedrock);
            container.RegisterFill("Wireworld", fWireworld);
        }

        #region Flatgrass
        static readonly Fill fFlatgrass = new Fill { Plugin = "", Run = FlatgrassHandler };

        static void FlatgrassHandler(HypercubeMap map, string[] args) {
            var SW = new Stopwatch();
            SW.Start();

            map.CWMap.BlockData = new byte[map.CWMap.BlockData.Length];

            var GrassBlock = map.Servercore.Blockholder.GetBlock(2);
            var DirtBlock = map.Servercore.Blockholder.GetBlock(3);
            var AirBlock = map.Servercore.Blockholder.GetBlock(0);

            for (var X = 0; X < map.CWMap.SizeX; X++) {
                for (var Y = 0; Y < map.CWMap.SizeZ; Y++) {
                    for (var Z = 0; Z < (map.CWMap.SizeY / 2); Z++) {
                        if (Z == (map.CWMap.SizeY / 2) - 1)
                            map.BlockChange(-1, (short)X, (short)Y, (short)Z, GrassBlock, AirBlock, false, false, false, 1);
                        else
                            map.BlockChange(-1, (short)X, (short)Y, (short)Z, DirtBlock, AirBlock, false, false, false, 1);
                    }
                }
            }

            SW.Stop();
            Chat.SendMapChat(map, map.Servercore, "&cMap created in " + ((float)(SW.ElapsedMilliseconds / 1000F)).ToString() + "s.");
        }
        #endregion
        #region White
        static readonly Fill fWhite = new Fill { Plugin = "", Run = WhiteHandler };

        static void WhiteHandler(HypercubeMap map, string[] args) {
            map.CWMap.BlockData = new byte[map.CWMap.BlockData.Length];

            var AirBlock = map.Servercore.Blockholder.GetBlock(0);
            var WhiteBlock = map.Servercore.Blockholder.GetBlock(36);

            for (var ix = 0; ix < map.CWMap.SizeX; ix++) {
                for (var iy = 0; iy < map.CWMap.SizeY; iy++)
                    map.BlockChange(-1, (short)ix, (short)iy, 0, WhiteBlock, AirBlock, false, false, false, 1);
            }

            for (var ix = 0; ix < map.CWMap.SizeX; ix++) {
                for (var iz = 0; iz < map.CWMap.SizeZ / 2; iz++) {
                    map.BlockChange(-1, (short)ix, 0, (short)iz, WhiteBlock, AirBlock, false, false, false, 1);
                    map.BlockChange(-1, (short)ix, (short)(map.CWMap.SizeY - 1), (short)iz, WhiteBlock, AirBlock, false, false, false, 1);
                }
            }

            for (var iy = 0; iy < map.CWMap.SizeY; iy++) {
                for (var iz = 0; iz < map.CWMap.SizeZ / 2; iz++) {
                    map.BlockChange(-1, 0, (short)iy, (short)iz, WhiteBlock, AirBlock, false, false, false, 1);
                    map.BlockChange(-1, (short)(map.CWMap.SizeX - 1), (short)iy, (short)iz, WhiteBlock, AirBlock, false, false, false, 1);
                }
            }

            Chat.SendMapChat(map, map.Servercore, "&cMap done.");
        }
        #endregion
        #region Bedrock
        static readonly Fill fBedrock = new Fill { Plugin = "", Run = BedrockHandler };

        static void BedrockHandler(HypercubeMap map, string[] args) {
            map.CWMap.BlockData = new byte[map.CWMap.BlockData.Length];

            var AirBlock = map.Servercore.Blockholder.GetBlock(0);
            var Bedrock = map.Servercore.Blockholder.GetBlock(7);

            for (var ix = 0; ix < map.CWMap.SizeX; ix++) {
                for (var iy = 0; iy < map.CWMap.SizeY; iy++)
                    map.BlockChange(-1, (short)ix, (short)iy, 0, Bedrock, AirBlock, false, false, false, 1);
            }

            for (var ix = 0; ix < map.CWMap.SizeX; ix++) {
                for (var iz = 0; iz < map.CWMap.SizeZ / 2; iz++) {
                    map.BlockChange(-1, (short)ix, 0, (short)iz, Bedrock, AirBlock, false, false, false, 1);
                    map.BlockChange(-1, (short)ix, (short)(map.CWMap.SizeY - 1), (short)iz, Bedrock, AirBlock, false, false, false, 1);
                }
            }

            for (var iy = 0; iy < map.CWMap.SizeY; iy++) {
                for (var iz = 0; iz < map.CWMap.SizeZ / 2; iz++) {
                    map.BlockChange(-1, 0, (short)iy, (short)iz, Bedrock, AirBlock, false, false, false, 1);
                    map.BlockChange(-1, (short)(map.CWMap.SizeX - 1), (short)iy, (short)iz, Bedrock, AirBlock, false, false, false, 1);
                }
            }

            Chat.SendMapChat(map, map.Servercore, "&cMap done.");
        }
        #endregion
        #region Wireworld
        static readonly Fill fWireworld = new Fill { Plugin = "", Run = WireworldHandler };

        static void WireworldHandler(HypercubeMap map, string[] args) {
            map.CWMap.BlockData = new byte[map.CWMap.BlockData.Length];

            var AirBlock = map.Servercore.Blockholder.GetBlock(0);
            var BlackBlock = map.Servercore.Blockholder.GetBlock(34);

            for (var x = 0; x < map.CWMap.SizeX; x++) {
                for (var y = 0; y < map.CWMap.SizeY; y++)
                    map.BlockChange(-1, (short)x, (short)y, 0, BlackBlock, AirBlock, false, false, false, 1);
            }

            Chat.SendMapChat(map, map.Servercore, "&cMap done.");
        }
        #endregion
    }
}
