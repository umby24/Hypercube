using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Hypercube.Libraries;
using Hypercube.Core;

namespace Hypercube.Map {
    

    public class MapHistory {
        #region Variables
        public List<HistoryEntry> Entries; //TODO: Look into possibly changing this..

        HypercubeMap ThisMap;
        string BaseName;
        bool Fragmented = false;
        #endregion

        /// <summary>
        /// Creates a new MapHistory object.
        /// </summary>
        /// <param name="Map">The map object to hold history for.</param>
        public MapHistory(HypercubeMap Map) {
            ThisMap = Map;
            BaseName = Map.Path.Substring(0, Map.Path.LastIndexOf('.'));

            if (!File.Exists(BaseName + ".hch")) {
                // -- Create a history file.
                using (FileStream stream = new FileStream(BaseName + ".hch", FileMode.Create)) {
                    var History = new byte[(ThisMap.CWMap.SizeX * ThisMap.CWMap.SizeY * ThisMap.CWMap.SizeZ) * 4];

                    stream.Write(History, 0, History.Length);

                    History = null;
                    GC.Collect();
                }

                if (Map.Loaded == false && Map.Servercore.CompressHistory)
                    GZip.CompressFile(BaseName + ".hch");
            }

            if (Map.Loaded && Map.Servercore.CompressHistory && FileCompressed())
                GZip.DecompressFile(BaseName + ".hch");
            else if (!Map.Servercore.CompressHistory) { // -- If the user recently disabled compression, we should decompress it first anyway.

                if (FileCompressed())
                    GZip.DecompressFile(BaseName + ".hch");
            }

            Entries = new List<HistoryEntry>();
        }

        public bool FileCompressed() {
            bool Compressed = false;

            using (var FS = new FileStream(BaseName + ".hch", FileMode.Open)) {
                int b1 = FS.ReadByte();
                int b2 = FS.ReadByte();

                if (b1 == 0x1f && b2 == 0x8B)
                    Compressed = true;
            }

            return Compressed;
        }
        /// <summary>
        /// Uncompresses the history file, if it was previously compressed.
        /// </summary>
        public void ReloadHistory() {
            ThisMap.Servercore.Logger.Log("MapHistory", "Reloaded history", LogType.Debug);

            if (ThisMap.Servercore.CompressHistory && ThisMap.Loaded == false && FileCompressed())
                GZip.DecompressFile(BaseName + ".hch");
        }

        /// <summary>
        /// Saves all cached entries, and compresses the history file (If setting enabled)
        /// </summary>
        public void UnloadHistory() {
            SaveEntries();
            ThisMap.Servercore.Logger.Log("MapHistory", "Unloaded history", LogType.Debug);

            if (ThisMap.Servercore.CompressHistory && ThisMap.Loaded == true && !FileCompressed())
                GZip.CompressFile(BaseName + ".hch");
        }

