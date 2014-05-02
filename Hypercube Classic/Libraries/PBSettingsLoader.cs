using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace Hypercube_Classic.Libraries {
    /// <summary>
    /// Public interface for easy settings management. This allows settings to be reloaded as they are changed.
    /// </summary>
    public interface ISettings {
        string Filename { get; set; }
        string CurrentGroup { get; set; }
        DateTime LastModified { get; set; }
        Dictionary<string, Dictionary<string, string>> Settings { get; set; }
        object LoadSettings { get; set; }
        bool Save { get; set; }
    }

    public class PBSettingsLoader {
        public List<ISettings> SettingsFiles;
        public Thread ReadingThead;
        Hypercube ServerCore;

        public delegate void LoadSettings();

        public PBSettingsLoader(Hypercube Core) {
            ServerCore = Core;
            SettingsFiles = new List<ISettings>();
        }

        /// <summary>
        /// Clears all the settings currently loaded, and completely reloads the settings file.
        /// This will also run the "LoadSettings" object once the file is loaded.
        /// </summary>
        /// <param name="Settingsfile"></param>
        public void ReadSettings(ISettings Settingsfile) {
            if (!File.Exists("Settings/" + Settingsfile.Filename))
                File.WriteAllText("Settings/" + Settingsfile.Filename, "");

            PreLoad(Settingsfile);
            Settingsfile.LastModified = File.GetLastWriteTime("Settings/" + Settingsfile.Filename);

            var myDele = (LoadSettings)Settingsfile.LoadSettings;

            if (myDele != null)
                myDele();
        }

        /// <summary>
        /// Saves the settings to file.
        /// </summary>
        /// <param name="SettingsFile"></param>
        public void SaveSettings(ISettings SettingsFile) {
            if (!SettingsFile.Save)
                return;

            using (var fileWriter = new StreamWriter("Settings/" + SettingsFile.Filename)) {
                foreach (KeyValuePair<string, Dictionary<string, string>> pair in SettingsFile.Settings) {
                    if (pair.Key != "")
                        fileWriter.WriteLine("[" + pair.Key + "]");

                    foreach (KeyValuePair<string, string> subset in pair.Value)
                        fileWriter.WriteLine(subset.Key + " = " + subset.Value);
                }
            }

            SettingsFile.LastModified = File.GetLastWriteTime(SettingsFile.Filename);
        }

        /// <summary>
        /// Loads all of the groups and related settings for the file.
        /// </summary>
        /// <param name="SettingsFile"></param>
        void PreLoad(ISettings SettingsFile) {
            SettingsFile.CurrentGroup = "";

            using (var SR = new StreamReader("Settings/" + SettingsFile.Filename)) {
                while (!SR.EndOfStream) {
                    string thisLine = SR.ReadLine();

                    if (thisLine.StartsWith(";")) // -- Comment
                        continue;

                    if (thisLine.StartsWith("[") && thisLine.EndsWith("]")) { // -- Group.
                        SettingsFile.Settings.Add(thisLine.Substring(1, thisLine.Length - 2), new Dictionary<string, string>());
                        SettingsFile.CurrentGroup = thisLine.Substring(1, thisLine.Length - 2);
                        continue;
                    }

                    if (thisLine.Contains("=")) { // -- Setting.
                        if (SettingsFile.CurrentGroup == "" && !SettingsFile.Settings.ContainsKey(""))
                            SettingsFile.Settings.Add("", new Dictionary<string, string>());

                        try {
                            SettingsFile.Settings[SettingsFile.CurrentGroup].Add(thisLine.Substring(0, thisLine.IndexOf("=")).TrimEnd(' '), thisLine.Substring(thisLine.IndexOf("=") + 1, thisLine.Length - (thisLine.IndexOf("=") + 1)).TrimStart(' '));
                        } catch {

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Selects a different group of settings. If the group does not exist, it is created.
        /// </summary>
        /// <param name="SettingsFile"></param>
        /// <param name="GroupName"></param>
        public ISettings SelectGroup(ISettings SettingsFile, string GroupName) {
            if (SettingsFile.Settings.ContainsKey(GroupName))
                SettingsFile.CurrentGroup = GroupName;
            else {
                SettingsFile.Settings.Add(GroupName, new Dictionary<string, string>());
                SettingsFile.CurrentGroup = GroupName;
            }

            return SettingsFile;
        }

        /// <summary>
        /// Reads an individual value from a settings object. If the setting does not exist, an entry is created with the given defaultValue.
        /// </summary>
        /// <param name="SettingsFile"></param>
        /// <param name="Key"></param>
        /// <param name="def"></param>
        /// <returns>string</returns>
        public string ReadSetting(ISettings SettingsFile, string Key, string def) {
            if (!SettingsFile.Settings[SettingsFile.CurrentGroup].ContainsKey(Key)) {
                SettingsFile.Settings[SettingsFile.CurrentGroup].Add(Key, def);
                return def;
            } else
                return SettingsFile.Settings[SettingsFile.CurrentGroup][Key];
        }

        /// <summary>
        /// Reads an individual value from a settings object. If the setting does not exist, an entry is created with the given defaultValue.
        /// </summary>
        /// <param name="SettingsFile"></param>
        /// <param name="Key"></param>
        /// <param name="def"></param>
        /// <returns>int</returns>
        public int ReadSetting(ISettings SettingsFile, string Key, int def) {
            if (!SettingsFile.Settings[SettingsFile.CurrentGroup].ContainsKey(Key)) {
                SettingsFile.Settings[SettingsFile.CurrentGroup].Add(Key, def.ToString());
                return def;
            } else
                return int.Parse(SettingsFile.Settings[SettingsFile.CurrentGroup][Key]);
        }

        public void SaveSetting(ISettings SettingsFile, string settingsKey, string settingsValue) {
            if (SettingsFile.Settings[SettingsFile.CurrentGroup].ContainsKey(settingsKey))
                SettingsFile.Settings[SettingsFile.CurrentGroup][settingsKey] = settingsValue;
            else
                SettingsFile.Settings[SettingsFile.CurrentGroup].Add(settingsKey, settingsValue);
        }

        public void SettingsMain() {
            while (ServerCore.Running) {
                foreach (ISettings c in SettingsFiles) {
                    if (File.GetLastWriteTime("Settings/" + c.Filename) != c.LastModified)
                        ReadSettings(c);
                }

                Thread.Sleep(1000);
            }
        }
    }
}
