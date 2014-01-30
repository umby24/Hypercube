﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hypercube_Classic.Client;
using Hypercube_Classic.Map;

namespace Hypercube_Classic.Core {
    public class Entity {
        public byte ClientID, Rot, Look, Heldblock, Lastmaterial, Boundblock;
        public bool SendOwn, Changed;
        public short X, Y, Z, BuildMaterial;
        public int ID;
        public string Name, Model, BuildMode;
        public NetworkClient MyClient;
        public HypercubeMap Map;

        public Entity(Hypercube Core, HypercubeMap _Map, string _Name, short _X, short _Y, short _Z, byte _Rot, byte _Look) {
            Name = _Name;
            X = _X;
            Y = _Y;
            Z = _Z;
            Rot = _Rot;
            Look = _Look;
            Changed = true;
            SendOwn = true;
            ID = Core.EFree;

            if (Core.EFree != Core.ENext)
                Core.EFree = Core.ENext;
            else {
                Core.EFree += 1;
                Core.NextID = Core.EFree;
            }

            Map = _Map;

            if (_Map.FreeID != 128) {
                ClientID = (byte)_Map.FreeID;
                if (_Map.FreeID != _Map.NextID)
                    _Map.FreeID = _Map.NextID;
                else {
                    _Map.FreeID += 1;
                    _Map.NextID = _Map.FreeID;
                }
            }
        }
    }
}
