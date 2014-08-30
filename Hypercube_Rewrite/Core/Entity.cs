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
        public BmStruct BuildMode;
        public BuildState ClientState;
        public Dictionary<string, string> BuildVariables = new Dictionary<string, string>();
        public NetworkClient MyClient;
        public HypercubeMap Map;
        public Block Lastmaterial, Boundblock, BuildMaterial;

        public Entity(HypercubeMap map, string name, short x, short y, short z, byte rot, byte look) {
            Name = name;
            X = x;
            Y = y;
            Z = z;
            Rot = rot;
            Look = look;
            Map = map;

            
            Id = ServerCore.FreeEids.Pop();
            ServerCore.Logger.Log("Client", "Pop ID " + Id, LogType.Debug);

            BuildMaterial = ServerCore.Blockholder.GetBlock("");
            Lastmaterial = ServerCore.Blockholder.GetBlock(1);
            ClientState = new BuildState();
            BuildMode = new BmStruct {Name = ""};

            ClientId = (byte)Map.FreeIds.Pop();
        }

        public void SetBuildmode(string mode)
        {
            BuildMode = ServerCore.BmContainer.Modes.ContainsKey(mode) ? ServerCore.BmContainer.Modes[mode] : new BmStruct {Name = ""};
            ClientState.ResendBlocks(MyClient);
        }

        public EntityStub CreateStub() {
            return new EntityStub(Id, ClientId, Visible, Map, X, Y, Z, Rot, Look);
        }
    }
}
