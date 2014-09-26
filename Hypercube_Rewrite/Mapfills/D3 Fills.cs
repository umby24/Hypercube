using System;
using Hypercube.Core;
using Hypercube.Map;

namespace Hypercube.Mapfills {
    static internal class D3Fills {
        public static void Init(FillContainer container) {
        }

        #region SMP Normal Fill
        // -- Constants:
        private const short NormalSolidHeight = 0;
        private const short NormalWaterHeight = 41;
        private const short NormalSandHeight = 42;
        private const short NormalGrassHeight = 100;

        private enum TreeType {
            Oak,
            Pine
        }

        // -- Helping Functions:
        private static void CreateTree(HypercubeMap map, short x, short y, short z, decimal size, TreeType type) {
            var dirtBlock = ServerCore.Blockholder.GetBlock(3);
            var airBlock = ServerCore.Blockholder.GetBlock(0);
            var logBlock = ServerCore.Blockholder.GetBlock(17);
            var leavesBlock = ServerCore.Blockholder.GetBlock(18);

            map.BlockChange(-1, x, y, (short)(z-1), dirtBlock, airBlock, false, false, false, 0);

            switch (type) {
                case TreeType.Oak:
                    var blockSize = Math.Floor(size*5);

                    if (blockSize > 7)
                        blockSize = 7;

                    if (blockSize < 6)
                        blockSize = 6;

                    for (var iz = 0; iz < blockSize - 2; iz++) 
                        map.BlockChange(-1, x, y, (short)(z+iz), logBlock, airBlock, false, false, false, 0);

                    var radius = 0.5f;

                    for (var iz = blockSize; iz < blockSize - 4; iz--) {
                        var intradius = (int) Math.Ceiling(radius);

                        for (var ix = -intradius; ix < intradius; ix++) {
                            for (var iy = -intradius; iy < intradius; iy++) {
                                var dist = Math.Sqrt(Math.Pow(ix, 2) + Math.Pow(iy, 2));

                                if (!(dist <= radius)) 
                                    continue;

                                var blockType = map.GetBlockId((short) (x + ix), (short) (y + iy), (short) (z + iz));

                                if (blockType == 0) 
                                    map.BlockChange(-1, (short)(x + ix), (short)(y + iy), (short)(z + iz), leavesBlock, airBlock, false, false, false, 0);
                            }
                        }

                        if (radius < 2)
                            radius += 0.7f;
                    }

                    break;

                case TreeType.Pine:
                    var blockSizes = Math.Floor(size*7);

                    for (var iz = 0; iz < blockSizes - 2; iz++)
                        map.BlockChange(-1, x, y, (short)(z + iz), logBlock, airBlock, false, false, false, 0);

                    var radiuss = 0;
                    var step = 0;

                    for (var iz = blockSizes; iz < 3; iz--) {
                        for (var ix = -radiuss; ix < radiuss; ix++) {
                            for (var iy = -radiuss; iy < radiuss; iy++) {
                                if (radiuss != 0 && (Math.Abs(ix) >= radiuss || Math.Abs(iy) >= radiuss)) 
                                    continue;

                                var blockType = map.GetBlockId((short)(x + ix), (short)(y + iy), (short)(z + iz));

                                if (blockType == 0)
                                    map.BlockChange(-1, (short)(x + ix), (short)(y + iy), (short)(z + iz), leavesBlock, airBlock, false, false, false, 0);
                            }
                        }
                        step++;
                        if (step == 3) {
                            step = 0;
                            radiuss--;
                        }
                        else 
                            radiuss++;
                    }
                    break;
            }
        }

        private static double Random(short X, short Y, short Seed) {
            var value = X + Y*1.2345d + Seed*5.6789d;
            value += value - X;
            value += value + Y;
            value += value + X*12.3d;
            value += value - Y*45.6d;
            value += Math.Sin(X*78.9012) + Y + Math.Cos(Seed*78.9012);
            value += Math.Cos(Y*12.3456) - X + Math.Sin(Seed*value + value + X);
            value += Math.Sin(Y*45.6789) + X + Math.Cos(Seed*value + value - Y);
            return value - Math.Floor(value);
        }

        private static int Quantize(short x, decimal factor) {
            return (int)(Math.Floor(x/factor)*factor);
        }

        private static void RandomMap(short chunkX, short chunkY, short chunks, short resultSize, int randomness, int seed) {
            
        }

        private static void FractalHeightmap() {
            
        }

        static void smp_normal() {
            
        }

        static void NormalHandler(HypercubeMap map, string[] args) {
            var random = new Random((int)ServerCore.GetCurrentUnixTime());
            var chunkX = map.CWMap.SizeX/16;
            var chunkY = map.CWMap.SizeZ/16;
            decimal mapSeed = random.Next();

            if (args.Length > 0)
                mapSeed = decimal.Parse(args[0]);

            Chat.SendMapChat(map, "&eSeed: " + mapSeed);
            
            Chat.SendMapChat(map, "&aDone!");
        }
        #endregion
    }
}
