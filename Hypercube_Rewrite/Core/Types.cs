using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hypercube.Client;
using Hypercube.Map;

namespace Hypercube.Core {
    public enum LogType {
        Debug,
        Info,
        Warning,
        Error,
        Critical,
        Chat,
        Command,
        NotSet
    }

    public struct Undo {
        public short x;
        public short y;
        public short z;
        public Block OldBlock;
        public Block NewBlock;
    }

    public struct Vector3S {
        public short X;
        public short Y;
        public short Z;
    }

    public struct BMStruct {
        public string Name;
        public string Plugin;
    }

    public struct QueueComparator : IEqualityComparer<QueueItem> {
        public bool Equals(QueueItem Item1, QueueItem Item2) {
            if (Item1.X == Item2.X && Item1.Y == Item2.Y && Item1.Z == Item2.Z)
                return true;
            else
                return false;
        }

        public int GetHashCode(QueueItem Item) {
            int hCode = Item.X ^ Item.Y ^ Item.Z;
            return hCode.GetHashCode();
        }
    }

    public class QueueItem {
        public short X, Y, Z, Priority;
        public DateTime DoneTime;

        public QueueItem(short _X, short _Y, short _Z, short _Priority) {
            X = _X;
            Y = _Y;
            Z = _Z;
            Priority = _Priority;
        }
        public QueueItem(short _X, short _Y, short _Z, DateTime _DoneTime) {
            X = _X;
            Y = _Y;
            Z = _Z;
            DoneTime = _DoneTime;
        }
    }

    public delegate void CommandInvoker(NetworkClient Client, string[] args, string Text1, string Text2);
    public delegate void FillInvoker(HypercubeMap map, string[] args);

    public struct HistoryEntry {
        public int Timestamp { get; set; }
        public short x { get; set; }
        public short y { get; set; }
        public short z { get; set; }
        public ushort Player { get; set; }
        public ushort LastPlayer { get; set; }
        public byte NewBlock { get; set; }
        public byte LastBlock { get; set; }

        /// <summary>
        /// Loads a HistoryEntry from a byte array.
        /// </summary>
        /// <param name="Array"></param>
        public void FromByteArray(byte[] Array, short _x, short _y, short _z) {
            if (Array.Length != 10)
                throw new FormatException("The provided byte array is not 10 bytes long.");

            x = _x;
            y = _y;
            z = _z;

            Timestamp = BitConverter.ToInt32(Array, 0);
            Player = BitConverter.ToUInt16(Array, 4);
            LastPlayer = BitConverter.ToUInt16(Array, 6);
            NewBlock = Array[8];
            LastBlock = Array[9];
        }

        /// <summary>
        /// Serializes the history entry into a byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray() {
            var Result = new byte[10];

            Buffer.BlockCopy(BitConverter.GetBytes(Timestamp), 0, Result, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Player), 0, Result, 4, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(LastPlayer), 0, Result, 6, 2);

            Result[8] = NewBlock;
            Result[9] = LastBlock;

            return Result;
        }
    }

    public struct HistoryComparator : IEqualityComparer<HistoryEntry> {
        public bool Equals(HistoryEntry Item1, HistoryEntry Item2) {
            if (Item1.x == Item2.x && Item1.y == Item2.y && Item1.z == Item2.z)
                return true;
            else
                return false;
        }

        public int GetHashCode(HistoryEntry Item) {
            int hCode = Item.x ^ Item.y ^ Item.z;
            return hCode.GetHashCode();
        }
    }
}
