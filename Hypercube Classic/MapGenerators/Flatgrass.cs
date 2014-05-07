using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Hypercube_Classic.Map;
using Hypercube_Classic.Core;

namespace Hypercube_Classic.MapGenerators {
    public struct FlatgrassFill : IMapFill {
        public string Name { get; set; }
        public string Script { get; set; }
        public object GenerateNew { get; set; }
        public object GenerateExisting { get; set; }
    }

    public class Flatgrass {
        
        /// <summary>
        /// Create a new map that is flatgrass.
        /// </summary>
        /// <param name="SizeX"></param>
        /// <param name="SizeY"></param>
        /// <param name="SizeZ"></param>
        /// <returns></returns>
        public static HypercubeMap GenerateFlatGrassNew(Hypercube Core, string Name, short SizeX, short SizeY, short SizeZ) {
            var SW = new Stopwatch();
            SW.Start();

            var MyMap = new HypercubeMap(Core, Name, Name, SizeX, SizeY, SizeZ);
            
            for (int X = 0; X < SizeX; X++) {
                for (int Y = 0; Y < SizeY; Y++) {
                    for (int Z = 0; Z < (SizeZ / 2); Z++) {
                        if (Z == (SizeZ / 2) - 1) 
                            MyMap.BlockChange(-1, (short)X, (short)Y, (short)Z, Core.Blockholder.GetBlock(2), Core.Blockholder.GetBlock(0), false, false, false, 1);
                         else 
                            MyMap.BlockChange(-1, (short)X, (short)Y, (short)Z, Core.Blockholder.GetBlock(3), Core.Blockholder.GetBlock(0), false, false, false, 1);
                    }
                }
            }

            SW.Stop();
            Chat.SendMapChat(MyMap, Core, "&cMap created in " + (SW.ElapsedMilliseconds / 1000).ToString() + "s.");
            return MyMap;
        }

        /// <summary>
        /// Generate flatgrass on an existing map.
        /// </summary>
        /// <param name="Map"></param>
        /// <returns></returns>
        public static HypercubeMap GenerateFlatGrass(HypercubeMap Map) {
            var SW = new Stopwatch();
            SW.Start();

            Map.Map.BlockData = new byte[Map.Map.BlockData.Length];

            for (int X = 0; X < Map.Map.SizeX; X++) {
                for (int Y = 0; Y < Map.Map.SizeY; Y++) {
                    for (int Z = 0; Z < (Map.Map.SizeZ / 2); Z++) {
                        if (Z == (Map.Map.SizeZ / 2) - 1) 
                            Map.BlockChange(-1, (short)X, (short)Y, (short)Z, Map.ServerCore.Blockholder.GetBlock(2), Map.ServerCore.Blockholder.GetBlock(0), false, false, false, 1);
                         else 
                            Map.BlockChange(-1, (short)X, (short)Y, (short)Z, Map.ServerCore.Blockholder.GetBlock(3), Map.ServerCore.Blockholder.GetBlock(0), false, false, false, 1);
                    }
                }
            }

            SW.Stop();
            Chat.SendMapChat(Map, Map.ServerCore, "&cMap created in " + ((float)(SW.ElapsedMilliseconds / 1000F)).ToString() + "s.");
            return Map;
        }
    }
}
