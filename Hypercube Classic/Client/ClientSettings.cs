using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hypercube_Classic.Core;
using Hypercube_Classic.Map;

namespace Hypercube_Classic.Client {
    /// <summary>
    /// A container for all settings used by clients.
    /// </summary>
    public class ClientSettings {
        public string LoginName, FormattedName, IP, MPPass;
        public DateTime LastActive;
        public bool LoggedIn, Stopped, Global;
        public short NameID;
        public Rank PlayerRank;
        public int RankStep, ID, MuteTime;
        public Entity MyEntity;
        public HypercubeMap CurrentMap;

        public bool CPE;
        public string Appname;
        public short Extensions;
        public byte CustomBlocksLevel;
        public Dictionary<string, int> CPEExtensions;
        public List<byte> SelectionCuboids;
    }
}
