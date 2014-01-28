using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hypercube_Classic.Client;
using Hypercube_Classic.Packets;

namespace Hypercube_Classic.Packets {
    class CPE {
        // -- General information for CPE.
        public const short SupportedExtensions = 14;
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
            CExtInfo.Write(Client);

            var CExtEntry = new ExtEntry();
            CExtEntry.ExtName = "CustomBlocks";
            CExtEntry.Version = CustomBlocksVersion;
            CExtEntry.Write(Client);

            CExtEntry.ExtName = "EmoteFix";
            CExtEntry.Version = EmoteFixVersion;
            CExtEntry.Write(Client);

            CExtEntry.ExtName = "HeldBlock";
            CExtEntry.Version = HeldBlockVersion;
            CExtEntry.Write(Client);

            CExtEntry.ExtName = "ClickDistance";
            CExtEntry.Version = ClickDistanceVersion;
            CExtEntry.Write(Client);

            CExtEntry.ExtName = "ChangeModel";
            CExtEntry.Version = ChangeModelVersion;
            CExtEntry.Write(Client);

            CExtEntry.ExtName = "ExtPlayerList";
            CExtEntry.Version = ExtPlayerListVersion;
            CExtEntry.Write(Client);

            CExtEntry.ExtName = "EnvWeatherType";
            CExtEntry.Version = EnvWeatherTypeVersion;
            CExtEntry.Write(Client);

            CExtEntry.ExtName = "EnvMapAppearance";
            CExtEntry.Version = EnvMapAppearanceVersion;
            CExtEntry.Write(Client);

            CExtEntry.ExtName = "MessageTypes";
            CExtEntry.Version = MessageTypesVersion;
            CExtEntry.Write(Client);

            CExtEntry.ExtName = "BlockPermissions";
            CExtEntry.Version = BlockPermissionsVersion;
            CExtEntry.Write(Client);

            CExtEntry.ExtName = "TextHotKey";
            CExtEntry.Version = TextHotKeyVersion;
            CExtEntry.Write(Client);

            CExtEntry.ExtName = "HackControl";
            CExtEntry.Version = HackControlVersion;
            CExtEntry.Write(Client);

            CExtEntry.ExtName = "SelectionCuboid";
            CExtEntry.Version = SelectionCuboidVersion;
            CExtEntry.Write(Client);

            CExtEntry.ExtName = "EnvColors";
            CExtEntry.Version = EnvColorsVersion;
            CExtEntry.Write(Client);
        }
        
        /// <summary>
        /// Sends additional pre-login packets after receiving a client's supported extensions.
        /// </summary>
        /// <param name="Client"></param>
        public static void CPEPackets(NetworkClient Client) {
            if (Client.CS.CPEExtensions.ContainsKey("CustomBlocks")) {
                var CBSL = new CustomBlockSupportLevel();
                CBSL.SupportLevel = CustomBlocksSupportLevel;
                CBSL.Write(Client);
            } else {
                Client.Login();
            }
        }
    }
}
