using Hypercube.Client;
using Hypercube.Core;

namespace Hypercube.Network {
    public class CPE {
        // -- General information for CPE.
        public const short SupportedExtensions = 11;
        public const byte CustomBlocksSupportLevel = 1;

        // -- Individual extension versions.
        public const short CustomBlocksVersion = 1;
        public const short EmoteFixVersion = 1;
        public const short HeldBlockVersion = 1;
        public const short ClickDistanceVersion = 1;
        public const short ChangeModelVersion = 1;
        public const short ExtPlayerListVersion = 1;
        public const short EnvWeatherTypeVersion = 1;
        public const short EnvMapAppearanceVersion = 1;
        public const short MessageTypesVersion = 1;
        public const short BlockPermissionsVersion = 1;
        public const short TextHotKeyVersion = 1;
        public const short HackControlVersion = 1;
        public const short SelectionCuboidVersion = 1;
        public const short EnvColorsVersion = 1;

        /// <summary>
        /// Sends all server supported extensions to the client.
        /// </summary>
        /// <param name="client"></param>
        public static void CPEHandshake(NetworkClient client) {
            var cExtInfo = new ExtInfo {AppName = "Hypercube Server", ExtensionCount = SupportedExtensions};
            client.SendQueue.Enqueue(cExtInfo);

            var cExtEntry = new ExtEntry {ExtName = "CustomBlocks", Version = CustomBlocksVersion};
            client.SendQueue.Enqueue(cExtEntry);

            cExtEntry.ExtName = "EmoteFix";
            cExtEntry.Version = EmoteFixVersion;
            client.SendQueue.Enqueue(cExtEntry);

            cExtEntry.ExtName = "HeldBlock";
            cExtEntry.Version = HeldBlockVersion;
            client.SendQueue.Enqueue(cExtEntry);

            cExtEntry.ExtName = "ClickDistance";
            cExtEntry.Version = ClickDistanceVersion;
            client.SendQueue.Enqueue(cExtEntry);

            cExtEntry.ExtName = "ChangeModel";
            cExtEntry.Version = ChangeModelVersion;
            client.SendQueue.Enqueue(cExtEntry);

            cExtEntry.ExtName = "ExtPlayerList";
            cExtEntry.Version = ExtPlayerListVersion;
            client.SendQueue.Enqueue(cExtEntry);

            cExtEntry.ExtName = "EnvWeatherType";
            cExtEntry.Version = EnvWeatherTypeVersion;
            client.SendQueue.Enqueue(cExtEntry);

            cExtEntry.ExtName = "EnvMapAppearance";
            cExtEntry.Version = EnvMapAppearanceVersion;
            client.SendQueue.Enqueue(cExtEntry);

            cExtEntry.ExtName = "MessageTypes";
            cExtEntry.Version = MessageTypesVersion;
            client.SendQueue.Enqueue(cExtEntry);

            cExtEntry.ExtName = "BlockPermissions";
            cExtEntry.Version = BlockPermissionsVersion;
            client.SendQueue.Enqueue(cExtEntry);

            //CExtEntry.ExtName = "TextHotKey";
            //CExtEntry.Version = TextHotKeyVersion;
            //CExtEntry.Write(Client);

            //CExtEntry.ExtName = "HackControl";
            //CExtEntry.Version = HackControlVersion;
            //CExtEntry.Write(Client);

            //CExtEntry.ExtName = "SelectionCuboid";
            //CExtEntry.Version = SelectionCuboidVersion;
            //CExtEntry.Write(Client);

            cExtEntry.ExtName = "EnvColors";
            cExtEntry.Version = EnvColorsVersion;
            client.SendQueue.Enqueue(cExtEntry);
        }
        
        /// <summary>
        /// Sends additional pre-login packets after receiving a client's supported extensions.
        /// </summary>
        /// <param name="client"></param>
        public static void CPEPackets(NetworkClient client) {
            if (client.CS.CPEExtensions.ContainsKey("ClickDistance")) {
                var distance = new SetClickDistance {Distance = (short) ServerCore.ClickDistance};
                client.SendQueue.Enqueue(distance);
            }

            if (client.CS.CPEExtensions.ContainsKey("CustomBlocks")) {
                var cbsl = new CustomBlockSupportLevel {SupportLevel = CustomBlocksSupportLevel};
                client.SendQueue.Enqueue(cbsl);
            } else 
                client.Login();
        }

        public static void SendAllClickDistance() {
            foreach (var nc in ServerCore.Nh.ClientList) {
                if (!nc.CS.CPEExtensions.ContainsKey("ClickDistance")) 
                    continue;
                var distance = new SetClickDistance { Distance = (short)ServerCore.ClickDistance };
                nc.SendQueue.Enqueue(distance);
            }
        }

