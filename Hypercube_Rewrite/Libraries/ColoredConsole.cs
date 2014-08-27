using System;

namespace Hypercube.Libraries {
    class ColoredConsole {
        public static class ColorConvertingConsole {
            /// <summary>Allows using Minecraft color codes when writing text to Console.</summary>
            static ColorConvertingConsole() {
                DefaultColor = ConsoleColor.White;
            }


            /// <summary>Default color to use for messages, in absence of Minecraft color codes.</summary>
            public static ConsoleColor DefaultColor { get; set; }

            // A global lock for console writes is necessary to avoid race conditions with concurrent calls to WriteLine.
            // WriteLine modifies global state (Console.ForegroundColor/BackgroundColor) and ought not be interrupted.
            static readonly object ConsoleLock = new object();


            /// <summary>Writes the text representation of the specified array of objects
            /// to the standard output stream using the specified format information.
            /// Removes all ampersand-character sequences, and replaces MC colors with Console colors.</summary>
            /// <param name="format">A composite format string. </param>
            /// <param name="arg">An array of objects to write using <paramref name="format"/>.</param>
            /// <exception cref="T:System.IO.IOException">An I/O error occurred.</exception>
            /// <exception cref="T:System.ArgumentNullException"><paramref name="format"/> or <paramref name="arg"/> is null.</exception>
            /// <exception cref="T:System.FormatException">The format specification in <paramref name="format"/> is invalid.</exception>
            public static void Write(string format, params object[] arg) {
                if (arg == null) throw new ArgumentNullException("arg");
                if (arg.Length > 0) format = String.Format(format, arg);
                Write(format);
            }


            /// <summary>Writes the text representation of the specified array of objects, followed by the current line terminator,
            /// to the standard output stream using the specified format information.
            /// Removes all ampersand-character sequences, and replaces MC colors with Console colors.</summary>
            /// <param name="format">A composite format string.</param>
            /// <param name="arg">An array of objects to write using <paramref name="format"/>.</param>
            /// <exception cref="T:System.IO.IOException">An I/O error occurred.</exception>
            /// <exception cref="T:System.ArgumentNullException"><paramref name="format"/> or <paramref name="arg"/> is null.</exception>
            /// <exception cref="T:System.FormatException">The format specification in <paramref name="format"/> is invalid.</exception>
            public static void WriteLine(string format, params object[] arg) {
                if (arg == null) throw new ArgumentNullException("arg");
                if (arg.Length > 0) format = String.Format(format, arg);
                Write(format + Environment.NewLine);
            }


            /// <summary>Writes the specified message to the standard output stream.
            /// Removes all ampersand-character sequences, and replaces MC colors with Console colors.</summary>
            /// <param name="message">The message to write.</param>
            /// <exception cref="T:System.IO.IOException">An I/O error occurred.</exception>
            /// <exception cref="T:System.ArgumentNullException"><paramref name="message"/> is null.</exception>
            public static void WriteLine(string message) {
                Write(message + Environment.NewLine);
            }


            /// <summary> Writes the specified message followed by the current line terminator, to the standard output stream.
            /// Removes all ampersand-character sequences, and replaces MC colors with Console colors. </summary>
            /// <param name="message">The message to write.</param>
            /// <exception cref="T:System.IO.IOException">An I/O error occurred.</exception>
            /// <exception cref="T:System.ArgumentNullException"><paramref name="message"/> is null.</exception>
            public static void Write(string message) {
                if (message == null) throw new ArgumentNullException("message");
                var start = message.IndexOf('&');
                lock (ConsoleLock) {
                    Console.ForegroundColor = DefaultColor;
                    if (start == -1) {
                        // No ampersands detected! Just print it.
                        Console.Write(message);
                    } else {
                        // Colors detected. Split it up and print in fragments.
                        var lastInsert = 0;
                        while (start != -1) {
                            SetColor(lastInsert, message);
                            Console.Write(message.Substring(lastInsert, start - lastInsert));
                            lastInsert = Math.Min(start + 2, message.Length);
                            start = message.IndexOf('&', lastInsert);
                        }
                        SetColor(lastInsert, message);
                        Console.Write(message.Substring(lastInsert, message.Length - lastInsert));
                    }
                    Console.ForegroundColor = DefaultColor;
                }
            }


            // Sets the foreground and background Console colors, based on the last color code in given message.
            static void SetColor(int lastInsert, string message) {
                if (lastInsert <= 0) {
                    // No color codes have been encountered yet.
                    return;
                }
                var colorCode = message[lastInsert - 1];
                Console.ForegroundColor = MinecraftToConsoleColor(colorCode);
                Console.BackgroundColor = Console.ForegroundColor == ConsoleColor.Black ? ConsoleColor.DarkGray : ConsoleColor.Black;
            }


            // Maps Minecraft color codes to ConsoleColor. Case-insensitive.
            // Returns DefaultColor for any unrecognized code.
            static ConsoleColor MinecraftToConsoleColor(char mcColorCode) {
                switch (mcColorCode) {
                    case '0':
                        return ConsoleColor.Black;
                    case '1':
                        return ConsoleColor.DarkBlue;
                    case '2':
                        return ConsoleColor.DarkGreen;
                    case '3':
                        return ConsoleColor.DarkCyan;
                    case '4':
                        return ConsoleColor.DarkRed;
                    case '5':
                        return ConsoleColor.DarkMagenta;
                    case '6':
                        return ConsoleColor.DarkYellow;
                    case '7':
                        return ConsoleColor.Gray;
                    case '8':
                        return ConsoleColor.DarkGray;
                    case '9':
                        return ConsoleColor.Blue;
                    case 'a':
                    case 'A':
                        return ConsoleColor.Green;
                    case 'b':
                    case 'B':
                        return ConsoleColor.Cyan;
                    case 'c':
                    case 'C':
                        return ConsoleColor.Red;
                    case 'd':
                    case 'D':
                        return ConsoleColor.Magenta;
                    case 'e':
                    case 'E':
                        return ConsoleColor.Yellow;
                    case 'f':
                    case 'F':
                        return ConsoleColor.White;
                    default:
                        return DefaultColor;
                }
            }
        }
    }
}
