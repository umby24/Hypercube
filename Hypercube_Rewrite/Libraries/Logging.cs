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
        Hypercube Servercore;
        object LogLock = new object();
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

        public Logging(Hypercube core) {
            Servercore = core;

            if (!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");

        }

        public void Log(string module, string message, LogType type = LogType.NotSet) {
            if (!Servercore.ColoredConsole)
                Console.WriteLine(DateTime.Now.ToShortTimeString() + "> [" + type.ToString() + "] [" + module + "] " + Text.RemoveColors(message));
            else {
                switch (type) {
                    case LogType.Debug:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.DebugConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.ConsoleModule) + " " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.ConsoleMessage));

                        if (DebugMessage != null)
                            DebugMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Info:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.InfoConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.ConsoleModule) + " " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.ConsoleMessage));

                        if (InfoMessage != null)
                            InfoMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Warning:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.WarningConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.ConsoleModule) + " " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.ConsoleMessage));

                        if (WarningMessage != null)
                            WarningMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Error:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.ErrorConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.ConsoleModule) + " " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.ConsoleMessage));

                        if (ErrorMessage != null)
                            ErrorMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Critical:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.CriticalConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.ConsoleModule) + " " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.ConsoleMessage));

                        if (CriticalMessage != null)
                            CriticalMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Chat:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.ChatConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.ConsoleModule) + " " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.ConsoleMessage));

                        if (ChatMessage != null)
                            ChatMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.Command:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.CommandConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.ConsoleMessage));

                        if (CommandMessage != null)
                            CommandMessage(DateTime.Now.ToShortTimeString() + "> [" + module + "] " + message);
                        break;
                    case LogType.NotSet:
                        ColoredConsole.ColorConvertingConsole.WriteLine(DateTime.Now.ToShortTimeString() + "> " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.NotSetConsole) + " " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.ConsoleModule) + " " +
                            Text.FormatString(module, type.ToString(), message, Servercore.TextFormats.ConsoleMessage));

                        if (NotsetMessage != null)
                            NotsetMessage(message);
                        break;
                }
            }

            if (Servercore.LogOutput) {
                lock (LogLock) {
                    LogWrite(DateTime.Now.ToShortTimeString() + "> [" + type.ToString() + "] [" + module + "] " + message);
                }
            }
            
        }

        public void RotateLogs() {
            string[] files = Directory.GetFiles("Logs");
            int Rotation = 0;

            foreach (string path in files) {
                string fileName = path.Substring(path.LastIndexOf("\\") + 1, path.Length - (path.LastIndexOf("\\") + 1));

                if (fileName.Substring(0, Servercore.Logfile.Length + 1) == Servercore.Logfile + "_") { // -- If the file name ends in _, it is a rotated log.
                    short tempRotation = short.Parse(fileName.Substring(fileName.LastIndexOf("_") + 1, fileName.Length - (fileName.LastIndexOf("_") + 5))); // -- Get the rotation number for that log.

                    if (tempRotation == Rotation)
                        Rotation += 1;
                }
            }

            Servercore.Logfile = Servercore.Logfile + "_" + Rotation;
        }

        public void LogWrite(string message) {
            File.AppendAllText("Logs\\" + Servercore.Logfile + ".txt", message + "\n");
        }
    }
}
