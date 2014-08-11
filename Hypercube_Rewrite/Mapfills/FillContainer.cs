using System;
using System.Collections.Generic;

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

        public void FillMap(HypercubeMap map, string fillname, params string[] args) {
            if (!Mapfills.ContainsKey(fillname))
                return;

            if (Mapfills[fillname].Plugin == "") {
                Mapfills[fillname].Run(map, args);
            } else {
                Servercore.Luahandler.RunFunction(Mapfills[fillname].Plugin, map, args);
            }

            map.Resend();
            Servercore.Luahandler.RunFunction("E_MapFilled", map, fillname);
        }
    }
}
