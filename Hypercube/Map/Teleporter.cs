using System.Collections.Generic;
using Hypercube.Core;
using Hypercube.Libraries;

namespace Hypercube.Map {
    public class TeleporterContainer {
        private readonly Settings _porterSettings;
        private readonly List<Teleporter> _teleporters;

        public TeleporterContainer(string mapName) {
            _teleporters = new List<Teleporter>();
            _porterSettings = ServerCore.Settings.RegisterFile(mapName + "ports.txt", "Maps/", true, ReadPorters);
            ServerCore.Settings.ReadSettings(_porterSettings);
        }

        void ReadPorters() {
            foreach (var key in _porterSettings.SettingsDictionary.Keys) {
                ServerCore.Settings.SelectGroup(_porterSettings, key);

                var newtp = new Teleporter {
                    Name = key,

                    Start = {
                        X = (short) ServerCore.Settings.ReadSetting(_porterSettings, "StartX", 0),
                        Y = (short) ServerCore.Settings.ReadSetting(_porterSettings, "StartY", 0),
                        Z = (short) ServerCore.Settings.ReadSetting(_porterSettings, "StartZ", 0)
                    },

                    End = {
                        X = (short) ServerCore.Settings.ReadSetting(_porterSettings, "EndX", 0),
                        Y = (short) ServerCore.Settings.ReadSetting(_porterSettings, "EndY", 0),
                        Z = (short) ServerCore.Settings.ReadSetting(_porterSettings, "EndZ", 0),
                    },

                    Dest = {
                        X = (short) ServerCore.Settings.ReadSetting(_porterSettings, "DestX", 0),
                        Y = (short) ServerCore.Settings.ReadSetting(_porterSettings, "DestY", 0),
                        Z = (short) ServerCore.Settings.ReadSetting(_porterSettings, "DestZ", 0),
                    },

                    DestLook = (byte)ServerCore.Settings.ReadSetting(_porterSettings, "DestLook", 0),
                    DestRot = (byte)ServerCore.Settings.ReadSetting(_porterSettings, "DestRot", 0),
                    DestinationMap = HypercubeMap.GetMap(ServerCore.Settings.ReadSetting(_porterSettings, "DestMap", "")),
                };

                if (newtp.DestinationMap == null)
                    continue; // -- Incorrectly formatted teleporter, or map has been deleted.

                _teleporters.Add(newtp);
            }
        }

        public void CreateTeleporter(string name, Vector3S start, Vector3S end, Vector3S dest, byte destLook, byte destRot, HypercubeMap destMap) {
            var newtp = new Teleporter {
                Name = name,
                Start = start,
                End = end,
                Dest = dest,
                DestLook = destLook,
                DestRot = destRot,
                DestinationMap = destMap,
            };

            var myTp = _teleporters.Find(o => o.Name == name); // -- Linq is so hacky.. damn.

            if (myTp != null)
                _teleporters.Remove(myTp);

            _teleporters.Add(newtp);

            // -- Save to file as well.
            ServerCore.Settings.SelectGroup(_porterSettings, name);
            ServerCore.Settings.SaveSetting(_porterSettings, "StartX", start.X.ToString());
            ServerCore.Settings.SaveSetting(_porterSettings, "StartY", start.Y.ToString());
            ServerCore.Settings.SaveSetting(_porterSettings, "StartZ", start.Z.ToString());
            ServerCore.Settings.SaveSetting(_porterSettings, "EndX", end.X.ToString());
            ServerCore.Settings.SaveSetting(_porterSettings, "EndY", end.Y.ToString());
            ServerCore.Settings.SaveSetting(_porterSettings, "EndZ", end.Z.ToString());
            ServerCore.Settings.SaveSetting(_porterSettings, "DestX", dest.X.ToString());
            ServerCore.Settings.SaveSetting(_porterSettings, "DestY", dest.Y.ToString());
            ServerCore.Settings.SaveSetting(_porterSettings, "DestZ", dest.Z.ToString());
            ServerCore.Settings.SaveSetting(_porterSettings, "DestRot", destRot.ToString());
            ServerCore.Settings.SaveSetting(_porterSettings, "DestLook", destLook.ToString());
            ServerCore.Settings.SaveSetting(_porterSettings, "DestMap", destMap.CWMap.MapName);
            ServerCore.Settings.SaveSettings(_porterSettings);
        }

        public void DeleteTeleporter(string name) {
            var myTp = _teleporters.Find(o => o.Name == name); // -- Linq is so hacky.. damn.

            if (myTp == null) 
                return;

            _teleporters.Remove(myTp);
            _porterSettings.SettingsDictionary.Remove(name);
        }

        public Teleporter FindTeleporter(Vector3S location) {
            foreach (var tele in _teleporters) {
                if (location.X < tele.Start.X || location.X > tele.End.X) 
                    continue;

                if (location.Y < tele.Start.Y || location.Y > tele.End.Y)
                    continue;

                if (location.Z < tele.Start.Z || location.Z > tele.End.Z)
                    continue;

                // -- Teleport the player.
                return tele;
            }

            return null; // -- No teleporter found.
        }
    }

    public class Teleporter {
        public string Name;
        public Vector3S Start, End, Dest;
        public byte DestRot, DestLook;
        public HypercubeMap DestinationMap;
    }
}
