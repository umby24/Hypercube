using System;
using Hypercube.Client;
using Hypercube.Core;
using Hypercube.Map;

namespace Hypercube.Command {
    internal static class Buildmodes {
        /// <summary>
        /// Initiates and loads all internal buildmodes. 
        /// </summary>
        public static void Init() {
            ServerCore.BmContainer.Modes.Add("Box", BoxStruct);
            ServerCore.BmContainer.Modes.Add("CreateTP", CreateTpStruct);
            ServerCore.BmContainer.Modes.Add("History", HistoryStruct);
        }

        #region Box

        private static readonly BmStruct BoxStruct = new BmStruct {
            Function = BoxHandler,
            Name = "Box",
            Plugin = "",
        };

        static void BoxHandler(NetworkClient client, HypercubeMap map, Vector3S location, byte mode, Block block) {
            if (mode != 1)
                return;

            switch (client.CS.MyEntity.BuildState) {
                case 0:
                    client.CS.MyEntity.ClientState.SetCoord(location, 0);
                    client.CS.MyEntity.BuildState = 1;
                    break;
                case 1:
                    var coord1 = client.CS.MyEntity.ClientState.GetCoord(0);
                    var blocks = Math.Abs(location.X - coord1.X)*Math.Abs(location.Y - coord1.Y)*
                                 Math.Abs(location.Z - coord1.Z);
                    var replaceBlock = client.CS.MyEntity.ClientState.GetString(0);

                    if (blocks < 50000) {
                        map.BuildBox(client, coord1.X, coord1.Y, coord1.Z, location.X, location.Y, location.Z, block,
                            String.IsNullOrEmpty(replaceBlock)
                                ? ServerCore.Blockholder.UnknownBlock
                                : ServerCore.Blockholder.GetBlock(replaceBlock), false, 1, true, false);

                        Chat.SendClientChat(client, "§SBox created.");
                    }
                    else 
                        Chat.SendClientChat(client, "§EBox too large.");
                    

                    client.CS.MyEntity.SetBuildmode("");
                    break;
            }
        }
        #endregion
        #region CreateTP

        private static readonly BmStruct CreateTpStruct = new BmStruct {
            Function = CreateTPHandler,
            Name = "CreateTP",
            Plugin = "",
        };

        static void CreateTPHandler(NetworkClient client, HypercubeMap map, Vector3S location, byte mode, Block block) {
            if (mode != 1)
                return;

            var state = client.CS.MyEntity.BuildState;

            switch (state) {
                case 0:
                    client.CS.MyEntity.ClientState.SetCoord(location.X, location.Y, location.Z, 1);
                    client.CS.MyEntity.BuildState = 1;
                    return;
                case 1:
                    var destCoord = client.CS.MyEntity.ClientState.GetCoord(0);
                    var destRot = client.CS.MyEntity.ClientState.GetInt(0);
                    var destLook = client.CS.MyEntity.ClientState.GetInt(1);
                    var startCoord = client.CS.MyEntity.ClientState.GetCoord(1);
                    var endCoord = new Vector3S { X = location.X, Y = location.Y, Z = location.Z };
                    var teleName = client.CS.MyEntity.ClientState.GetString(0);
                    var destMap = HypercubeMap.GetMap(client.CS.MyEntity.ClientState.GetString(1));

                    // -- Move things around so the smaller is the start, the larger being the end.
                    if (startCoord.X > location.X) {
                        endCoord.X = startCoord.X;
                        startCoord.X = location.X;
                    }

                    if (startCoord.Y > location.Y) {
                        endCoord.Y = startCoord.Y;
                        startCoord.Y = location.Y;
                    }

                    if (startCoord.Z > location.Z) {
                        endCoord.Z = startCoord.Z;
                        startCoord.Z = location.Z;
                    }

                    client.CS.CurrentMap.Teleporters.CreateTeleporter(teleName, startCoord, endCoord, destCoord, (byte)destLook, (byte)destRot, destMap);
                    Chat.SendClientChat(client, "§STeleporter created.");
                    client.CS.MyEntity.SetBuildmode("");
                    break;
            }
        }
        #endregion
        #region History
        private static readonly BmStruct HistoryStruct = new BmStruct {
            Function = HistoryHandler,
            Name = "History",
            Plugin = "",
        };

        private static void HistoryHandler(NetworkClient client, HypercubeMap map, Vector3S location, byte mode, Block block) {
            Chat.SendClientChat(client,
                map.HCSettings.History
                    ? map.History.LookupString(location.X, location.Y, location.Z)
                    : "§EHistory is not enabled on this map.");

            client.CS.MyEntity.SetBuildmode("");
        }

        #endregion
        #region Line
        #endregion
        #region Sphere
        #endregion
    }
}
