using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Hypercube_Classic.Libraries {
    public class Logging {
        public bool FileLogging = true;
        public string LogFile;
        private short Rotation = 0;

        public Logging(string _logFile, bool rotate, bool logging = true) {
            LogFile = _logFile;
            FileLogging = logging;

            if (!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");

            if (rotate && File.Exists("Logs\\" + LogFile + ".txt") && logging) {
                RotateLogs();
            }
        }

        /// <summary>
        /// Prints your message to the console, and if file logging is enabled, to the server's log.
        /// </summary>
        /// <param name="type">The log type. Types are Debug, Info, Warning, Error, Critical, Chat, Command, and notset.</param>
        /// <param name="module">The module that this log entry is coming from.</param>
        /// <param name="message">The acutal message this log is producing.</param>
        public void _Log(string type, string module, string message) {
            Console.WriteLine(DateTime.Now.ToShortTimeString() + " [" + type.ToUpper() + "] [" + module.ToUpper() + "] " + message);

            if (FileLogging)
                LogWrite(DateTime.Now.ToShortTimeString() + " [" + type.ToUpper() + "] [" + module.ToUpper() + "] " + message);
        }

        /// <summary>
        /// Rotate the server log file.
        /// </summary>
        public void RotateLogs() {
            string[] files = Directory.GetFiles("Logs");

            foreach (string path in files) {
                string fileName = path.Substring(path.LastIndexOf("\\") + 1, path.Length - (path.LastIndexOf("\\") + 1));

                if (fileName.Substring(0, LogFile.Length + 1) == LogFile + "_") { // -- If the file name ends in _, it is a rotated log.
                    short tempRotation = short.Parse(fileName.Substring(fileName.LastIndexOf("_") + 1, fileName.Length - (fileName.LastIndexOf("_") + 5))); // -- Get the rotation number for that log.

                    if (tempRotation == Rotation)
                        Rotation += 1;
                }
            }

            LogFile = LogFile + "_" + Rotation;
        }

        /// <summary>
        /// Writes a line to the server log file.
        /// </summary>
        /// <param name="line"></param>
        void LogWrite(string line) {
            File.AppendAllText("Logs\\" + LogFile + ".txt", line + "\n");
        }
    }
}
