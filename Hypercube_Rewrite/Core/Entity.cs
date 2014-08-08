using System.Collections.Generic;

using Hypercube.Map;
using Hypercube.Client;

namespace Hypercube.Core {
    public class EntityStub {
        public int Id;
        public byte ClientId, Rot, Look;
        public short X, Y, Z;
        public bool Looked, Changed, Visible, Spawned;
        public HypercubeMap Map;

        public EntityStub(int id, byte clientId, bool visible, HypercubeMap cMap, short x, short y, short z, byte rot, byte look) {
            Map = cMap;
            Id = id;
            ClientId = clientId;
            Visible = visible;
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
        public byte ClientId, Rot, Look, Heldblock;
        public bool SendOwn, Visible;
        public short X, Y, Z;
        public int Id, BuildState;
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
            
            Id = core.EFree;
            BuildMaterial = core.Blockholder.GetBlock("");
            Lastmaterial = core.Blockholder.GetBlock(1);
            ClientState = new BuildState();
            BuildMode = new BMStruct {Name = ""};

            // -- Move entity free IDs.
            if (core.EFree != core.ENext)
                core.EFree = core.ENext;
            else {
                core.EFree += 1;
                core.ENext = core.EFree;
            }

            if (Map.FreeId != 128) {
                ClientId = (byte)Map.FreeId;

                if (Map.FreeId != Map.NextId)
                    Map.FreeId = Map.NextId;
                else {
                    Map.FreeId += 1;
                    Map.NextId = Map.FreeId;
                }
            }

        }

        public void SetBuildmode(string mode)
        {
            BuildMode = Servercore.BMContainer.Modes.ContainsKey(mode) ? Servercore.BMContainer.Modes[mode] : new BMStruct {Name = ""};
            ClientState.ResendBlocks(MyClient);
        }

        public EntityStub CreateStub() {
            return new EntityStub(Id, ClientId, Visible, Map, X, Y, Z, Rot, Look);
        }
    }
}
