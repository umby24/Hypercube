using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hypercube_Classic.Map {
    public interface IMapFill {
        string Name { get; set; }
        string Script { get; set; }
        object GenerateNew { get; set; }
        object GenerateExisting { get; set; }
    }

    public class FillContainer {
        public Dictionary<string, IMapFill> MapFills;
        public Hypercube Servercore;

        // -- Delegates used for holding the creation functions. 
        public delegate HypercubeMap FillNew(Hypercube Core, string Name, short SizeX, short SizeY, short SizeZ);
        public delegate HypercubeMap Fill(HypercubeMap Map);

        public FillContainer(Hypercube Core) {
            MapFills = new Dictionary<string, IMapFill>();
            Servercore = Core;
        }

        public void RegisgerFill(string Fillname, IMapFill Fill) {
            if (MapFills.Keys.Contains(Fillname))
                MapFills.Remove(Fillname);

            MapFills.Add(Fillname, Fill);
            Servercore.Logger._Log("MapFill", "Fill registered: " + Fillname, Libraries.LogType.Info);
        }

        public void FillMap(HypercubeMap Map, string FillName, params object[] Args) {
            if (MapFills.ContainsKey(FillName)) {
                var myDele = (Fill)MapFills[FillName].GenerateExisting;

                if (myDele != null) {
                    myDele(Map);
                    Map.ResendMap();
                }
            }
        }

        public void CreateAndFillMap(string MapName, string FillName, short SizeX, short SizeY, short SizeZ) {
            if (MapFills.ContainsKey(FillName)) {
                var myDele = (FillNew)MapFills[FillName].GenerateExisting;

                if (myDele != null)
                    myDele(Servercore, MapName, SizeX, SizeY, SizeZ);
            }
        }
    }
}
