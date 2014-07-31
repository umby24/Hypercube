using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace Hypercube.Libraries {
    /// <summary>
    /// Public interface for easy settings management. This allows settings to be reloaded as they are changed.
    /// </summary>
    public class ISettings {
        public string Filename { get; set; }
        public string CurrentGroup { get; set; }
        public DateTime LastModified { get; set; }
        public Dictionary<string, Dictionary<string, string>> Settings { get; set; }
        public object LoadSettings { get; set; }
        public bool Save { get; set; }
    }

    public class PBSettingsLoader {
        public List<ISettings> SettingsFiles;
        public Thread ReadingThead;
        Hypercube ServerCore;

        public delegate void LoadSettings();

        public PBSettingsLoader(Hypercube Core) {
            ServerCore = Core;
            SettingsFiles = new List<ISettings>();

            if (!Directory.Exists("Settings"))
                Directory.CreateDirectory("Settings");
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

            SettingsFile.LastModified = File.GetLastWriteTime("Settings/" + SettingsFile.Filename);
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
                        if (SettingsFile.Settings.ContainsKey(thisLine.Substring(1, thisLine.Length - 2)))
                            continue;

                        SettingsFile.Settings.Add(thisLine.Substring(1, thisLine.Length - 2), new Dictionary<string, string>());
                        SettingsFile.CurrentGroup = thisLine.Substring(1, thisLine.Length - 2);
                        continue;
                    }

                    if (thisLine.Contains("=")) { // -- Setting.
                        if (SettingsFile.CurrentGroup == "" && !SettingsFile.Settings.ContainsKey(""))
                            SettingsFile.Settings.Add("", new Dictionary<string, string>());

                        if (SettingsFile.Settings[SettingsFile.CurrentGroup].ContainsKey(thisLine.Substring(0, thisLine.IndexOf("=")).TrimEnd(' ')))
                            SettingsFile.Settings[SettingsFile.CurrentGroup][thisLine.Substring(0, thisLine.IndexOf("=")).TrimEnd(' ')] = thisLine.Substring(thisLine.IndexOf("=") + 1, thisLine.Length - (thisLine.IndexOf("=") + 1)).TrimStart(' ');
                        else
                            SettingsFile.Settings[SettingsFile.CurrentGroup].Add(thisLine.Substring(0, thisLine.IndexOf("=")).TrimEnd(' '), thisLine.Substring(thisLine.IndexOf("=") + 1, thisLine.Length - (thisLine.IndexOf("=") + 1)).TrimStart(' '));

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
            if (!SettingsFile.Settings.ContainsKey(SettingsFile.CurrentGroup)) {
                SettingsFile.Settings.Add(SettingsFile.CurrentGroup, new Dictionary<string, string>());
                SettingsFile.CurrentGroup = SettingsFile.CurrentGroup;
                SettingsFile.Settings[SettingsFile.CurrentGroup].Add(Key, def);
                return def;
            }

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

        /// <summary>
        /// Saves a setting with the given key and value. 
        /// </summary>
        /// <param name="SettingsFile">The settings class to save this setting to.</param>
        /// <param name="settingsKey">The key for the setting.</param>
        /// <param name="settingsValue">The value of the setting.</param>
        public void SaveSetting(ISettings SettingsFile, string settingsKey, string settingsValue) {
            if (SettingsFile.Settings[SettingsFile.CurrentGroup].ContainsKey(settingsKey))
                SettingsFile.Settings[SettingsFile.CurrentGroup][settingsKey] = settingsValue;
            else
                SettingsFile.Settings[SettingsFile.CurrentGroup].Add(settingsKey, settingsValue);
        }

        public ISettings RegisterFile(string filename, bool save, LoadSettings ReadFunction) {
            var NewSettings = new ISettings();
            NewSettings.Filename = filename;
            NewSettings.CurrentGroup = "";
            NewSettings.Settings = new Dictionary<string,Dictionary<string,string>>();
            NewSettings.Save = save;
            NewSettings.LoadSettings = ReadFunction;

            SettingsFiles.Add(NewSettings);

            return NewSettings;
        }

        /// <summary>
        /// A loop that constantly checks the modified date of every setting file
        /// and reloads the settings file if it has been modified.
        /// This should be run in a thread.
        /// </summary>
        public void SettingsMain() {
            while (ServerCore.Running) {
                for (int i = 0; i < SettingsFiles.Count; i++) {
                    if (File.GetLastWriteTime("Settings/" + SettingsFiles[i].Filename) != SettingsFiles[i].LastModified) {
                        ReadSettings(SettingsFiles[i]);
                        SettingsFiles[i].LastModified = File.GetLastWriteTime("Settings/" + SettingsFiles[i].Filename);
                    }
                }

                Thread.Sleep(1000);
            }
        }
    }
}
