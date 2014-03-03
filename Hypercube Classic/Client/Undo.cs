using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hypercube_Classic.Core;

namespace Hypercube_Classic.Client {
    public struct Undo {
        short x;
        short y;
        short z;
        Block OldBlock;
    }
}
