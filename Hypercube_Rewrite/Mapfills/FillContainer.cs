using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Hypercube.Core;
using Hypercube.Map;

namespace Hypercube.Mapfills {
    public class Fill {
        public string Plugin;
        public FillInvoker Run;
    }

    public class FillContainer {
        public Dictionary<string, Fill> Mapfills;
        public Hypercube Servercore;

        public FillContainer(Hypercube core) {
            Servercore = core;
            Mapfills = new Dictionary<string, Fill>(StringComparer.InvariantCultureIgnoreCase);
            DefaultFills.Init(this);
        }

        public void RegisterFill(string name, Fill mapfill) {
            if (Mapfills.ContainsKey(name))
                Mapfills.Remove(name);

            Mapfills.Add(name, mapfill);
            Servercore.Logger.Log("MapFill", "Fill registered: " + name, LogType.Info);
        }

        public void FillMap(HypercubeMap map, string fillname, params string[] Args) {
            if (!Mapfills.ContainsKey(fillname))
                return;

            if (Mapfills[fillname].Plugin == "") {
                Mapfills[fillname].Run(map, Args);
            } else {
                Servercore.Luahandler.RunFunction(Mapfills[fillname].Plugin, map, Args);
            }

            map.Resend();
            Servercore.Luahandler.RunFunction("E_MapFilled", map, fillname);
        }
    }
}
