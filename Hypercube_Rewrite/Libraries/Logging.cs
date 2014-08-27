using System;
using System.IO;

using Hypercube.Core;

namespace Hypercube.Libraries {
    

    public class Logging {
        #region Variables
        readonly object _logLock = new object();
        #endregion
        #region Events
        public delegate void MessageEventHandler(string message);
        public event MessageEventHandler DebugMessage;
        public event MessageEventHandler InfoMessage;
        public event MessageEventHandler WarningMessage;
        public event MessageEventHandler ErrorMessage;
        public event MessageEventHandler CriticalMessage;
        public event MessageEventHandler ChatMessage;
        public event MessageEventHandler CommandMessage;
        public event MessageEventHandler NotsetMessage;
        #endregion

        public Logging() {

            if (!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");

        }

        public void Log(string module, string message, LogType type = LogType.NotSet) {
            if (!ServerCore.ColoredConsole)
                Console.WriteLine(DateTime.Now.ToShortTimeString() + "> [" + type + "] [" + module + "] " + Text.RemoveColors(message));
            else {
                switch (type) {
                    case LogType.Debug:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.DebugConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.ConsoleModule) + " " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.ConsoleMessage));

                        if (DebugMessage != null)
                            DebugMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Info:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.InfoConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.ConsoleModule) + " " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.ConsoleMessage));

                        if (InfoMessage != null)
                            InfoMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Warning:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.WarningConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.ConsoleModule) + " " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.ConsoleMessage));

                        if (WarningMessage != null)
                            WarningMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Error:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.ErrorConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.ConsoleModule) + " " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.ConsoleMessage));

                        if (ErrorMessage != null)
                            ErrorMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Critical:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.CriticalConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.ConsoleModule) + " " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.ConsoleMessage));

                        if (CriticalMessage != null)
                            CriticalMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Chat:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.ChatConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.ConsoleModule) + " " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.ConsoleMessage));

                        if (ChatMessage != null)
                            ChatMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Command:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.CommandConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.ConsoleMessage));

                        if (CommandMessage != null)
                            CommandMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.NotSet:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.NotSetConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.ConsoleModule) + " " +
                            Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.ConsoleMessage));

                        if (NotsetMessage != null)
                            NotsetMessage(message);
                        break;
                }
            }

            if (ServerCore.LogOutput) {
                lock (_logLock) {
                    LogWrite(DateTime.Now.ToShortTimeString() + "> [" + type + "] [" + module + "] " + message);
                }
            }
            
        }

        public void RotateLogs() {
            var files = Directory.GetFiles("Logs");
            var rotation = 0;

            foreach (var path in files) {
                var fileName = path.Substring(path.LastIndexOf("\\") + 1, path.Length - (path.LastIndexOf("\\") + 1));

                if (fileName.Substring(0, ServerCore.Logfile.Length + 1) == ServerCore.Logfile + "_") { // -- If the file name ends in _, it is a rotated log.
                    var tempRotation = short.Parse(fileName.Substring(fileName.LastIndexOf("_") + 1, fileName.Length - (fileName.LastIndexOf("_") + 5))); // -- Get the rotation number for that log.

                    if (tempRotation == rotation)
                        rotation += 1;
                }
            }

            ServerCore.Logfile = ServerCore.Logfile + "_" + rotation;
        }

        public void LogWrite(string message) {
            File.AppendAllText("Logs\\" + ServerCore.Logfile + ".txt", message + "\n");
        }
    }
}
