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
        public string LoginName, FormattedName, IP, MPPass, CurrentMap;
        public DateTime LastActive;
        public bool LoggedIn, Muted, Stopped, Global;
        public short NameID;
        public Rank PlayerRank;
        public int RankStep;
        public Entity MyEntity;

        public bool CPE;
        public string Appname;
        public short Extensions;
        public byte CustomBlocksLevel;
        public Dictionary<string, int> CPEExtensions;
        public List<byte> SelectionCuboids;
    }
}
