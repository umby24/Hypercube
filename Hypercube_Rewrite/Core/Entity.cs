using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hypercube.Map;
using Hypercube.Client;

namespace Hypercube.Core {
    public class EntityStub {
        public int ID;
        public byte ClientID, Rot, Look;
        public short X, Y, Z;
        public bool Looked, Changed, Visible, Spawned;
        public HypercubeMap Map;

        public EntityStub(int _ID, byte _ClientID, bool _Visible, HypercubeMap cMap, short x, short y, short z, byte rot, byte look) {
            Map = cMap;
            ID = _ID;
            ClientID = _ClientID;
            Visible = _Visible;
            X = x;
            Y = y;
            Z = z;
            Rot = rot;
            Look = look;
            Looked = false;
            Changed = false;
            Spawned = false;
        }
    }

    public class Entity {
        public byte ClientID, Rot, Look, Heldblock;
        public bool SendOwn, Visible;
        public short X, Y, Z;
        public int ID, BuildState;
        public string Name, Model;
        public BMStruct BuildMode;
        public BuildState ClientState;
        public Dictionary<string, string> BuildVariables = new Dictionary<string, string>();
        public NetworkClient MyClient;
        public Hypercube Servercore;
        public HypercubeMap Map;
        public Block Lastmaterial, Boundblock, BuildMaterial;

        public Entity(Hypercube core, HypercubeMap map, string name, short x, short y, short z, byte rot, byte look) {
            Name = name;
            X = x;
            Y = y;
            Z = z;
            Rot = rot;
            Look = look;
            Map = map;
            Servercore = core;
            
            ID = core.EFree;
            BuildMaterial = core.Blockholder.GetBlock("");
            Lastmaterial = core.Blockholder.GetBlock(1);
            ClientState = new BuildState();
            BuildMode = new BMStruct();
            BuildMode.Name = "";

            // -- Move entity free IDs.
            if (core.EFree != core.ENext)
                core.EFree = core.ENext;
            else {
                core.EFree += 1;
                core.ENext = core.EFree;
            }

            if (Map.FreeID != 128) {
                ClientID = (byte)Map.FreeID;

                if (Map.FreeID != Map.NextID)
                    Map.FreeID = Map.NextID;
                else {
                    Map.FreeID += 1;
                    Map.NextID = Map.FreeID;
                }
            }

        }

        public void SetBuildmode(string Mode) {
            var TestBM = new BMStruct();
            TestBM.Name = Mode;

            if (Servercore.BMContainer.Modes.ContainsKey(Mode)) {
                BuildMode = Servercore.BMContainer.Modes[Mode];
            } else {
                BuildMode = new BMStruct();
                BuildMode.Name = "";
            }

            ClientState.ResendBlocks(MyClient);
        }

        public EntityStub CreateStub() {
            return new EntityStub(ID, ClientID, Visible, Map, X, Y, Z, Rot, Look);
        }
    }
}
