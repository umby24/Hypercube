using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hypercube.Core;
using Hypercube.Libraries;

namespace Hypercube.Map
{
    public class MapHistory
    {
        #region Variables

        private readonly string _baseName;
        private readonly HypercubeMap _thisMap;
        public List<HistoryEntry> Entries; //TODO: Look into possibly changing this..
        private bool _fragmented;

        #endregion

        /// <summary>
        ///     Creates a new MapHistory object.
        /// </summary>
        /// <param name="map">The map object to hold history for.</param>
        public MapHistory(HypercubeMap map)
        {
            _thisMap = map;
            _baseName = map.Path.Substring(0, map.Path.LastIndexOf('.'));

            if (!File.Exists(_baseName + ".hch"))
            {
                // -- Create a history file.
                using (var stream = new FileStream(_baseName + ".hch", FileMode.Create))
                {
                    var history = new byte[(_thisMap.CWMap.SizeX*_thisMap.CWMap.SizeY*_thisMap.CWMap.SizeZ)*4];

                    stream.Write(history, 0, history.Length);

                    history = null;
                    GC.Collect();
                }

                if (map.Loaded == false && Hypercube.CompressHistory)
                    GZip.CompressFile(_baseName + ".hch");
            }

            if (map.Loaded && Hypercube.CompressHistory && FileCompressed())
                GZip.DecompressFile(_baseName + ".hch");
            else if (!Hypercube.CompressHistory)
            {
                // -- If the user recently disabled compression, we should decompress it first anyway.

                if (FileCompressed())
                    GZip.DecompressFile(_baseName + ".hch");
            }

            Entries = new List<HistoryEntry>();
        }

        public bool FileCompressed()
        {
            bool compressed = false;

            using (var fs = new FileStream(_baseName + ".hch", FileMode.Open))
            {
                var b1 = fs.ReadByte();
                var b2 = fs.ReadByte();

                if (b1 == 0x1f && b2 == 0x8B)
                    compressed = true;
            }

            return compressed;
        }

        /// <summary>
        ///     Uncompresses the history file, if it was previously compressed.
        /// </summary>
        public void ReloadHistory()
        {
            Hypercube.Logger.Log("MapHistory", "Reloaded history", LogType.Debug);

            if (Hypercube.CompressHistory && _thisMap.Loaded == false && FileCompressed())
                GZip.DecompressFile(_baseName + ".hch");
        }

        /// <summary>
        ///     Saves all cached entries, and compresses the history file (If setting enabled)
        /// </summary>
        public void UnloadHistory()
        {
            SaveEntries();
            Hypercube.Logger.Log("MapHistory", "Unloaded history", LogType.Debug);

            if (Hypercube.CompressHistory && _thisMap.Loaded && !FileCompressed())
                GZip.CompressFile(_baseName + ".hch");
        }