        /// <summary>
        /// Saves all history entries in the Entries list to file.
        /// </summary>
        public void SaveEntries() {
            ThisMap.Servercore.Logger.Log("MapHistory", "Saving Entries", LogType.Debug);
            int IndexTableSize = (ThisMap.CWMap.SizeX * ThisMap.CWMap.SizeY * ThisMap.CWMap.SizeZ) * 4;

            using (var FS = new FileStream(BaseName + ".hch", FileMode.Open)) {
                foreach (HistoryEntry h in Entries) {
                    var temp = new byte[4];
                    int index = (h.z * ThisMap.CWMap.SizeZ + h.y) * ThisMap.CWMap.SizeX + h.x;
                    
                    FS.Seek(index * 4, SeekOrigin.Begin);
                    FS.Read(temp, 0, 4);

                    int EntryIndex = BitConverter.ToInt32(temp, 0);

                    temp = null;

                    if (EntryIndex == 0) { // -- There are no entries for this block yet.
                        FS.Seek(0, SeekOrigin.End); // -- Seek to the end of the file.
                        int EndLocation = ((int)FS.Position - IndexTableSize); // -- Store the index for this entry

                        FS.WriteByte(1); // -- There is now 1 entry
                        FS.Write(h.ToByteArray(), 0, 10); // -- And this is the data.

                        FS.Seek(index * 4, SeekOrigin.Begin); // -- Seek back to the Int for this block.
                        FS.Write(BitConverter.GetBytes(EndLocation), 0, 4); // -- Write in the location for that block's entries.
                        continue; // -- Move on to the next HistoryEntry.
                    }

                    // -- There is already one or more entries for this block.
                    FS.Seek(IndexTableSize + EntryIndex, SeekOrigin.Begin); // -- Seek to the position for this block
                    int NumEntries = FS.ReadByte(); // -- And get the number of entries.

                    // -- Before adding anything, we'll check to see if this user already has an entry.
                    var TempArray = new HistoryEntry[NumEntries];
                    bool shift = false;

                    for (int i = 0; i < NumEntries; i++) { // -- Load the entries for checking.
                        var ThisEntry = new byte[10];

                        FS.Read(ThisEntry, 0, 10);
                        TempArray[i].FromByteArray(ThisEntry, h.x, h.y, h.z);

                        if (TempArray[i].Player == h.Player) { // -- If there is a player, update the new block, and the change time.
                            TempArray[i].Timestamp = h.Timestamp;
                            TempArray[i].NewBlock = h.NewBlock;
                            shift = true; // -- This entry now needs to be shifted.
                        }

                        ThisEntry = null;
                    }

                    if (shift) {
                        TempArray = TempArray.OrderBy(o => o.Timestamp).ToArray(); // -- If there was an entry, order the array...
                        FS.Seek((IndexTableSize + EntryIndex) + 1, SeekOrigin.Begin); // -- Seek to the beginning of the entries..

                        foreach (HistoryEntry z in TempArray) // -- Write the newly ordered entries
                            FS.Write(z.ToByteArray(), 0, 10);

                        TempArray = null;
                        continue; // -- Move to the next iteration.
                    }

                    // -- Now We'll handle the easy case first, if we've already reached the maximum.

                    if (NumEntries == ThisMap.Servercore.MaxHistoryEntries) {
                        TempArray[0] = h; // -- Overwrite the old entry with the newest one.
                        TempArray = TempArray.OrderBy(o => o.Timestamp).ToArray(); // -- Order the array by timestamp, so that our new entry is at the end.

                        FS.Seek((IndexTableSize + EntryIndex) + 1, SeekOrigin.Begin); // -- Seek to the beginning of the entries..

                        foreach (HistoryEntry z in TempArray) // -- Write the newly ordered entries
                            FS.Write(z.ToByteArray(), 0, 10);

                        TempArray = null;
                        continue; // -- Move on to the next iteration.
                    }

                    // -- And finally, the condition that causes fragmentation :( Actually creating an entry.

                    FS.Seek(0, SeekOrigin.End);
                    int EndPosition = ((int)FS.Position - IndexTableSize);
                    FS.WriteByte((byte)(NumEntries + 1));

                    foreach (HistoryEntry z in TempArray)
                        FS.Write(z.ToByteArray(), 0, 10);

                    FS.Write(h.ToByteArray(), 0, 10);

                    FS.Seek(index * 4, SeekOrigin.Begin); // -- Seek back to the Int for this block.
                    FS.Write(BitConverter.GetBytes(EndPosition), 0, 4); // -- Write in the location for that block's entries.

                    Fragmented = true;
                    TempArray = null;
                    // -- Annnd that's all folks!
                }
            }

            Entries.Clear();
        }

        /// <summary>
        /// Looks up all the entries for a given map coordinate in the history system.
        /// </summary>
        /// <param name="x">X location of block</param>
        /// <param name="y">Y Location of block</param>
        /// <param name="z">Z Location of block</param>
        /// <returns>Array of History Entries.</returns>
        public HistoryEntry[] Lookup(short x, short y, short z) {
            if (!ThisMap.HCSettings.History)
                return null;

            int IndexTableSize = (ThisMap.CWMap.SizeX * ThisMap.CWMap.SizeY * ThisMap.CWMap.SizeZ) * 4;
            var Result = new HistoryEntry[ThisMap.Servercore.MaxHistoryEntries];

            using (var FS = new FileStream(BaseName + ".hch", FileMode.Open)) {
                var temp = new byte[4];
                int index = (z * ThisMap.CWMap.SizeZ + y) * ThisMap.CWMap.SizeX + x;

                FS.Seek(index * 4, SeekOrigin.Begin); // -- Seek to the int in the index table
                FS.Read(temp, 0, 4);

                int EntryIndex = BitConverter.ToInt32(temp, 0); // -- This will give us the location of our entries (if any).

                temp = null;

                if (EntryIndex == 0) // -- No entries.
                    return null;

                FS.Seek((IndexTableSize + EntryIndex), SeekOrigin.Begin); // -- Seek to the entries.
                int NumEntries = FS.ReadByte(); // -- The number of entries to follow (max by design: 255).
                
                for (int i = 0; i < NumEntries; i++) { // -- Load the entries.
                    var ThisEntry = new byte[10];

                    FS.Read(ThisEntry, 0, 10);
                    Result[i].FromByteArray(ThisEntry, x, y, z);

                    ThisEntry = null;
                }
            }

            var MyList = new List<HistoryEntry>();
            bool Lebroke = false;

            // -- Now we've loaded the entries from file, and we must apply everything that has changed since then..
            foreach (HistoryEntry h in Entries) {
                if (Result.Contains(h, new HistoryComparator())) { // -- If the coords for this entry match the results
                    for (int q = 0; q < Result.Length; q++) {
                        if (Result[q].Player == h.Player) {
                            Result[q].NewBlock = h.NewBlock;
                            Result[q].Timestamp = h.Timestamp;
                            Lebroke = true;
                            break;
                        }
                    }

                    if (Lebroke) {
                        Lebroke = false;
                        continue;
                    }

                    MyList.Add(h); // -- Add it to the temporary list
                }
            }

            var TempList = Result.ToList();
            TempList.AddRange(MyList);
            MyList.Clear();

            // -- Sort the list by time..
            if (TempList.Count > ThisMap.Servercore.MaxHistoryEntries) {
                TempList = TempList.OrderByDescending(o => o.Timestamp).ToList();
                TempList.RemoveRange(ThisMap.Servercore.MaxHistoryEntries, TempList.Count - ThisMap.Servercore.MaxHistoryEntries);
                TempList = TempList.OrderBy(o => o.Timestamp).ToList();
                Result = TempList.ToArray();
            } else {
                TempList = TempList.OrderBy(o => o.Timestamp).ToList();
                Result = TempList.ToArray();
            }

            TempList = null;

            return Result;
        }

