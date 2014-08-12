using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Hypercube.Core;

namespace Hypercube.Libraries {
    

    public class Logging {
        #region Variables
        readonly object LogLock = new object();
        #endregion
        #region Events
        public delegate void MessageEventHandler(string Message);
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
            if (!Hypercube.ColoredConsole)
                Console.WriteLine(DateTime.Now.ToShortTimeString() + "> [" + type.ToString() + "] [" + module + "] " + Text.RemoveColors(message));
            else {
                switch (type) {
                    case LogType.Debug:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.DebugConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.ConsoleModule) + " " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.ConsoleMessage));

                        if (DebugMessage != null)
                            DebugMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Info:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.InfoConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.ConsoleModule) + " " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.ConsoleMessage));

                        if (InfoMessage != null)
                            InfoMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Warning:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.WarningConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.ConsoleModule) + " " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.ConsoleMessage));

                        if (WarningMessage != null)
                            WarningMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Error:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.ErrorConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.ConsoleModule) + " " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.ConsoleMessage));

                        if (ErrorMessage != null)
                            ErrorMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Critical:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.CriticalConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.ConsoleModule) + " " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.ConsoleMessage));

                        if (CriticalMessage != null)
                            CriticalMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Chat:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.ChatConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.ConsoleModule) + " " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.ConsoleMessage));

                        if (ChatMessage != null)
                            ChatMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Command:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.CommandConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.ConsoleMessage));

                        if (CommandMessage != null)
                            CommandMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.NotSet:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.NotSetConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.ConsoleModule) + " " +
                            Text.FormatString(module, type.ToString(), message, Hypercube.TextFormats.ConsoleMessage));

                        if (NotsetMessage != null)
                            NotsetMessage(message);
                        break;
                }
            }

            if (Hypercube.LogOutput) {
                lock (LogLock) {
                    LogWrite(DateTime.Now.ToShortTimeString() + "> [" + type + "] [" + module + "] " + message);
                }
            }
            
        }

        public void RotateLogs() {
            var files = Directory.GetFiles("Logs");
            var rotation = 0;

            foreach (var path in files) {
                var fileName = path.Substring(path.LastIndexOf("\\") + 1, path.Length - (path.LastIndexOf("\\") + 1));

                if (fileName.Substring(0, Hypercube.Logfile.Length + 1) == Hypercube.Logfile + "_") { // -- If the file name ends in _, it is a rotated log.
                    var tempRotation = short.Parse(fileName.Substring(fileName.LastIndexOf("_") + 1, fileName.Length - (fileName.LastIndexOf("_") + 5))); // -- Get the rotation number for that log.

                    if (tempRotation == rotation)
                        rotation += 1;
                }
            }

            Hypercube.Logfile = Hypercube.Logfile + "_" + rotation;
        }

        public void LogWrite(string message) {
            File.AppendAllText("Logs\\" + Hypercube.Logfile + ".txt", message + "\n");
        }
    }
}
