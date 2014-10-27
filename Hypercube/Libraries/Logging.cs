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
                var log = DateTime.Now.ToShortTimeString() + "> ";

                switch (type) {
                    case LogType.Debug:
                        log += Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.DebugConsole) + " ";

                        if (DebugMessage != null)
                            DebugMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Info:
                        log += Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.InfoConsole) + " ";

                        if (InfoMessage != null)
                            InfoMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Warning:
                        log += Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.WarningConsole) + " ";

                        if (WarningMessage != null)
                            WarningMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Error:
                        log += Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.ErrorConsole) + " ";

                        if (ErrorMessage != null)
                            ErrorMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Critical:
                        log += Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.CriticalConsole) + " ";

                        if (CriticalMessage != null)
                            CriticalMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Chat:
                        log += Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.ChatConsole) + " ";

                        if (ChatMessage != null)
                            ChatMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Command:
                        log += Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.CommandConsole) + " ";

                        if (CommandMessage != null)
                            CommandMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.NotSet:
                        log += Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.NotSetConsole) + " ";

                        if (NotsetMessage != null)
                            NotsetMessage(message);
                        break;
                }

                log += Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.ConsoleModule) + " ";
                log += Text.FormatString(module, type.ToString(), message, ServerCore.TextFormats.ConsoleMessage);
                ColoredConsole.ColorConvertingConsole.WriteLine(log);
            }

            if (!ServerCore.LogOutput) 
                return;

            lock (_logLock) {
                LogWrite(DateTime.Now.ToShortTimeString() + "> [" + type + "] [" + module + "] " + message);
            }
        }

        public void RotateLogs() {
            var files = Directory.GetFiles("Logs");
            var rotation = 0;

            foreach (var path in files) {
                var fileName = path.Substring(path.LastIndexOf("\\") + 1, path.Length - (path.LastIndexOf("\\") + 1));

                if (fileName.Substring(0, ServerCore.Logfile.Length + 1) != ServerCore.Logfile + "_") 
                    continue;

                // -- If the file name ends in _, it is a rotated log.
                var tempRotation = short.Parse(fileName.Substring(fileName.LastIndexOf("_") + 1, fileName.Length - (fileName.LastIndexOf("_") + 5))); // -- Get the rotation number for that log.

                if (tempRotation == rotation)
                    rotation += 1;
            }

            ServerCore.Logfile = ServerCore.Logfile + "_" + rotation;
        }

        public void LogWrite(string message) {
            File.AppendAllText("Logs\\" + ServerCore.Logfile + ".txt", message + "\n");
        }
    }
}
