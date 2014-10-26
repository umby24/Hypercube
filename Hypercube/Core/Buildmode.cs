using System;
using System.Collections.Generic;

using Hypercube.Client;
using Hypercube.Command;
using Hypercube.Libraries;

namespace Hypercube.Core {

    /// <summary>
    /// Holds and manages server build modes
    /// </summary>
    public class BuildMode {
        public Dictionary<string, BmStruct> Modes;
        public Settings BuildModeLoader;

        public BuildMode() {
            BuildModeLoader = ServerCore.Settings.RegisterFile("Buildmodes.txt", "Settings/", true, Load);
            ServerCore.Settings.ReadSettings(BuildModeLoader);
        }

        public void Load() {
            Modes = new Dictionary<string, BmStruct>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var bm in BuildModeLoader.SettingsDictionary.Keys) {
                var myStruct = new BmStruct();
                BuildModeLoader = ServerCore.Settings.SelectGroup(BuildModeLoader, bm);
                myStruct.Name = ServerCore.Settings.ReadSetting(BuildModeLoader, "Name", "");
                myStruct.Plugin = ServerCore.Settings.ReadSetting(BuildModeLoader, "Plugin", "");
                Modes.Add(myStruct.Name, myStruct);
            }

            Modes = Buildmodes.Init(Modes);
            ServerCore.Logger.Log("Buildmode", "Buildmodes loaded.", LogType.Info);
        }
    }

    public class BuildState {
        public const int MaxResendSize = 1000;
        public List<string> SItems;
        public List<float> FItems;
        public List<int> Items;
        public List<Vector3S> CoordItems;
        public List<Vector3S> Blocks;

        public BuildState() {
            SItems = new List<string>();
            FItems = new List<float>();
            Items = new List<int>();
            CoordItems = new List<Vector3S>();
            Blocks = new List<Vector3S>();
        }

        public string GetString(int index) {
            if (SItems.Count >= index + 1)
                return SItems[index];

            return null;
        }

        public float GetFloat(int index) {
            return FItems[index];
        }

        public int GetInt(int index) {
            return Items[index];
        }

        public Vector3S GetCoord(int index) {
            return CoordItems[index];
        }

        // -- D3 Compatibility

        public short GetCoordX(int index) {
            return CoordItems[index].X;
        }

        public short GetCoordY(int index) {
            return CoordItems[index].Y;
        }

        public short GetCoordZ(int index) {
            return CoordItems[index].Z;
        }

        // -- Set functions

        public void SetString(string value, int index) {
            if ((index + 1) > SItems.Count) {
                for (var i = 0; i < (index + 1); i++)
                    SItems.Add(null);
            }

            SItems.Insert(index, value);
        }

        public void SetFloat(float value, int index) {
            if ((index + 1) > FItems.Count) {
                for (var i = 0; i < (index + 1); i++)
                    FItems.Add(0.0f);
            }

            FItems.Insert(index, value);
        }

        public void SetInt(int value, int index) {
            if ((index + 1) > Items.Count) {
                for (var i = 0; i < (index + 1); i++)
                    Items.Add(0);
            }

            Items[index] = value;
        }

        public void SetCoord(Vector3S coord, int index) {
            if ((index + 1) > CoordItems.Count) {
                for (var i = 0; i < (index + 1); i++)
                    CoordItems.Add(new Vector3S());
            }
            CoordItems.Insert(index, coord);
        }

        public void SetCoord(short x, short y, short z, int index) {
            if ((index + 1) > CoordItems.Count) {
                for (var i = 0; i < (index + 1); i++)
                    CoordItems.Add(new Vector3S());
            }

            var myCoord = new Vector3S {X = x, Y = y, Z = z};
            CoordItems.Insert(index, myCoord);
        }

        public void AddBlock(short x, short y, short z) {
            var thisPoint = new Vector3S {X = x, Y = y, Z = z};

            if (Blocks.Contains(thisPoint))
                return;

            if (Blocks.Count < MaxResendSize)
                Blocks.Add(thisPoint);
            else {
                Blocks.RemoveAt(0);
                Blocks.Add(thisPoint);
            }
        }

        public void ResendBlocks(NetworkClient client) {
            foreach (var point in Blocks)
                client.CS.CurrentMap.SendBlock(client, point.X, point.Y, point.Z, client.CS.CurrentMap.GetBlock(point.X, point.Y, point.Z));

            Blocks.Clear();
        }
    }
}