        /// <summary>
        /// Does initial setup of ExtPlayerList for a player that is logging in.
        /// </summary>
        /// <param name="client">Client logging in</param>
        public static void SetupExtPlayerList(NetworkClient client) {

            var extPlayerListPacket = new ExtAddPlayerName {GroupRank = 0};

            lock (ServerCore.Nh.ClientLock) {
                foreach (var c in ServerCore.Nh.Clients) {
                    if (c != client) {
                        if (c.CS.CPEExtensions.ContainsKey("ExtPlayerList")) {
                            extPlayerListPacket.NameId = client.CS.NameId;
                            extPlayerListPacket.ListName = client.CS.FormattedName;
                            extPlayerListPacket.PlayerName = client.CS.LoginName;
                            extPlayerListPacket.GroupName = ServerCore.TextFormats.ExtPlayerList +
                                                            client.CS.CurrentMap.CWMap.MapName;

                            c.SendQueue.Enqueue(extPlayerListPacket);
                        }

                        if (!client.CS.CPEExtensions.ContainsKey("ExtPlayerList")) 
                            continue;

                        extPlayerListPacket.NameId = c.CS.NameId;
                        extPlayerListPacket.ListName = c.CS.FormattedName;
                        extPlayerListPacket.PlayerName = c.CS.LoginName;
                        extPlayerListPacket.GroupName = ServerCore.TextFormats.ExtPlayerList + c.CS.CurrentMap.CWMap.MapName;
                        client.SendQueue.Enqueue(extPlayerListPacket);
                    } else {
                        if (!client.CS.CPEExtensions.ContainsKey("ExtPlayerList")) 
                            continue;

                        extPlayerListPacket.NameId = client.CS.NameId;
                        extPlayerListPacket.ListName = client.CS.FormattedName;
                        extPlayerListPacket.PlayerName = client.CS.LoginName;
                        extPlayerListPacket.GroupName = ServerCore.TextFormats.ExtPlayerList +
                                                        client.CS.CurrentMap.CWMap.MapName;
                        client.SendQueue.Enqueue(extPlayerListPacket);
                    }
                }
            }
        }

        /// <summary>
        /// Updates a client on everyone's ExtPlayerList (Ex. Client changed maps.)
        /// </summary>
        /// <param name="client">Client that has moved maps.</param>
        public static void UpdateExtPlayerList(NetworkClient client) {
            var toUpdate = new ExtAddPlayerName
            {
                NameId = client.CS.NameId,
                ListName = client.CS.FormattedName,
                PlayerName = client.CS.LoginName,
                GroupName = ServerCore.TextFormats.ExtPlayerList + client.CS.CurrentMap.CWMap.MapName,
                GroupRank = 0
            };

            lock (ServerCore.Nh.ClientLock) {
                foreach (var c in ServerCore.Nh.Clients) {
                    if (c.CS.CPEExtensions.ContainsKey("ExtPlayerList")) {
                        c.SendQueue.Enqueue(toUpdate);
                    }
                }
            }
        }

        /// <summary>
        /// Determines if a block is valid to be sent in the EnvSetMapAppearence packet.
        /// By spec: Sprites and half-blocks are not allowed to be a side/edge block.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public static bool AppearanceAllowed(Block block) {
            switch (block.Name) {
                case "Sapling":
                case "Yellow Flower":
                case "Red Flower":
                case "Brown Mushroom":
                case "Red Mushroom":
                case "Stair":
                case "Cobblestone Slab":
                case "Rope":
                case "Fire":
                case "Snow":
                    return false;
                default:
                    return true;
            }    
        }

