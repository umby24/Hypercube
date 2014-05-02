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
                    var NewMap = new HypercubeMap(ServerCore, file);
                    ServerCore.Maps.Add(NewMap);
                    ServerCore.Logger._Log("MapWatcher", "Loaded map '" + file + "'. (X=" + NewMap.Map.SizeX + " Y=" + NewMap.Map.SizeZ + " Z=" + NewMap.Map.SizeY + ")", Libraries.LogType.Info);
                } catch (Exception e) {
                    ServerCore.Logger._Log("MapWatcher", "Failed to load map '" + file + "'.", Libraries.LogType.Warning);
                    ServerCore.Logger._Log("MapWatcher", e.Message, Libraries.LogType.Error);
                }
            }
        }
    }
}