        /// <summary>
        /// Returns lookup results as a string.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public string LookupString(short x, short y, short z) {
            var Entries = Lookup(x, y, z);

            if (Entries == null || Entries.Length == 0)
                return "No Entries";
            else {
                string result = "";

                foreach (HistoryEntry e in Entries) 
                    result += "§S" + e.Player + " changed " + e.LastBlock + " to " + e.NewBlock + ".<br>";

                return result;
            }
        }

        public short GetLastPlayer(short x, short y, short z) {
            if (!ThisMap.HCSettings.History)
                return -1;

            var Results = Lookup(x, y, z);

            if (Results == null)
                return -1;
            else
                return (short)Results[Results.Length - 1].Player;
        }
        /// <summary>
        /// Creates an entry into the History System. Will not be saved until SaveEntries() is called.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="Player"></param>
        /// <param name="LastPlayer"></param>
        /// <param name="NewBlock"></param>
        /// <param name="LastBlock"></param>
        public void AddEntry(short x, short y, short z, ushort Player, ushort LastPlayer, byte NewBlock, byte LastBlock) {
            var HE = new HistoryEntry();
            HE.LastBlock = LastBlock;
            HE.NewBlock = NewBlock;
            HE.Player = Player;
            HE.LastPlayer = LastPlayer;
            HE.x = x;
            HE.y = y;
            HE.z = z;
            HE.Timestamp = (int)Hypercube.GetCurrentUnixTime();

            Entries.Add(HE);

            if (Entries.Count > 55000)
                SaveEntries();
        }
        
        /// <summary>
        /// Defragments the history file, if fragmented.
        /// </summary>
        public void DeFragment() {
            if (!Fragmented)
                return;

            if (ThisMap.Loaded == false)
                ReloadHistory();
            else
                SaveEntries();

            using (var FS = new FileStream(BaseName + ".hch",FileMode.Open, FileAccess.Read)) {
                using (var NFS = new FileStream(BaseName + ".temp", FileMode.Create)) {
                    var History = new byte[(ThisMap.CWMap.SizeX * ThisMap.CWMap.SizeY * ThisMap.CWMap.SizeZ) * 4]; // -- Create the index table in the new file.
                    NFS.Write(History, 0, History.Length);
                    History = null;

                    for (int ix = 0; ix < ThisMap.CWMap.SizeX; ix++) {
                        for (int iy = 0; iy < ThisMap.CWMap.SizeZ; iy++) {
                            for (int iz = 0; iz < ThisMap.CWMap.SizeY; iz++) {
                                var temp = new byte[4];
                                int index = (iz * ThisMap.CWMap.SizeZ + iy) * ThisMap.CWMap.SizeX + ix;

                                FS.Seek(index * 4, SeekOrigin.Begin);
                                FS.Read(temp, 0, 4);

                                int EntryIndex = BitConverter.ToInt32(temp, 0);

                                if (EntryIndex == 0)
                                    continue;

                                var Entries = Lookup((short)ix, (short)iy, (short)iz); // -- Gets the entries for this block..

                                NFS.Seek(0, SeekOrigin.End);
                                temp = BitConverter.GetBytes(NFS.Position);

                                NFS.WriteByte((byte)Entries.Length); // -- Write the number of entries..

                                foreach (HistoryEntry f in Entries) 
                                    NFS.Write(f.ToByteArray(), 0, 10); // -- Write the entries in.
                                
                                NFS.Seek(index * 4, SeekOrigin.Begin); // -- Seek back to the index table.
                                NFS.Write(temp, 0, 4); // -- And write in where our entries reside.

                                // -- Now on to the next coordinate!
                            }
                        }
                    }
                }
            }

            Fragmented = false;
            File.Delete(BaseName + ".hch");
            File.Move(BaseName + ".temp", BaseName + ".hch");

            if (ThisMap.Loaded == false)
                UnloadHistory();
        }
    }
}
