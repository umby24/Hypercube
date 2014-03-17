using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Hypercube_Classic.Libraries {
    class PBSettingsLoader {
        string Filename;
        string CurrentGroup = "";
        public Dictionary<string, Dictionary<string, string>> DasSettings;

        public PBSettingsLoader(string SettingsFile) {
            if (!File.Exists(SettingsFile))
                throw new FileNotFoundException("File " + SettingsFile + " not found.");

            Filename = SettingsFile;
            DasSettings = new Dictionary<string, Dictionary<string, string>>();
            PreLoad();
        }

        void PreLoad() {
            using (var SR = new StreamReader(Filename)) {
                while (!SR.EndOfStream) {
                    string thisLine = SR.ReadLine();

                    if (thisLine.StartsWith(";")) // -- Comment
                        continue;

                    if (thisLine.StartsWith("[") && thisLine.EndsWith("]")) { // -- Group.
                        DasSettings.Add(thisLine.Substring(1, thisLine.Length - 2), new Dictionary<string, string>());
                        CurrentGroup = thisLine.Substring(1, thisLine.Length - 2);
                        continue;
                    }

                    if (thisLine.Contains("=")) { // -- Setting.
                        if (CurrentGroup == "" && !DasSettings.ContainsKey(""))
                            DasSettings.Add("", new Dictionary<string, string>());

                        DasSettings[CurrentGroup].Add(thisLine.Substring(0, thisLine.IndexOf("=")).TrimEnd(' '), thisLine.Substring(thisLine.IndexOf("=") + 1, thisLine.Length - (thisLine.IndexOf("=") + 1)).TrimStart(' '));
                    }

                }
            }
        }

        public void SelectGroup(string GroupName) {
            if (DasSettings.ContainsKey(GroupName))
                CurrentGroup = GroupName;
            else
                CurrentGroup = "";
        }

        /// <summary>
        /// Reads a setting from file.
        /// </summary>
        /// <param name="Key">The key of the setting</param>
        /// <param name="def">The default value to return if this setting cannot be found.</param>
        public string ReadSetting(string Key, string def) {
            if (!DasSettings[CurrentGroup].ContainsKey(Key))
                return def;
            else
                return DasSettings[CurrentGroup][Key];
        }

        public int ReadSetting(string Key, int def) {
            if (!DasSettings[CurrentGroup].ContainsKey(Key))
                return def;
            else
                return int.Parse(DasSettings[CurrentGroup][Key]);
        }

        public void SaveSetting() {

        }
    }
}
