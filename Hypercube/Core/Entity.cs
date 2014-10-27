using System.Collections.Generic;

using Hypercube.Map;
using Hypercube.Client;

namespace Hypercube.Core {
    public class EntityStub {
        public int Id;
        public byte ClientId, Rot, Look;
        //public short X, Y, Z;
        public Vector3S Location;
        public bool Looked, Changed, Visible, Spawned;
        public string Model;
        public HypercubeMap Map;

        public EntityStub(int id, byte clientId, bool visible, HypercubeMap cMap, Vector3S location, byte rot, byte look, string model) {
            Map = cMap;
            Id = id;
            ClientId = clientId;
            Visible = visible;
            Location = location;
            Rot = rot;
            Look = look;
            Looked = false;
            Changed = false;
            Spawned = false;
            Model = model;
        }
    }

    public class Entity {
        public byte ClientId, Rot, Look, Heldblock;
        public bool SendOwn, Visible;
        public Vector3S Location;
        public int Id, BuildState;
        public string Name, Model;
        public BmStruct BuildMode;
        public BuildState ClientState;
        public Dictionary<string, string> BuildVariables = new Dictionary<string, string>();
        public NetworkClient MyClient;
        public HypercubeMap Map;
        public Block Lastmaterial, Boundblock, BuildMaterial;

        public Entity(HypercubeMap map, string name, Vector3S location, byte rot, byte look) {
            Name = name;
            Location = location;
            Rot = rot;
            Look = look;
            Map = map;
            Model = "default";
            Visible = true;
            Id = ServerCore.FreeEids.Pop();

            BuildMaterial = ServerCore.Blockholder.GetBlock("");
            Lastmaterial = ServerCore.Blockholder.GetBlock(1);
            ClientState = new BuildState();
            BuildMode = new BmStruct {Name = ""};

            ClientId = (byte)Map.FreeIds.Pop();
        }

        public void Kill() {
            Chat.SendMapChat(Map, "&c" + Name + " was killed.");
            SetBlockPosition(Map.CWMap.SpawnX, Map.CWMap.SpawnZ, Map.CWMap.SpawnY);

            Rot = Map.CWMap.SpawnRotation;
            Look = Map.CWMap.SpawnLook;
            SendOwn = true;
        }

        public void SetBuildmode(string mode)
        {
            BuildMode = ServerCore.BmContainer.Modes.ContainsKey(mode) ? ServerCore.BmContainer.Modes[mode] : new BmStruct {Name = ""};
            ClientState.ResendBlocks(MyClient);
            ClientState = new BuildState();
        }

        public EntityStub CreateStub() {
            return new EntityStub(Id, ClientId, Visible, Map, Location, Rot, Look, Model);
        }

        public Vector3S GetBlockLocation() {
            var myLoc = new Vector3S {
                X = (short)(Location.X / 32),
                Y = (short)(Location.Y / 32),
                Z = (short)((Location.Z - 51) / 32),
            };

            return myLoc;
        }

        public void SetBlockPosition(short x, short y, short z) {
            Location.X = (short)(x * 32);
            Location.X = (short)(y * 32);
            Location.X = (short)((z * 32) + 51);
        }

        public void SetBlockPosition(Vector3S blockLoc) {
            Location.X = (short)(blockLoc.X * 32);
            Location.Y = (short)(blockLoc.Y * 32);
            Location.Z = (short)((blockLoc.Z * 32) + 51);
        }
    }
}
