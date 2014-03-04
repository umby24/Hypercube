﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hypercube_Classic.Client;
using Hypercube_Classic.Map;

namespace Hypercube_Classic.Core {
    public class Entity {
        public byte ClientID, Rot, Look, Heldblock;
        public bool SendOwn, Changed;
        public short X, Y, Z;
        public int ID, BuildState;
        public string Name, Model, BuildMode;
        public Dictionary<string, string> BuildVariables = new Dictionary<string, string>();
        public NetworkClient MyClient;
        public HypercubeMap Map;
        public Block Lastmaterial, Boundblock, BuildMaterial;

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
            BuildMaterial = Core.Blockholder.GetBlock("");
            Lastmaterial = Core.Blockholder.GetBlock(1);

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

        public void HandleBuildmode(short _X, short _Y, short _Z, byte Mode, byte Type) {
            var MyBlock = MyClient.ServerCore.Blockholder.GetBlock((int)Type);

            if (MyBlock == Boundblock && BuildMaterial.Name != "Unknown")
                MyBlock = BuildMaterial;

            Lastmaterial = MyBlock;

            if (BuildMode != null && BuildMode != "") {
                //TODO: Add buildmodes all proper like..
            } else {
                if (!MyClient.CS.Stopped)
                    Map.ClientChangeBlock(MyClient, _X, _Y, _Z, Mode, MyBlock);
                else {
                    Chat.SendClientChat(MyClient, "&4Error:&f You are stopped, you cannot build.");

                    if ((0 > _X || Map.Map.SizeX <= _X) || (0 > _Z || Map.Map.SizeY <= _Z) || (0 > _Y || Map.Map.SizeZ <= _Y))
                        return;

                    MyClient.CS.CurrentMap.SendBlockToClient(_X, _Y, _Z, MyClient.CS.CurrentMap.GetBlock(_X, _Y, _Z), MyClient);
                }
            }
        }
    }
}