        /// <summary>
        /// Sends additional packets to clients after sending the map to the client.
        /// </summary>
        /// <param name="client"></param>
        public static void PostMapActions(NetworkClient client) {
            int mapAppearance, blockPerms, weatherVer, colorVer;

            // -- EnvMapAppearance
            if (client.CS.CPEExtensions.TryGetValue("EnvMapAppearance", out mapAppearance) &&
                mapAppearance == EnvMapAppearanceVersion) {

                var cpeData = client.CS.CurrentMap.CPESettings;

                var mapApprPacket = new EnvSetMapAppearance {
                    EdgeBlock = cpeData.EdgeBlock,
                    SideBlock = cpeData.SideBlock,
                    SideLevel = cpeData.SideLevel,
                    TextureUrl = cpeData.TextureUrl
                };

                // -- Customblocks compatibility check
                if (mapApprPacket.EdgeBlock > 49) {
                    var mBlock = ServerCore.Blockholder.GetBlock(mapApprPacket.EdgeBlock);

                    if (mBlock.CPELevel > client.CS.CustomBlocksLevel)
                        mapApprPacket.EdgeBlock = (byte)mBlock.CPEReplace;
                }

                if (mapApprPacket.SideBlock > 49) {
                    var mBlock = ServerCore.Blockholder.GetBlock(mapApprPacket.SideBlock);

                    if (mBlock.CPELevel > client.CS.CustomBlocksLevel)
                        mapApprPacket.SideBlock = (byte)mBlock.CPEReplace;
                }

                client.SendQueue.Enqueue(mapApprPacket);
            }

            // -- BlockPermissions

            if (client.CS.CPEExtensions.TryGetValue("BlockPermissions", out blockPerms) &&
                blockPerms == BlockPermissionsVersion) {

                foreach (var block in ServerCore.Blockholder.NumberList) { // -- For every block
                    if (block.CPELevel > client.CS.CustomBlocksLevel) // -- If its within this player's CustomBlock support
                        continue;

                    if (block.Name == "Unknown") // -- If its not an unknown block
                        continue;

                    if (block.Special) // -- If it's not a custom block.
                        continue;

                    var disallowPlace = new SetBlockPermissions { // -- THen set the permissions for the block
                        AllowDeletion = 1,
                        AllowPlacement = 1,
                        BlockType = block.OnClient,
                    };

                    if (!client.HasAllPermissions(block.PlacePermissions)) {
                        disallowPlace.AllowPlacement = 0;
                    }

                    if (!client.HasAllPermissions(block.DeletePermissions))
                        disallowPlace.AllowDeletion = 0;

                    if (disallowPlace.AllowDeletion != 1 || disallowPlace.AllowPlacement != 1) // -- Only send if we're changing permissions though
                        client.SendQueue.Enqueue(disallowPlace);
                }
            }

            // -- EnvWeatherType
            if (client.CS.CPEExtensions.TryGetValue("EnvWeatherType", out weatherVer) &&
                weatherVer == EnvWeatherTypeVersion) {

                var weather = new EnvSetWeatherType {
                    WeatherType = client.CS.CurrentMap.CPESettings.Weather,
                };

                client.SendQueue.Enqueue(weather);
            }

            // -- EnvColors
            if (client.CS.CPEExtensions.TryGetValue("EnvColors", out colorVer) &&
                colorVer == EnvColorsVersion) {

                // -- if envcolors is enabled on the map
                if (client.CS.CurrentMap.CPESettings.EnvColorsVersion > 0) {
                    var skyColor = new EnvSetColor {
                        ColorType = (byte) EnvSetColor.ColorTypes.SkyColor,
                        Red = client.CS.CurrentMap.CPESettings.SkyColor[0],
                        Green = client.CS.CurrentMap.CPESettings.SkyColor[1],
                        Blue = client.CS.CurrentMap.CPESettings.SkyColor[2],
                    };

                    var cloudColor = new EnvSetColor {
                        ColorType = (byte)EnvSetColor.ColorTypes.CloudColor,
                        Red = client.CS.CurrentMap.CPESettings.CloudColor[0],
                        Green = client.CS.CurrentMap.CPESettings.CloudColor[1],
                        Blue = client.CS.CurrentMap.CPESettings.CloudColor[2],
                    };

                    var fogColor = new EnvSetColor {
                        ColorType = (byte)EnvSetColor.ColorTypes.FogColor,
                        Red = client.CS.CurrentMap.CPESettings.FogColor[0],
                        Green = client.CS.CurrentMap.CPESettings.FogColor[1],
                        Blue = client.CS.CurrentMap.CPESettings.FogColor[2],
                    };

                    var ambeintColor = new EnvSetColor {
                        ColorType = (byte)EnvSetColor.ColorTypes.AmbientColor,
                        Red = client.CS.CurrentMap.CPESettings.AmbientColor[0],
                        Green = client.CS.CurrentMap.CPESettings.AmbientColor[1],
                        Blue = client.CS.CurrentMap.CPESettings.AmbientColor[2],
                    };

                    var sunlightColor = new EnvSetColor {
                        ColorType = (byte)EnvSetColor.ColorTypes.SunlightColor,
                        Red = client.CS.CurrentMap.CPESettings.SunlightColor[0],
                        Green = client.CS.CurrentMap.CPESettings.SunlightColor[1],
                        Blue = client.CS.CurrentMap.CPESettings.SunlightColor[2],
                    };

                    client.SendQueue.Enqueue(skyColor);
                    client.SendQueue.Enqueue(cloudColor);
                    client.SendQueue.Enqueue(fogColor);
                    client.SendQueue.Enqueue(ambeintColor);
                    client.SendQueue.Enqueue(sunlightColor);
                }
            }
        }
    }
}
