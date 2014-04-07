using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hypercube_Classic.Core;

namespace Hypercube_Classic.Client {
    public struct Undo {
        public short x;
        public short y;
        public short z;
        public Block OldBlock;
    }
}
