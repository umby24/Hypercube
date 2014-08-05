using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hypercube.Core;
using Hypercube.Map;

namespace Hypercube.Client {
    /// <summary>
    /// A container for all settings used by clients.
    /// </summary>
    public struct ClientSettings {
        public string LoginName, FormattedName, IP, MPPass;
        public DateTime LastActive;
        public bool LoggedIn, Stopped, Global, Op;
        public short NameID, ID;
        public List<Rank> PlayerRanks;
        public List<int> RankSteps;
        public int MuteTime;
        public Entity MyEntity;
        public HypercubeMap CurrentMap;

        // -- Undo System
        public List<Undo> UndoObjects;
        public int CurrentIndex;

        // -- CPE Stuff
        public bool CPE;
        public string Appname;
        public short Extensions;
        public byte CustomBlocksLevel;
        public Dictionary<string, int> CPEExtensions;
        public List<byte> SelectionCuboids;
        public Block HeldBlock;
    }
}
