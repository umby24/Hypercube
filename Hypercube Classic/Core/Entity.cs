using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hypercube_Classic.Core {
    class Entity {
        public byte Rot, Look, Heldblock, Lastmaterial, Boundblock;
        public bool SendOwn, GlobalChat;
        public short X, Y, Z, BuildMaterial;
        public int ID, ClientID;
        public string FormattedName, Model, BuildMode;

        public Entity() {

        }
    }
}
