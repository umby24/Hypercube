using System.Collections.Generic;
using Hypercube.Core;
using Hypercube.Libraries;

namespace Hypercube.Map {
    public class TeleporterContainer {
        private readonly Settings _porterSettings;
        private readonly List<Teleporter> _teleporters;

        public TeleporterContainer(string mapName) {
            _teleporters = new List<Teleporter>();
            _porterSettings = new Settings(mapName + "ports.txt", ReadPorters, "Maps/");
            ServerCore.Setting.RegisterFile(_porterSettings);
            _porterSettings.LoadFile();
        }

        void ReadPorters() {
            foreach (var key in _porterSettings.SettingsDictionary.Keys) {
                _porterSettings.SelectGroup(key);
                var newtp = new Teleporter {
                    Name = key,

                    Start = {
                        X = (short) _porterSettings.Read("StartX", 0),
                        Y = (short) _porterSettings.Read("StartY", 0),
                        Z = (short) _porterSettings.Read("StartZ", 0)
                    },

                    End = {
                        X = (short) _porterSettings.Read("EndX", 0),
                        Y = (short) _porterSettings.Read("EndY", 0),
                        Z = (short) _porterSettings.Read("EndZ", 0),
                    },

                    Dest = {
                        X = (short) _porterSettings.Read("DestX", 0),
                        Y = (short) _porterSettings.Read("DestY", 0),
                        Z = (short) _porterSettings.Read("DestZ", 0),
                    },

                    DestLook = (byte)_porterSettings.Read("DestLook", 0),
                    DestRot = (byte)_porterSettings.Read("DestRot", 0),
                    DestinationMap = HypercubeMap.GetMap(_porterSettings.Read("DestMap", "")),
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
            _porterSettings.SelectGroup(name);
            _porterSettings.Write("StartX", start.X.ToString());
            _porterSettings.Write("StartY", start.Y.ToString());
            _porterSettings.Write("StartZ", start.Z.ToString());
            _porterSettings.Write("EndX", end.X.ToString());
            _porterSettings.Write("EndY", end.Y.ToString());
            _porterSettings.Write("EndZ", end.Z.ToString());
            _porterSettings.Write("DestX", dest.X.ToString());
            _porterSettings.Write("DestY", dest.Y.ToString());
            _porterSettings.Write("DestZ", dest.Z.ToString());
            _porterSettings.Write("DestRot", destRot.ToString());
            _porterSettings.Write("DestLook", destLook.ToString());
            _porterSettings.Write("DestMap", destMap.CWMap.MapName);
            _porterSettings.SaveFile();
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
