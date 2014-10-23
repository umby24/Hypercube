using System.Diagnostics;

using Hypercube.Core;
using Hypercube.Map;

namespace Hypercube.Mapfills {
    static internal class DefaultFills {
        public static void Init(FillContainer container) {
            container.RegisterFill("Flatgrass", FFlatgrass);
            container.RegisterFill("White", FWhite);
            container.RegisterFill("Bedrock", FBedrock);
            container.RegisterFill("Wireworld", FWireworld);
        }

        #region Flatgrass
        static readonly Fill FFlatgrass = new Fill { Plugin = "", Run = FlatgrassHandler };

        static void FlatgrassHandler(HypercubeMap map, string[] args) {
            var sw = new Stopwatch();
            sw.Start();

            map.CWMap.BlockData = new byte[map.CWMap.BlockData.Length];

            var grassBlock = ServerCore.Blockholder.GetBlock(2);
            var dirtBlock = ServerCore.Blockholder.GetBlock(3);
            var airBlock = ServerCore.Blockholder.GetBlock(0);

            for (var x = 0; x < map.CWMap.SizeX; x++) {
                for (var y = 0; y < map.CWMap.SizeZ; y++) {
                    for (var z = 0; z < (map.CWMap.SizeY / 2); z++) {
                        map.BlockChange(-1, (short) x, (short) y, (short) z,
                            z == (map.CWMap.SizeY/2) - 1 ? grassBlock : dirtBlock, airBlock, false, false, false, 1);
                    }
                }
            }

            sw.Stop();
            Chat.SendMapChat(map, "&cMap created in " + ((sw.ElapsedMilliseconds / 1000F)) + "s.");
        }
        #endregion
        #region White
        static readonly Fill FWhite = new Fill { Plugin = "", Run = WhiteHandler };

        static void WhiteHandler(HypercubeMap map, string[] args) {
            map.CWMap.BlockData = new byte[map.CWMap.BlockData.Length];

            var airBlock = ServerCore.Blockholder.GetBlock(0);
            var whiteBlock = ServerCore.Blockholder.GetBlock(36);

            for (var ix = 0; ix < map.CWMap.SizeX; ix++) {
                for (var iy = 0; iy < map.CWMap.SizeY; iy++)
                    map.BlockChange(-1, (short)ix, (short)iy, 0, whiteBlock, airBlock, false, false, false, 1);
            }

            for (var ix = 0; ix < map.CWMap.SizeX; ix++) {
                for (var iz = 0; iz < map.CWMap.SizeZ / 2; iz++) {
                    map.BlockChange(-1, (short)ix, 0, (short)iz, whiteBlock, airBlock, false, false, false, 1);
                    map.BlockChange(-1, (short)ix, (short)(map.CWMap.SizeY - 1), (short)iz, whiteBlock, airBlock, false, false, false, 1);
                }
            }

            for (var iy = 0; iy < map.CWMap.SizeY; iy++) {
                for (var iz = 0; iz < map.CWMap.SizeZ / 2; iz++) {
                    map.BlockChange(-1, 0, (short)iy, (short)iz, whiteBlock, airBlock, false, false, false, 1);
                    map.BlockChange(-1, (short)(map.CWMap.SizeX - 1), (short)iy, (short)iz, whiteBlock, airBlock, false, false, false, 1);
                }
            }

            Chat.SendMapChat(map, "&cMap done.");
        }
        #endregion
        #region Bedrock
        static readonly Fill FBedrock = new Fill { Plugin = "", Run = BedrockHandler };

        static void BedrockHandler(HypercubeMap map, string[] args) {
            map.CWMap.BlockData = new byte[map.CWMap.BlockData.Length];

            var airBlock = ServerCore.Blockholder.GetBlock(0);
            var bedrock = ServerCore.Blockholder.GetBlock(7);

            for (var ix = 0; ix < map.CWMap.SizeX; ix++) {
                for (var iy = 0; iy < map.CWMap.SizeY; iy++)
                    map.BlockChange(-1, (short)ix, (short)iy, 0, bedrock, airBlock, false, false, false, 1);
            }

            for (var ix = 0; ix < map.CWMap.SizeX; ix++) {
                for (var iz = 0; iz < map.CWMap.SizeZ / 2; iz++) {
                    map.BlockChange(-1, (short)ix, 0, (short)iz, bedrock, airBlock, false, false, false, 1);
                    map.BlockChange(-1, (short)ix, (short)(map.CWMap.SizeY - 1), (short)iz, bedrock, airBlock, false, false, false, 1);
                }
            }

            for (var iy = 0; iy < map.CWMap.SizeY; iy++) {
                for (var iz = 0; iz < map.CWMap.SizeZ / 2; iz++) {
                    map.BlockChange(-1, 0, (short)iy, (short)iz, bedrock, airBlock, false, false, false, 1);
                    map.BlockChange(-1, (short)(map.CWMap.SizeX - 1), (short)iy, (short)iz, bedrock, airBlock, false, false, false, 1);
                }
            }

            Chat.SendMapChat(map, "&cMap done.");
        }
        #endregion
        #region Wireworld
        static readonly Fill FWireworld = new Fill { Plugin = "", Run = WireworldHandler };

        static void WireworldHandler(HypercubeMap map, string[] args) {
            map.CWMap.BlockData = new byte[map.CWMap.BlockData.Length];

            var airBlock = ServerCore.Blockholder.GetBlock(0);
            var blackBlock = ServerCore.Blockholder.GetBlock(34);

            for (var x = 0; x < map.CWMap.SizeX; x++) {
                for (var y = 0; y < map.CWMap.SizeY; y++)
                    map.BlockChange(-1, (short)x, (short)y, 0, blackBlock, airBlock, false, false, false, 1);
            }

            Chat.SendMapChat(map, "&cMap done.");
        }
        #endregion
    }
}
