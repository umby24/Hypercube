using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hypercube.Client;
using Hypercube.Network;

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
        /// <param name="Client"></param>
        public static void CPEHandshake(NetworkClient Client) {
            var CExtInfo = new ExtInfo();
            CExtInfo.AppName = "Hypercube Server";
            CExtInfo.ExtensionCount = SupportedExtensions;
            Client.SendQueue.Enqueue(CExtInfo);
            //CExtInfo.Write(Client);

            var CExtEntry = new ExtEntry();
            CExtEntry.ExtName = "CustomBlocks";
            CExtEntry.Version = CustomBlocksVersion;
            Client.SendQueue.Enqueue(CExtEntry);

            CExtEntry.ExtName = "EmoteFix";
            CExtEntry.Version = EmoteFixVersion;
            Client.SendQueue.Enqueue(CExtEntry);

            CExtEntry.ExtName = "HeldBlock";
            CExtEntry.Version = HeldBlockVersion;
            Client.SendQueue.Enqueue(CExtEntry);

            //CExtEntry.ExtName = "ClickDistance";
            //CExtEntry.Version = ClickDistanceVersion;
            //CExtEntry.Write(Client);

            //CExtEntry.ExtName = "ChangeModel";
            //CExtEntry.Version = ChangeModelVersion;
            //CExtEntry.Write(Client);

            CExtEntry.ExtName = "ExtPlayerList";
            CExtEntry.Version = ExtPlayerListVersion;
            Client.SendQueue.Enqueue(CExtEntry);

            //CExtEntry.ExtName = "EnvWeatherType";
            //CExtEntry.Version = EnvWeatherTypeVersion;
            //CExtEntry.Write(Client);

            //CExtEntry.ExtName = "EnvMapAppearance";
            //CExtEntry.Version = EnvMapAppearanceVersion;
            //CExtEntry.Write(Client);

            CExtEntry.ExtName = "MessageTypes";
            CExtEntry.Version = MessageTypesVersion;
            Client.SendQueue.Enqueue(CExtEntry);

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
        /// <param name="Client"></param>
        public static void CPEPackets(NetworkClient Client) {
            if (Client.CS.CPEExtensions.ContainsKey("CustomBlocks")) {
                var CBSL = new CustomBlockSupportLevel();
                CBSL.SupportLevel = CustomBlocksSupportLevel;
                Client.SendQueue.Enqueue(CBSL);
                //CBSL.Write(Client);
            } else {
                Client.Login();
            }
        }

        public static void SetupExtPlayerList(NetworkClient Client) {
            Client.CS.NameID = Client.ServerCore.FreeID;

            if (Client.ServerCore.FreeID != Client.ServerCore.NextID)
                Client.ServerCore.FreeID = Client.ServerCore.NextID;
            else {
                Client.ServerCore.FreeID += 1;
                Client.ServerCore.NextID = Client.ServerCore.FreeID;
            }

            Client.ServerCore.Logger.Log("CPE", Client.ServerCore.FreeID.ToString(), Core.LogType.Debug);
            Client.ServerCore.Logger.Log("CPE", Client.ServerCore.NextID.ToString(), Core.LogType.Debug);

            var ExtPlayerListPacket = new ExtAddPlayerName();
            ExtPlayerListPacket.GroupRank = 0;

            lock (Client.ServerCore.nh.ClientLock) {
                foreach (NetworkClient c in Client.ServerCore.nh.Clients) {
                    if (c.CS.CPEExtensions.ContainsKey("ExtPlayerList")) {
                        if (c != Client) {
                            ExtPlayerListPacket.NameID = Client.CS.NameID;
                            ExtPlayerListPacket.ListName = Client.CS.FormattedName;
                            ExtPlayerListPacket.PlayerName = Client.CS.LoginName;
                            ExtPlayerListPacket.GroupName = Client.ServerCore.TextFormats.ExtPlayerList + Client.CS.CurrentMap.CWMap.MapName;

                            Client.SendQueue.Enqueue(ExtPlayerListPacket);
                            //ExtPlayerListPacket.Write(c);

                            if (Client.CS.CPEExtensions.ContainsKey("ExtPlayerList")) {
                                ExtPlayerListPacket.NameID = c.CS.NameID;
                                ExtPlayerListPacket.ListName = c.CS.FormattedName;
                                ExtPlayerListPacket.PlayerName = c.CS.LoginName;
                                ExtPlayerListPacket.GroupName = Client.ServerCore.TextFormats.ExtPlayerList + c.CS.CurrentMap.CWMap.MapName;
                                Client.SendQueue.Enqueue(ExtPlayerListPacket);
                            }
                        } else {
                            ExtPlayerListPacket.NameID = Client.CS.NameID;
                            ExtPlayerListPacket.ListName = Client.CS.FormattedName;
                            ExtPlayerListPacket.PlayerName = Client.CS.LoginName;
                            ExtPlayerListPacket.GroupName = Client.ServerCore.TextFormats.ExtPlayerList + Client.CS.CurrentMap.CWMap.MapName;
                            Client.SendQueue.Enqueue(ExtPlayerListPacket);
                        }
                    }
                }
            }
        }

        public static void UpdateExtPlayerList(NetworkClient Client) {
            // -- ExtPlayerList
            var ToRemove = new ExtRemovePlayerName(); // -- This is needed due to a client bug that doesn't update entries properly. I submitted a PR that fixes this issue, but it hasn't been pushed yet.
            ToRemove.NameID = Client.CS.NameID;

            var ToUpdate = new ExtAddPlayerName();
            ToUpdate.NameID = Client.CS.NameID;
            ToUpdate.ListName = Client.CS.FormattedName;
            ToUpdate.PlayerName = Client.CS.LoginName;
            ToUpdate.GroupName = Client.ServerCore.TextFormats.ExtPlayerList + Client.CS.CurrentMap.CWMap.MapName;
            ToUpdate.GroupRank = 0;

            Client.ServerCore.Logger.Log("CPEU", Client.ServerCore.FreeID.ToString(), Core.LogType.Debug);
            Client.ServerCore.Logger.Log("CPEU", Client.ServerCore.NextID.ToString(), Core.LogType.Debug);

            lock (Client.ServerCore.nh.ClientLock) {
                foreach (NetworkClient c in Client.ServerCore.nh.Clients) {
                    if (c.CS.CPEExtensions.ContainsKey("ExtPlayerList")) {
                        c.SendQueue.Enqueue(ToRemove);
                        c.SendQueue.Enqueue(ToUpdate);
                        //ToRemove.Write(c);
                        //ToUpdate.Write(c);
                    }
                }
            }
        }
    }
}
