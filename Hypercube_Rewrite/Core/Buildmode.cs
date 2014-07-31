using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hypercube.Client;
using Hypercube.Libraries;

namespace Hypercube.Core {

    /// <summary>
    /// Holds and manages server build modes
    /// </summary>
    public class BuildMode {
        public Dictionary<string, BMStruct> Modes;
        public ISettings BuildModeLoader;
        Hypercube ServerCore;

        public BuildMode(Hypercube Core) {
            ServerCore = Core;
            BuildModeLoader = ServerCore.Settings.RegisterFile("Buildmodes.txt", true, new PBSettingsLoader.LoadSettings(Load));
            ServerCore.Settings.ReadSettings(BuildModeLoader);
        }

        public void Load() {
            Modes = new Dictionary<string, BMStruct>(StringComparer.InvariantCultureIgnoreCase);

            foreach (string bm in BuildModeLoader.Settings.Keys) {
                var myStruct = new BMStruct();
                BuildModeLoader = ServerCore.Settings.SelectGroup(BuildModeLoader, bm);
                myStruct.Name = ServerCore.Settings.ReadSetting(BuildModeLoader, "Name", "");
                myStruct.Plugin = ServerCore.Settings.ReadSetting(BuildModeLoader, "Plugin", "");
                Modes.Add(myStruct.Name, myStruct);
            }

            ServerCore.Logger.Log("Buildmode", "Buildmodes loaded.", LogType.Info);
        }
    }

    public class BuildState {
        public const int MaxResendSize = 1000;
        public List<string> SItems;
        public List<float> FItems;
        public List<int> IItems;
        public List<Vector3S> CoordItems;
        public List<Vector3S> Blocks;

        public BuildState() {
            SItems = new List<string>();
            FItems = new List<float>();
            IItems = new List<int>();
            CoordItems = new List<Vector3S>();
            Blocks = new List<Vector3S>();
        }

        public string GetString(int index) {
            if (SItems.Count >= index + 1)
                return SItems[index];
            else
                return null;
        }

        public float GetFloat(int index) {
            return FItems[index];
        }

        public int GetInt(int index) {
            return IItems[index];
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

        public void SetString(string Value, int index) {
            SItems.Insert(index, Value);
        }

        public void SetFloat(float Value, int index) {
            FItems.Insert(index, Value);
        }

        public void SetInt(int Value, int index) {
            IItems.Insert(Value, index);
        }

        public void SetCoord(short x, short y, short z, int index) {
            var myCoord = new Vector3S();
            myCoord.X = x;
            myCoord.Y = y;
            myCoord.Z = z;

            CoordItems.Insert(index, myCoord);
        }

        public void AddBlock(short X, short Y, short Z) {
            var ThisPoint = new Vector3S();
            ThisPoint.X = X;
            ThisPoint.Y = Y;
            ThisPoint.Z = Z;

            if (Blocks.Contains(ThisPoint))
                return;

            if (Blocks.Count < MaxResendSize)
                Blocks.Add(ThisPoint);
            else {
                Blocks.RemoveAt(0);
                Blocks.Add(ThisPoint);
            }
        }

        public void ResendBlocks(NetworkClient Client) {
            foreach (Vector3S point in Blocks)
                Client.CS.CurrentMap.SendBlock(Client, point.X, point.Y, point.Z, Client.CS.CurrentMap.GetBlock(point.X, point.Y, point.Z));

            Blocks.Clear();
        }
    }
}
