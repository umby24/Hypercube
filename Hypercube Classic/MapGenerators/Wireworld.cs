using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hypercube_Classic.Map;
using Hypercube_Classic.Core;

namespace Hypercube_Classic.MapGenerators {
    public class Wireworld {
        public static HypercubeMap GenerateWhiteNew(Hypercube Core, string Name, short SizeX, short SizeY, short SizeZ) {
            var MyNewMap = new HypercubeMap(Core, "Maps/" + Name + ".cw", Name, SizeX, SizeY, SizeZ);
            return GenerateWhite(MyNewMap);
        }

        public static HypercubeMap GenerateWhite(HypercubeMap Map) {
            Map.Map.BlockData = new byte[Map.Map.BlockData.Length];

            for (int x = 0; x < Map.Map.SizeX; x++) {
                for (int y = 0; y < Map.Map.SizeY; y++) 
                    Map.BlockChange(-1, (short)x, (short)y, 0, Map.ServerCore.Blockholder.GetBlock(34), Map.ServerCore.Blockholder.GetBlock(0), false, false, false, 1);
            }

            return Map;
        }
    }
}
