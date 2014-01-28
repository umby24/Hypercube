using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Hypercube_Classic.Map;

namespace Hypercube_Classic.Map {
    public class MapWatcher {
        /// <summary>
        /// Loads all maps in the Maps directory. Creates this directory if it does not exist.
        /// </summary>
        /// <param name="ServerCore"></param>
        public static void Watch(Hypercube ServerCore) {
            if (!Directory.Exists("Maps"))
                Directory.CreateDirectory("Maps");

            string[] files = Directory.GetFiles("Maps", "*.cw*", SearchOption.AllDirectories);

            foreach (string file in files) {
                try {
                    var NewMap = new HypercubeMap(file);
                    ServerCore.Maps.Add(NewMap);
                    ServerCore.Logger._Log("Info", "MapWatcher", "Loaded map '" + file + "'. (X=" + NewMap.Map.SizeX + " Y=" + NewMap.Map.SizeZ + " Z=" + NewMap.Map.SizeY + ")");
                } catch {
                    ServerCore.Logger._Log("Error", "MapWatcher", "Failed to load map '" + file + "'.");
                }
            }
        }
    }
}
