using Hypercube.Client;

namespace Hypercube.Network {
    public class CPE {
        // -- General information for CPE.
        public const short SupportedExtensions = 5;
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

            //CExtEntry.ExtName = "ClickDistance";
            //CExtEntry.Version = ClickDistanceVersion;
            //CExtEntry.Write(Client);

            //CExtEntry.ExtName = "ChangeModel";
            //CExtEntry.Version = ChangeModelVersion;
            //CExtEntry.Write(Client);

            cExtEntry.ExtName = "ExtPlayerList";
            cExtEntry.Version = ExtPlayerListVersion;
            client.SendQueue.Enqueue(cExtEntry);

            //CExtEntry.ExtName = "EnvWeatherType";
            //CExtEntry.Version = EnvWeatherTypeVersion;
            //CExtEntry.Write(Client);

            //CExtEntry.ExtName = "EnvMapAppearance";
            //CExtEntry.Version = EnvMapAppearanceVersion;
            //CExtEntry.Write(Client);

            cExtEntry.ExtName = "MessageTypes";
            cExtEntry.Version = MessageTypesVersion;
            client.SendQueue.Enqueue(cExtEntry);

            //CExtEntry.ExtName = "BlockPermissions";
            //CExtEntry.Version = BlockPermissionsVersion;
            //CExtEntry.Write(Client);

            //CExtEntry.ExtName = "TextHotKey";
            //CExtEntry.Version = TextHotKeyVersion;
            //CExtEntry.Write(Client);

            //CExtEntry.ExtName = "HackControl";
            //CExtEntry.Version = HackControlVersion;
            //CExtEntry.Write(Client);

            //CExtEntry.ExtName = "SelectionCuboid";
            //CExtEntry.Version = SelectionCuboidVersion;
            //CExtEntry.Write(Client);

            //CExtEntry.ExtName = "EnvColors";
            //CExtEntry.Version = EnvColorsVersion;
            //CExtEntry.Write(Client);
        }
        
        /// <summary>
        /// Sends additional pre-login packets after receiving a client's supported extensions.
        /// </summary>
        /// <param name="client"></param>
        public static void CPEPackets(NetworkClient client) {
            if (client.CS.CPEExtensions.ContainsKey("CustomBlocks")) {
                var cbsl = new CustomBlockSupportLevel {SupportLevel = CustomBlocksSupportLevel};
                client.SendQueue.Enqueue(cbsl);
            } else 
                client.Login();
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
    }
}