        /// <summary>
        ///     Saves all history entries in the Entries list to file.
        /// </summary>
        public void SaveEntries()
        {
            Hypercube.Logger.Log("MapHistory", "Saving Entries", LogType.Debug);
            int indexTableSize = (_thisMap.CWMap.SizeX*_thisMap.CWMap.SizeY*_thisMap.CWMap.SizeZ)*4;

            using (var fs = new FileStream(_baseName + ".hch", FileMode.Open))
            {
                foreach (var h in Entries)
                {
                    var temp = new byte[4];
                    var index = (h.Z*_thisMap.CWMap.SizeZ + h.Y)*_thisMap.CWMap.SizeX + h.X;

                    fs.Seek(index*4, SeekOrigin.Begin);
                    fs.Read(temp, 0, 4);

                    int entryIndex = BitConverter.ToInt32(temp, 0);

                    temp = null;

                    if (entryIndex == 0)
                    {
                        // -- There are no entries for this block yet.
                        fs.Seek(0, SeekOrigin.End); // -- Seek to the end of the file.
                        int endLocation = ((int) fs.Position - indexTableSize); // -- Store the index for this entry

                        fs.WriteByte(1); // -- There is now 1 entry
                        fs.Write(h.ToByteArray(), 0, 10); // -- And this is the data.

                        fs.Seek(index*4, SeekOrigin.Begin); // -- Seek back to the Int for this block.
                        fs.Write(BitConverter.GetBytes(endLocation), 0, 4);
                        // -- Write in the location for that block's entries.
                        continue; // -- Move on to the next HistoryEntry.
                    }

                    // -- There is already one or more entries for this block.
                    fs.Seek(indexTableSize + entryIndex, SeekOrigin.Begin); // -- Seek to the position for this block
                    int numEntries = fs.ReadByte(); // -- And get the number of entries.

                    // -- Before adding anything, we'll check to see if this user already has an entry.
                    var tempArray = new HistoryEntry[numEntries];
                    bool shift = false;

                    for (int i = 0; i < numEntries; i++)
                    {
                        // -- Load the entries for checking.
                        var thisEntry = new byte[10];

                        fs.Read(thisEntry, 0, 10);
                        tempArray[i].FromByteArray(thisEntry, h.X, h.Y, h.Z);

                        if (tempArray[i].Player == h.Player)
                        {
                            // -- If there is a player, update the new block, and the change time.
                            tempArray[i].Timestamp = h.Timestamp;
                            tempArray[i].NewBlock = h.NewBlock;
                            shift = true; // -- This entry now needs to be shifted.
                        }

                        thisEntry = null;
                    }

                    if (shift)
                    {
                        tempArray = tempArray.OrderBy(o => o.Timestamp).ToArray();
                        // -- If there was an entry, order the array...
                        fs.Seek((indexTableSize + entryIndex) + 1, SeekOrigin.Begin);
                        // -- Seek to the beginning of the entries..

                        foreach (HistoryEntry z in tempArray) // -- Write the newly ordered entries
                            fs.Write(z.ToByteArray(), 0, 10);

                        tempArray = null;
                        continue; // -- Move to the next iteration.
                    }

                    // -- Now We'll handle the easy case first, if we've already reached the maximum.

                    if (numEntries == Hypercube.MaxHistoryEntries)
                    {
                        tempArray[0] = h; // -- Overwrite the old entry with the newest one.
                        tempArray = tempArray.OrderBy(o => o.Timestamp).ToArray();
                        // -- Order the array by timestamp, so that our new entry is at the end.

                        fs.Seek((indexTableSize + entryIndex) + 1, SeekOrigin.Begin);
                        // -- Seek to the beginning of the entries..

                        foreach (HistoryEntry z in tempArray) // -- Write the newly ordered entries
                            fs.Write(z.ToByteArray(), 0, 10);

                        tempArray = null;
                        continue; // -- Move on to the next iteration.
                    }

                    // -- And finally, the condition that causes fragmentation :( Actually creating an entry.

                    fs.Seek(0, SeekOrigin.End);
                    int endPosition = ((int) fs.Position - indexTableSize);
                    fs.WriteByte((byte) (numEntries + 1));

                    foreach (HistoryEntry z in tempArray)
                        fs.Write(z.ToByteArray(), 0, 10);

                    fs.Write(h.ToByteArray(), 0, 10);

                    fs.Seek(index*4, SeekOrigin.Begin); // -- Seek back to the Int for this block.
                    fs.Write(BitConverter.GetBytes(endPosition), 0, 4);
                    // -- Write in the location for that block's entries.

                    _fragmented = true;
                    tempArray = null;
                    // -- Annnd that's all folks!
                }
            }

            Entries.Clear();
        }

        /// <summary>
        ///     Looks up all the entries for a given map coordinate in the history system.
        /// </summary>
        /// <param name="x">X location of block</param>
        /// <param name="y">Y Location of block</param>
        /// <param name="z">Z Location of block</param>
        /// <returns>Array of History Entries.</returns>
        public HistoryEntry[] Lookup(short x, short y, short z)
        {
            if (!_thisMap.HCSettings.History)
                return null;

            int indexTableSize = (_thisMap.CWMap.SizeX*_thisMap.CWMap.SizeY*_thisMap.CWMap.SizeZ)*4;
            var result = new HistoryEntry[Hypercube.MaxHistoryEntries];

            using (var fs = new FileStream(_baseName + ".hch", FileMode.Open))
            {
                var temp = new byte[4];
                var index = (z*_thisMap.CWMap.SizeZ + y)*_thisMap.CWMap.SizeX + x;

                fs.Seek(index*4, SeekOrigin.Begin); // -- Seek to the int in the index table
                fs.Read(temp, 0, 4);

                var entryIndex = BitConverter.ToInt32(temp, 0);
                // -- This will give us the location of our entries (if any).

                temp = null;

                if (entryIndex == 0) // -- No entries.
                    return null;

                fs.Seek((indexTableSize + entryIndex), SeekOrigin.Begin); // -- Seek to the entries.
                var numEntries = fs.ReadByte(); // -- The number of entries to follow (max by design: 255).

                for (var i = 0; i < numEntries; i++)
                {
                    // -- Load the entries.
                    var thisEntry = new byte[10];

                    fs.Read(thisEntry, 0, 10);
                    result[i].FromByteArray(thisEntry, x, y, z);

                    thisEntry = null;
                }
            }

            var myList = new List<HistoryEntry>();
            bool lebroke = false;

            // -- Now we've loaded the entries from file, and we must apply everything that has changed since then..
            foreach (var h in Entries)
            {
                if (!result.Contains(h, new HistoryComparator())) 
                    continue;

                // -- If the coords for this entry match the results
                for (var q = 0; q < result.Length; q++)
                {
                    if (result[q].Player != h.Player) 
                        continue;

                    result[q].NewBlock = h.NewBlock;
                    result[q].Timestamp = h.Timestamp;
                    lebroke = true;
                    break;
                }

                if (lebroke)
                {
                    lebroke = false;
                    continue;
                }

                myList.Add(h); // -- Add it to the temporary list
            }

            var tempList = result.ToList();
            tempList.AddRange(myList);
            myList.Clear();

            // -- Sort the list by time..
            if (tempList.Count > Hypercube.MaxHistoryEntries)
            {
                tempList = tempList.OrderByDescending(o => o.Timestamp).ToList();
                tempList.RemoveRange(Hypercube.MaxHistoryEntries,
                    tempList.Count - Hypercube.MaxHistoryEntries);
                tempList = tempList.OrderBy(o => o.Timestamp).ToList();
                result = tempList.ToArray();
            }
            else
            {
                tempList = tempList.OrderBy(o => o.Timestamp).ToList();
                result = tempList.ToArray();
            }

            tempList = null;

            return result;
        }

