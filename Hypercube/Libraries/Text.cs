using System.Text.RegularExpressions;

namespace Hypercube.Libraries {
    public class Text {
        const string RegexString = "[^A-Za-z0-9!\\^\\~$%&/()=?{}\t\\[\\]\\\\ ,\\\";.:\\-_#'+*<>|@]|&.$|&.(&.)";

        public Settings TextSettings;
        public string ErrorMessage; // -- Shortcut in text will be $E
        public string SystemMessage; // -- $S
        public string ExtPlayerList;
        public string DebugConsole, InfoConsole, WarningConsole, ErrorConsole, CriticalConsole, ChatConsole, CommandConsole, NotSetConsole;
        public string ConsoleModule, ConsoleMessage;
        public string Divider; // -- $D

        public Text() {
            TextSettings = new Settings("Colors.txt", ReadTextSettings);
            ServerCore.Setting.RegisterFile(TextSettings);
            TextSettings.LoadFile();
        }

        /// <summary>
        /// Replaces invalid chat characters with "*".
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CleanseString(string input) {
            input = input.Replace("§E", ServerCore.TextFormats.ErrorMessage);
            input = input.Replace("§S", ServerCore.TextFormats.SystemMessage);
            input = input.Replace("§D", ServerCore.TextFormats.Divider);

            var matcher = new Regex(RegexString, RegexOptions.Multiline);
            return matcher.Replace(input, "*");
        }

        /// <summary>
        /// Returns true if an illegal character is inside of the given string.
        /// </summary>
        /// <returns></returns>
        public static bool StringMatches(string input) {
            var matcher = new Regex(RegexString, RegexOptions.Multiline);
            return matcher.IsMatch(input);
        }

        /// <summary>
        /// Replaces special codes to aid in console formatting.
        /// </summary>
        /// <param name="module">The module this console message is coming from.</param>
        /// <param name="type">The logging type.</param>
        /// <param name="message">The primary message.</param>
        /// <param name="replace">The string to format.</param>
        /// <returns>Formatted console string</returns>
        public static string FormatString(string module, string type, string message, string replace) {
            return replace.Replace("#TYPE#", type).Replace("#MODULE#", module).Replace("#MESSAGE#", message);
        }

        /// <summary>
        /// Removes color codes from messages.
        /// </summary>
        /// <param name="input">The text to strip color codes from.</param>
        /// <returns>Non-colored text</returns>
        public static string RemoveColors(string input) {
            input = input.Replace("&0", "");
            input = input.Replace("&1", "");
            input = input.Replace("&2", "");
            input = input.Replace("&3", "");
            input = input.Replace("&4", "");
            input = input.Replace("&5", "");
            input = input.Replace("&6", "");
            input = input.Replace("&7", "");
            input = input.Replace("&8", "");
            input = input.Replace("&9", "");

            input = input.Replace("&A", "");
            input = input.Replace("&B", "");
            input = input.Replace("&C", "");
            input = input.Replace("&D", "");
            input = input.Replace("&E", "");
            input = input.Replace("&F", "");

            input = input.Replace("&a", "");
            input = input.Replace("&b", "");
            input = input.Replace("&c", "");
            input = input.Replace("&d", "");
            input = input.Replace("&e", "");
            input = input.Replace("&f", "");

            return input;
        }

        /// <summary>
        /// Parses the text settings from file.
        /// </summary>
        public void ReadTextSettings() {
            ErrorMessage = TextSettings.Read("Error", "&4Error:&f ");
            SystemMessage = TextSettings.Read("System", "&e");
            ExtPlayerList = TextSettings.Read("ExtPlayerList", "&c");
            Divider = TextSettings.Read("Divider", "&3|");

            // -- Console colors (Must be vanilla MC color codes, no shortcuts.)
            DebugConsole = TextSettings.Read("DebugConsole", "&7[#TYPE#]");
            InfoConsole = TextSettings.Read("InfoConsole", "&e[#TYPE#]");
            WarningConsole = TextSettings.Read("WarningConsole", "&6[#TYPE#]");
            ErrorConsole = TextSettings.Read("ErrorConsole", "&c[#TYPE#]");
            CriticalConsole = TextSettings.Read("CriticalConsole", "&4[#TYPE#]");
            ChatConsole = TextSettings.Read("ChatConsole", "&7[#TYPE#]");
            CommandConsole = TextSettings.Read("CommandConsole", "&a[#TYPE#]");
            NotSetConsole = TextSettings.Read("NotSetConsole", "&b[#TYPE#]");
            ConsoleModule = TextSettings.Read("ConsoleModule", "&9[#MODULE#]");
            ConsoleMessage = TextSettings.Read("ConsoleMessage", "&f #MESSAGE#");
        }

        public void SaveTextSettings() {
            // -- Shortcut colors.
            TextSettings.Write("Error", ErrorMessage); 
            TextSettings.Write("System", SystemMessage);
            TextSettings.Write("ExtPlayerList", ExtPlayerList);
            TextSettings.Write("Divider", Divider);

            // -- Console colors (Must be vanilla MC color codes, no shortcuts.)
            TextSettings.Write("DebugConsole", DebugConsole);
            TextSettings.Write("InfoConsole", InfoConsole); 
            TextSettings.Write("WarningConsole", WarningConsole); 
            TextSettings.Write("ErrorConsole", ErrorConsole); 
            TextSettings.Write("CriticalConsole", CriticalConsole);
            TextSettings.Write("ChatConsole", ChatConsole);
            TextSettings.Write("CommandConsole", CommandConsole);
            TextSettings.Write("NotSetConsole", NotSetConsole);
            TextSettings.Write("ConsoleModule", ConsoleModule);
            TextSettings.Write("ConsoleMessage", ConsoleMessage);
        }
    }
}
