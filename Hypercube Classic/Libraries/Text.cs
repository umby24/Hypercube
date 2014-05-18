using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Hypercube_Classic.Libraries;

namespace Hypercube_Classic.Libraries {
    public class Text {
        const string RegexString = "[^A-Za-z0-9!\\^\\~$%&/()=?{}\t\\[\\]\\\\ ,\\\";.:\\-_#'+*<>|@]|&.$|&.(&.)";

        public ISettings TextSettings;
        public string ErrorMessage; // -- Shortcut in text will be $E
        public string SystemMessage; // -- $S
        public string ExtPlayerList;
        public string DebugConsole, InfoConsole, WarningConsole, ErrorConsole, CriticalConsole, ChatConsole, CommandConsole, NotSetConsole;
        public string ConsoleModule, ConsoleMessage;
        public string Divider; // -- $D
        public Hypercube ServerCore;

        /// <summary>
        /// Replaces invalid chat characters with "*".
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string CleanseString(string Input, Hypercube Core = null) {
            if (Core != null) {
                Input = Input.Replace("§E", Core.TextFormats.ErrorMessage);
                Input = Input.Replace("§S", Core.TextFormats.SystemMessage);
                Input = Input.Replace("§D", Core.TextFormats.Divider);
            }

            var Matcher = new Regex(RegexString, RegexOptions.Multiline);
            return Matcher.Replace(Input, "*");
        }

        /// <summary>
        /// Returns true if an illegal character is inside of the given string.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static bool StringMatches(string Input) {
            var Matcher = new Regex(RegexString, RegexOptions.Multiline);
            return Matcher.IsMatch(Input);
        }

        public static string FormatString(string module, string type, string message, string Replace) {
            return Replace.Replace("#TYPE#", type).Replace("#MODULE#", module).Replace("#MESSAGE#", message);
        }

        public Text(Hypercube Core) {
            ServerCore = Core;

            TextSettings = new ISettings();
            TextSettings.Filename = "Colors.txt";
            TextSettings.Save = true;
            TextSettings.CurrentGroup = "";
            TextSettings.Settings = new Dictionary<string, Dictionary<string, string>>();
            TextSettings.LoadSettings = new PBSettingsLoader.LoadSettings(ReadTextSettings);

            Core.Settings.ReadSettings(TextSettings);
            Core.Settings.SettingsFiles.Add(TextSettings);
            
        }

        public void ReadTextSettings() {
            ErrorMessage = ServerCore.Settings.ReadSetting(TextSettings, "Error", "&4Error:&f ");
            SystemMessage = ServerCore.Settings.ReadSetting(TextSettings, "System", "&e");
            ExtPlayerList = ServerCore.Settings.ReadSetting(TextSettings, "ExtPlayerList", "&c");
            Divider = ServerCore.Settings.ReadSetting(TextSettings, "Divider", "&3|");

            // -- Console colors (Must be vanilla MC color codes, no shortcuts.)
            DebugConsole = ServerCore.Settings.ReadSetting(TextSettings, "DebugConsole", "&7[#TYPE#]");
            InfoConsole = ServerCore.Settings.ReadSetting(TextSettings, "InfoConsole", "&e[#TYPE#]");
            WarningConsole = ServerCore.Settings.ReadSetting(TextSettings, "WarningConsole", "&6[#TYPE#]");
            ErrorConsole = ServerCore.Settings.ReadSetting(TextSettings, "ErrorConsole", "&c[#TYPE#]");
            CriticalConsole = ServerCore.Settings.ReadSetting(TextSettings, "CriticalConsole", "&4[#TYPE#]");
            ChatConsole = ServerCore.Settings.ReadSetting(TextSettings, "ChatConsole", "&7[#TYPE#]");
            CommandConsole = ServerCore.Settings.ReadSetting(TextSettings, "CommandConsole", "&a[#TYPE#]");
            NotSetConsole = ServerCore.Settings.ReadSetting(TextSettings, "NotSetConsole", "&b[#TYPE#]");
            ConsoleModule = ServerCore.Settings.ReadSetting(TextSettings, "ConsoleModule", "&9[#MODULE#]");
            ConsoleMessage = ServerCore.Settings.ReadSetting(TextSettings, "ConsoleMessage", "&f #MESSAGE#");

            //ServerCore.Logger._Log("Text", "Text colors loaded", LogType.Info);
        }
    }
}
