using System;
using System.Collections.Generic;
using System.IO;

namespace Hypercube.Libraries {
    /// <summary>
    /// Public interface for easy settings management. This allows settings to be reloaded as they are changed.
    /// </summary>
    public class Settings {
        public string Filename { get; set; }
        public string CurrentGroup { get; set; }
        public DateTime LastModified { get; set; }
        public Dictionary<string, Dictionary<string, string>> SettingsDictionary { get; set; }
        public object LoadSettings { get; set; }
        public bool Save { get; set; }
    }

    public class PbSettingsLoader {
        public List<Settings> SettingsFiles;

        public delegate void LoadSettings();

        public PbSettingsLoader() {
            SettingsFiles = new List<Settings>();

            if (!Directory.Exists("Settings"))
                Directory.CreateDirectory("Settings");
        }

        /// <summary>
        /// Clears all the settings currently loaded, and completely reloads the settings file.
        /// This will also run the "LoadSettings" object once the file is loaded.
        /// </summary>
        /// <param name="settingsfile"></param>
        public void ReadSettings(Settings settingsfile) {
            if (!File.Exists("Settings/" + settingsfile.Filename))
                File.WriteAllText("Settings/" + settingsfile.Filename, "");

            PreLoad(settingsfile);
            settingsfile.LastModified = File.GetLastWriteTime("Settings/" + settingsfile.Filename);

            var myDele = (LoadSettings)settingsfile.LoadSettings;

            if (myDele != null)
                myDele();
        }

        /// <summary>
        /// Saves the settings to file.
        /// </summary>
        /// <param name="settingsFile"></param>
        public void SaveSettings(Settings settingsFile) {
            if (!settingsFile.Save)
                return;

            using (var fileWriter = new StreamWriter("Settings/" + settingsFile.Filename)) {
                foreach (var pair in settingsFile.SettingsDictionary) {
                    if (pair.Key != "")
                        fileWriter.WriteLine("[" + pair.Key + "]");

                    foreach (var subset in pair.Value)
                        fileWriter.WriteLine(subset.Key + " = " + subset.Value);
                }
            }

            settingsFile.LastModified = File.GetLastWriteTime("Settings/" + settingsFile.Filename);
        }

        /// <summary>
        /// Loads all of the groups and related settings for the file.
        /// </summary>
        /// <param name="settingsFile"></param>
        void PreLoad(Settings settingsFile) {
            settingsFile.CurrentGroup = "";
            settingsFile.SettingsDictionary.Clear();

            using (var sr = new StreamReader("Settings/" + settingsFile.Filename)) {
                while (!sr.EndOfStream) {
                    var thisLine = sr.ReadLine();

                    if (thisLine != null && thisLine.StartsWith(";")) // -- Comment
                        continue;
                    
                    if (thisLine != null && (thisLine.StartsWith("[") && thisLine.EndsWith("]"))) { // -- Group. 
                        if (settingsFile.SettingsDictionary.ContainsKey(thisLine.Substring(1, thisLine.Length - 2)))
                            continue;

                        settingsFile.SettingsDictionary.Add(thisLine.Substring(1, thisLine.Length - 2), new Dictionary<string, string>());
                        settingsFile.CurrentGroup = thisLine.Substring(1, thisLine.Length - 2);
                        continue;
                    }

                    if (thisLine != null && thisLine.Contains("=")) { // -- Setting.
                        if (settingsFile.CurrentGroup == "" && !settingsFile.SettingsDictionary.ContainsKey(""))
                            settingsFile.SettingsDictionary.Add("", new Dictionary<string, string>());

                        if (settingsFile.SettingsDictionary[settingsFile.CurrentGroup].ContainsKey(thisLine.Substring(0, thisLine.IndexOf("=")).TrimEnd(' ')))
                            settingsFile.SettingsDictionary[settingsFile.CurrentGroup][thisLine.Substring(0, thisLine.IndexOf("=")).TrimEnd(' ')] = thisLine.Substring(thisLine.IndexOf("=") + 1, thisLine.Length - (thisLine.IndexOf("=") + 1)).TrimStart(' ');
                        else
                            settingsFile.SettingsDictionary[settingsFile.CurrentGroup].Add(thisLine.Substring(0, thisLine.IndexOf("=")).TrimEnd(' '), thisLine.Substring(thisLine.IndexOf("=") + 1, thisLine.Length - (thisLine.IndexOf("=") + 1)).TrimStart(' '));

                    }
                }
            }
        }

