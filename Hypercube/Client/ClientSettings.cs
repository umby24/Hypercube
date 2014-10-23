﻿using System;
using System.Collections.Generic;
using Hypercube.Core;
using Hypercube.Map;

namespace Hypercube.Client {
    /// <summary>
    /// A container for all settings used by clients.
    /// </summary>
    public struct ClientSettings {
        public string LoginName, FormattedName, Ip, MpPass;
        public DateTime LastActive;
        public bool LoggedIn, Stopped, Global, Op;
        public short NameId, Id;
        public List<Rank> PlayerRanks;
        public List<int> RankSteps;
        public int MuteTime;
        public Entity MyEntity;
        public HypercubeMap CurrentMap;

        // -- Entity stuff
        public Dictionary<int, EntityStub> Entities;

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