        /// <summary>
        ///     Returns lookup results as a string.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public string LookupString(short x, short y, short z)
        {
            var entries = Lookup(x, y, z);

            if (entries == null || entries.Length == 0)
                return "No Entries";

            var result = "";

            foreach (var e in entries)
                result += "§S" + e.Player + " changed " + e.LastBlock + " to " + e.NewBlock + ".<br>";

            return result;
        }

        public short GetLastPlayer(short x, short y, short z)
        {
            if (!_thisMap.HCSettings.History)
                return -1;

            HistoryEntry[] results = Lookup(x, y, z);

            if (results == null)
                return -1;
            return (short) results[results.Length - 1].Player;
        }

        /// <summary>
        ///     Creates an entry into the History System. Will not be saved until SaveEntries() is called.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="player"></param>
        /// <param name="lastPlayer"></param>
        /// <param name="newBlock"></param>
        /// <param name="lastBlock"></param>
        public void AddEntry(short x, short y, short z, ushort player, ushort lastPlayer, byte newBlock, byte lastBlock)
        {
            var he = new HistoryEntry
            {
                LastBlock = lastBlock,
                NewBlock = newBlock,
                Player = player,
                LastPlayer = lastPlayer,
                X = x,
                Y = y,
                Z = z,
                Timestamp = (int) Hypercube.GetCurrentUnixTime()
            };

            Entries.Add(he);

            if (Entries.Count > 55000)
                SaveEntries();
        }

        /// <summary>
        ///     Defragments the history file, if fragmented.
        /// </summary>
        public void DeFragment()
        {
            if (!_fragmented)
                return;

            if (_thisMap.Loaded == false)
                ReloadHistory();
            else
                SaveEntries();

            using (var fs = new FileStream(_baseName + ".hch", FileMode.Open, FileAccess.Read))
            {
                using (var nfs = new FileStream(_baseName + ".temp", FileMode.Create))
                {
                    var history = new byte[(_thisMap.CWMap.SizeX*_thisMap.CWMap.SizeY*_thisMap.CWMap.SizeZ)*4];
                    // -- Create the index table in the new file.
                    nfs.Write(history, 0, history.Length);
                    history = null;

                    for (var ix = 0; ix < _thisMap.CWMap.SizeX; ix++)
                    {
                        for (var iy = 0; iy < _thisMap.CWMap.SizeZ; iy++)
                        {
                            for (int iz = 0; iz < _thisMap.CWMap.SizeY; iz++)
                            {
                                var temp = new byte[4];
                                int index = (iz*_thisMap.CWMap.SizeZ + iy)*_thisMap.CWMap.SizeX + ix;

                                fs.Seek(index*4, SeekOrigin.Begin);
                                fs.Read(temp, 0, 4);

                                int entryIndex = BitConverter.ToInt32(temp, 0);

                                if (entryIndex == 0)
                                    continue;

                                HistoryEntry[] entries = Lookup((short) ix, (short) iy, (short) iz);
                                // -- Gets the entries for this block..

                                nfs.Seek(0, SeekOrigin.End);
                                temp = BitConverter.GetBytes(nfs.Position);

                                nfs.WriteByte((byte) entries.Length); // -- Write the number of entries..

                                foreach (HistoryEntry f in entries)
                                    nfs.Write(f.ToByteArray(), 0, 10); // -- Write the entries in.

                                nfs.Seek(index*4, SeekOrigin.Begin); // -- Seek back to the index table.
                                nfs.Write(temp, 0, 4); // -- And write in where our entries reside.

                                // -- Now on to the next coordinate!
                            }
                        }
                    }
                }
            }

            _fragmented = false;
            File.Delete(_baseName + ".hch");
            File.Move(_baseName + ".temp", _baseName + ".hch");

            if (_thisMap.Loaded == false)
                UnloadHistory();
        }
    }
}