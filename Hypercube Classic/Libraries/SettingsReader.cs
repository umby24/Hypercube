using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace Hypercube_Classic.Libraries {

    /// <summary>
    /// Public interface for easy settings management. This allows settings to be reloaded as they are changed.
    /// </summary>
    public interface ISettings {
        string Filename { get; set; }
        DateTime LastModified { get; set; }
        Dictionary<string, string> Settings { get; set; }
        object LoadSettings { get; set; }
    }

    /// <summary>
    /// Class for Loading and handling settings files.
    /// </summary>
    public class SettingsReader {
        public List<ISettings> SettingsFiles;
        public Thread ReadingThead;

        Hypercube ServerCore;

        public SettingsReader(Hypercube systemCore) {
            ServerCore = systemCore;
            SettingsFiles = new List<ISettings>();
        }

        public delegate void LoadSettings();

        /// <summary>
        /// Clears all the settings currently loaded, and completely reloads the settings file.
        /// This will also run the "LoadSettings" object once the file is loaded.
        /// </summary>
        /// <param name="Settingsfile"></param>
        public void ReadSettings(ISettings Settingsfile) {
            if (!File.Exists("Settings/" + Settingsfile.Filename)) {
                File.WriteAllText("Settings/" + Settingsfile.Filename, ""); // -- Create the file if it doesn't exist.
            }

            var fileReader = new StreamReader("Settings/" + Settingsfile.Filename);
            Settingsfile.Settings.Clear();

            while (!fileReader.EndOfStream) {
                string line = fileReader.ReadLine();

                if (!line.Contains("="))
                    continue;

                string key = line.Substring(0, line.IndexOf("=") - 1);
                string setting = line.Substring(line.IndexOf("=") + 2, line.Length - (line.IndexOf("=") + 2));

                try {
                    Settingsfile.Settings.Add(key, setting);
                } catch {

                }
            } 

            // -- Settings parsed.
            fileReader.Close();
            fileReader.Dispose();

            Settingsfile.LastModified = File.GetLastWriteTime("Settings/" + Settingsfile.Filename);

            var myDele = (LoadSettings)Settingsfile.LoadSettings;

            if (myDele != null)
                myDele();
            
        }

        /// <summary>
        /// Saves all settings in the file's dictonary.
        /// </summary>
        /// <param name="Settingsfile"></param>
        public void SaveSettings(ISettings Settingsfile) {
            var fileWriter = new StreamWriter("Settings/" + Settingsfile.Filename);

            foreach (KeyValuePair<string, string> pair in Settingsfile.Settings) {
                fileWriter.Write(pair.Key + " = " + pair.Value + "\r\n");
            }

            fileWriter.Close();
            fileWriter.Dispose();
            Settingsfile.LastModified = File.GetLastWriteTime(Settingsfile.Filename);
        }

        /// <summary>
        /// Reads an individual value from a settings object. If the setting does not exist, an entry is created with the given defaultValue.
        /// </summary>
        /// <param name="Settingsfile"></param>
        /// <param name="settingName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string ReadSetting(ISettings Settingsfile, string settingName, string defaultValue = "") {
            if (Settingsfile.Settings.ContainsKey(settingName))
                return Settingsfile.Settings[settingName];
            else {
                Settingsfile.Settings.Add(settingName, defaultValue);
                return defaultValue;
            }
        }

        /// <summary>
        /// Saves or updates an individual setting in a settings object.
        /// </summary>
        /// <param name="Settingsfile"></param>
        /// <param name="settingsKey"></param>
        /// <param name="settingsValue"></param>
        public void SaveSetting(ISettings Settingsfile, string settingsKey, string settingsValue) {
            if (Settingsfile.Settings.ContainsKey(settingsKey))
                Settingsfile.Settings[settingsKey] = settingsValue;
            else
                Settingsfile.Settings.Add(settingsKey, settingsValue);
        }

        /// <summary>
        /// Loops, checking for updates to settings files. If one is found, the file will be reloaded.
        /// </summary>
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
