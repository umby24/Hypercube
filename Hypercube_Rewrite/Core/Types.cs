using System;
using System.Collections.Generic;

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
        public short X;
        public short Y;
        public short Z;
        public Block OldBlock;
        public Block NewBlock;
    }

    public struct Vector3S {
        public short X;
        public short Y;
        public short Z;
    }

    public struct BmStruct {
        public string Name;
        public string Plugin;
    }

    public struct QueueComparator : IEqualityComparer<QueueItem> {
        public bool Equals(QueueItem item1, QueueItem item2) {
            if (item1.X == item2.X && item1.Y == item2.Y && item1.Z == item2.Z)
                return true;
            
            return false;
        }

        public int GetHashCode(QueueItem item) {
            var hCode = item.X ^ item.Y ^ item.Z;
            return hCode.GetHashCode();
        }
    }

    public class QueueItem {
        public short X, Y, Z, Priority;
        public DateTime DoneTime;

        public QueueItem(short x, short y, short z, short priority) {
            X = x;
            Y = y;
            Z = z;
            Priority = priority;
        }
        public QueueItem(short x, short y, short z, DateTime doneTime) {
            X = x;
            Y = y;
            Z = z;
            DoneTime = doneTime;
        }
    }

    public delegate void CommandInvoker(NetworkClient client, string[] args, string text1, string text2);
    public delegate void FillInvoker(HypercubeMap map, string[] args);

    public struct HistoryEntry {
        public int Timestamp { get; set; }
        public short X { get; set; }
        public short Y { get; set; }
        public short Z { get; set; }
        public ushort Player { get; set; }
        public ushort LastPlayer { get; set; }
        public byte NewBlock { get; set; }
        public byte LastBlock { get; set; }

        /// <summary>
        /// Creates a HistoryEntry from a byte array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static HistoryEntry FromByteArray(byte[] array, short x, short y, short z) {
            if (array.Length != 10)
                throw new FormatException("The provided byte array is not 10 bytes long.");

            var myEntry = new HistoryEntry {
                X = x,
                Y = y,
                Z = z,

                Timestamp = BitConverter.ToInt32(array, 0),
                Player = BitConverter.ToUInt16(array, 4),
                LastPlayer = BitConverter.ToUInt16(array, 6),
                NewBlock = array[8],
                LastBlock = array[9],
            };

            return myEntry;
        }

        /// <summary>
        /// Serializes the history entry into a byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray() {
            var result = new byte[10];

            Buffer.BlockCopy(BitConverter.GetBytes(Timestamp), 0, result, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Player), 0, result, 4, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(LastPlayer), 0, result, 6, 2);

            result[8] = NewBlock;
            result[9] = LastBlock;

            return result;
        }

        public bool Equals(HistoryEntry item) {
            return X == item.X && Y == item.Y && Z == item.Z;
        }
    }

    public struct HistoryComparator : IEqualityComparer<HistoryEntry> {
        public bool Equals(HistoryEntry item1, HistoryEntry item2) {
            if (item1.X == item2.X && item1.Y == item2.Y && item1.Z == item2.Z)
                return true;
            
            return false;
        }

        public int GetHashCode(HistoryEntry item) {
            var hCode = item.X ^ item.Y ^ item.Z;
            return hCode.GetHashCode();
        }
    }

    public enum MapActions {
        Fill,
        Delete,
        Resize,
        Save,
        Load
    }
    public struct MapAction {
        public HypercubeMap Map;
        public MapActions Action;
        public string[] Arguments;
    }
}
