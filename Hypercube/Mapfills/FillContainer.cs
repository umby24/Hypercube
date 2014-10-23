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

        public FillContainer() {
            Mapfills = new Dictionary<string, Fill>(StringComparer.InvariantCultureIgnoreCase);
            DefaultFills.Init(this);
        }

        public void CreateFill(string name, string plugin) {
            var newFill = new Fill {Plugin = plugin, Run = null};
            RegisterFill(name, newFill);
        }

        public void RegisterFill(string name, Fill mapfill) {
            if (Mapfills.ContainsKey(name))
                Mapfills.Remove(name);

            Mapfills.Add(name, mapfill);
            ServerCore.Logger.Log("MapFill", "Fill registered: " + name, LogType.Info);
        }

        public void FillMap(HypercubeMap map, string fillname, params string[] args) {
            if (!Mapfills.ContainsKey(fillname))
                return;

            if (Mapfills[fillname].Plugin == "")
                Mapfills[fillname].Run(map, args);
            else {
                map.CWMap.BlockData = new byte[map.CWMap.BlockData.Length];
                ServerCore.Luahandler.RunFunction(Mapfills[fillname].Plugin, map, map.CWMap.SizeX, map.CWMap.SizeZ,
                    map.CWMap.SizeY, args);
            }

            map.CWMap.GeneratorName = fillname;
            map.CWMap.GeneratingSoftware = "Hypercube";
            
            map.Resend();
            ServerCore.Luahandler.RunFunction("E_MapFilled", map, fillname);
        }
    }
}