        /// <summary>
        /// Selects a different group of settings. If the group does not exist, it is created.
        /// </summary>
        /// <param name="settingsFile"></param>
        /// <param name="groupName"></param>
        public Settings SelectGroup(Settings settingsFile, string groupName) {
            if (settingsFile.SettingsDictionary.ContainsKey(groupName))
                settingsFile.CurrentGroup = groupName;
            else {
                settingsFile.SettingsDictionary.Add(groupName, new Dictionary<string, string>());
                settingsFile.CurrentGroup = groupName;
            }

            return settingsFile;
        }

        /// <summary>
        /// Reads an individual value from a settings object. If the setting does not exist, an entry is created with the given defaultValue.
        /// </summary>
        /// <param name="settingsFile"></param>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <returns>string</returns>
        public string ReadSetting(Settings settingsFile, string key, string def) {
            if (!settingsFile.SettingsDictionary.ContainsKey(settingsFile.CurrentGroup)) {
                settingsFile.SettingsDictionary.Add(settingsFile.CurrentGroup, new Dictionary<string, string>());
                settingsFile.CurrentGroup = settingsFile.CurrentGroup;
                settingsFile.SettingsDictionary[settingsFile.CurrentGroup].Add(key, def);
                return def;
            }

            if (!settingsFile.SettingsDictionary[settingsFile.CurrentGroup].ContainsKey(key)) {
                settingsFile.SettingsDictionary[settingsFile.CurrentGroup].Add(key, def);
                return def;
            }

            return settingsFile.SettingsDictionary[settingsFile.CurrentGroup][key];
        }

        /// <summary>
        /// Reads an individual value from a settings object. If the setting does not exist, an entry is created with the given defaultValue.
        /// </summary>
        /// <param name="settingsFile"></param>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <returns>int</returns>
        public int ReadSetting(Settings settingsFile, string key, int def) {
            if (!settingsFile.SettingsDictionary[settingsFile.CurrentGroup].ContainsKey(key)) {
                settingsFile.SettingsDictionary[settingsFile.CurrentGroup].Add(key, def.ToString());
                return def;
            }

            return int.Parse(settingsFile.SettingsDictionary[settingsFile.CurrentGroup][key]);
        }

        /// <summary>
        /// Saves a setting with the given key and value. 
        /// </summary>
        /// <param name="settingsFile">The settings class to save this setting to.</param>
        /// <param name="settingsKey">The key for the setting.</param>
        /// <param name="settingsValue">The value of the setting.</param>
        public void SaveSetting(Settings settingsFile, string settingsKey, string settingsValue) {
            if (settingsFile.SettingsDictionary[settingsFile.CurrentGroup].ContainsKey(settingsKey))
                settingsFile.SettingsDictionary[settingsFile.CurrentGroup][settingsKey] = settingsValue;
            else
                settingsFile.SettingsDictionary[settingsFile.CurrentGroup].Add(settingsKey, settingsValue);
        }

        public Settings RegisterFile(string filename, bool save, LoadSettings readFunction) {
            var newSettings = new Settings {
                Filename = filename,
                CurrentGroup = "",
                SettingsDictionary = new Dictionary<string, Dictionary<string, string>>(),
                Save = save,
                LoadSettings = readFunction
            };

            SettingsFiles.Add(newSettings);

            return newSettings;
        }

        /// <summary>
        /// A loop that constantly checks the modified date of every setting file
        /// and reloads the settings file if it has been modified.
        /// This should be run in a thread.
        /// </summary>
        public void SettingsMain() {
            foreach (var t in SettingsFiles) {
                if (File.GetLastWriteTime("Settings/" + t.Filename) == t.LastModified) 
                    continue;

                ReadSettings(t);
                t.LastModified = File.GetLastWriteTime("Settings/" + t.Filename);
            }
            
        }
    }
}
