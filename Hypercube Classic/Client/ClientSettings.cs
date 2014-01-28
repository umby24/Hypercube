using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hypercube_Classic.Core;

namespace Hypercube_Classic.Client {
    /// <summary>
    /// A container for all settings used by clients.
    /// </summary>
    public class ClientSettings {
        public string LoginName;
        public string IP;
        public DateTime LastActive;
        public bool LoggedIn;
        public string MPPass;
        public short NameID;
        public string CurrentMap;

        public bool CPE;
        public string Appname;
        public short Extensions;
        public byte CustomBlocksLevel;
        public Dictionary<string, int> CPEExtensions;
        public List<byte> SelectionCuboids;
    }
}
