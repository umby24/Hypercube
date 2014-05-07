using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hypercube_Classic.Map;
using Hypercube_Classic.Core;

namespace Hypercube_Classic.MapGenerators {
    public struct WhiteMapFill : IMapFill {
        public string Name { get; set; }
        public string Script { get; set; }
        public object GenerateNew { get; set; }
        public object GenerateExisting { get; set; }
    }

    public class White {
        public static HypercubeMap GenerateWhiteNew(Hypercube Core, string Name, short SizeX, short SizeY, short SizeZ) {
            var MyMap = new HypercubeMap(Core, Name, Name, SizeX, SizeY, SizeZ);

            for (int ix = 0; ix < SizeX - 1; ix++) {
                for (int iy = 0; iy < SizeY - 1; iy++) 
                    MyMap.BlockChange(-1, (short)ix, (short)iy, 0, Core.Blockholder.GetBlock(36), Core.Blockholder.GetBlock(0), false, false, false, 1);
            }

            for (int ix = 0; ix < SizeX - 1; ix++) {
                for (int iz = 0; iz < SizeZ / 2 - 1; iz++) {
                    MyMap.BlockChange(-1, (short)ix, 0, (short)iz, Core.Blockholder.GetBlock(36), Core.Blockholder.GetBlock(0), false, false, false, 1);
                    MyMap.BlockChange(-1, (short)ix, (short)(SizeY - 1), (short)iz, Core.Blockholder.GetBlock(36), Core.Blockholder.GetBlock(0), false, false, false, 1);
                }
            }

            for (int iy = 0; iy < SizeY - 1; iy++) {
                for (int iz = 0; iz < SizeZ / 2 - 1; iz++) {
                    MyMap.BlockChange(-1, 0, (short)iy, (short)iz, Core.Blockholder.GetBlock(36), Core.Blockholder.GetBlock(0), false, false, false, 1);
                    MyMap.BlockChange(-1, (short)(SizeX - 1), (short)iy, (short)iz, Core.Blockholder.GetBlock(36), Core.Blockholder.GetBlock(0), false, false, false, 1);
                }
            }

            return MyMap;
        }

        public static HypercubeMap GenerateWhite(HypercubeMap Map) {
            Map.Map.BlockData = new byte[Map.Map.BlockData.Length];

            for (int ix = 0; ix < Map.Map.SizeX - 1; ix++) {
                for (int iy = 0; iy < Map.Map.SizeY - 1; iy++)
                    Map.BlockChange(-1, (short)ix, (short)iy, 0, Map.ServerCore.Blockholder.GetBlock(36), Map.ServerCore.Blockholder.GetBlock(0), false, false, false, 1);
            }

            for (int ix = 0; ix < Map.Map.SizeX - 1; ix++) {
                for (int iz = 0; iz < Map.Map.SizeZ / 2 - 1; iz++) {
                    Map.BlockChange(-1, (short)ix, 0, (short)iz, Map.ServerCore.Blockholder.GetBlock(36), Map.ServerCore.Blockholder.GetBlock(0), false, false, false, 1);
                    Map.BlockChange(-1, (short)ix, (short)(Map.Map.SizeY - 1), (short)iz, Map.ServerCore.Blockholder.GetBlock(36), Map.ServerCore.Blockholder.GetBlock(0), false, false, false, 1);
                }
            }

            for (int iy = 0; iy < Map.Map.SizeY - 1; iy++) {
                for (int iz = 0; iz < Map.Map.SizeZ / 2 - 1; iz++) {
                    Map.BlockChange(-1, 0, (short)iy, (short)iz, Map.ServerCore.Blockholder.GetBlock(36), Map.ServerCore.Blockholder.GetBlock(0), false, false, false, 1);
                    Map.BlockChange(-1, (short)(Map.Map.SizeX - 1), (short)iy, (short)iz, Map.ServerCore.Blockholder.GetBlock(36), Map.ServerCore.Blockholder.GetBlock(0), false, false, false, 1);
                }
            }

            Chat.SendMapChat(Map, Map.ServerCore, "&cMap Done.");
            return Map;
        }
    }
}
