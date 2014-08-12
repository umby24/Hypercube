using System;
using System.Collections.Generic;

using Hypercube.Client;
using Hypercube.Network;
using Hypercube.Libraries;
using Hypercube.Map;

namespace Hypercube.Core {
    public class Chat {
        /// <summary>
        /// Sends a chat message to all players across all maps.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="log"></param>
        public static void SendGlobalChat(string message, sbyte messageType = 0, bool log = false) {
            var chat = new Message {PlayerId = messageType};

            message = Text.CleanseString(message);

            if (log)
                Hypercube.Logger.Log("Global", message, LogType.Chat);

            message = EmoteReplace(message);

            if (message.Length > 0 && Text.StringMatches(message.Substring(message.Length - 1)))
                message += ".";

            var sending = SplitLines(message);

            foreach (var c in Hypercube.Nh.ClientList) {
                foreach (var b in sending) {
                    chat.Text = b;
                    c.SendQueue.Enqueue(chat);
                }
            }
        
        }

        /// <summary>
        /// Sends a message to all clients on a certain map.
        /// </summary>
        public static void SendMapChat(HypercubeMap map, string message, sbyte messageType = 0, bool log = false) {
            var chat = new Message {PlayerId = messageType};

            message = Text.CleanseString(message);

            if (log)
                Hypercube.Logger.Log(map.CWMap.MapName, message, LogType.Chat);

            message = EmoteReplace(message);

            if (message.Length > 0 && Text.StringMatches(message.Substring(message.Length - 1)))
                message += ".";

            var sending = SplitLines(message);

            foreach(var c in map.ClientsList) {
                foreach (var b in sending) {
                    chat.Text = b;
                    c.SendQueue.Enqueue(chat);
                }
            }
            
        }

        /// <summary>
        /// Sends chat to an individual client
        /// </summary>
        public static void SendClientChat(NetworkClient client, string message, sbyte messageType = 0) {
            message = Text.CleanseString(message);
            message = EmoteReplace(message);

            if (!client.CS.CPEExtensions.ContainsKey("EmoteFix") && Text.StringMatches(message.Substring(message.Length - 1)))
                message += ".";

            var sending = SplitLines(message);

            var chat = new Message {PlayerId = messageType};

            foreach (var b in sending) {
                chat.Text = b;
                client.SendQueue.Enqueue(chat);
            }

        }

        /// <summary>
        /// Performs any required escaping operations on strings coming in from clients.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string FilterIncomingChat(string message) {
            message = message.Replace("%%", "§"); // -- Double %, player actually wants to type %.

            for (var i = 0; i < 10; i++)
                message = message.Replace("%" + i, "&" + i);

            for (var i = 97; i < 103; i++)
                message = message.Replace("%" + (char)i, "&" + (char)i);

            message = message.Replace("§", "%");
            message = message.Replace("<br>", ""); // -- Don't allow clients to create newlines :).
            message = message.Replace("^detail.user=", ""); // -- Filter out WoM Messages from clients.

            message = Text.CleanseString(message);

            return message;
        }

        public static void HandleIncomingChat(NetworkClient incomingClient, string message) {
            message = FilterIncomingChat(message);

            if (incomingClient.CS.MuteTime > Hypercube.GetCurrentUnixTime()) {
                SendClientChat(incomingClient, "§EYou are muted.");
                return;
            }

            Hypercube.Luahandler.RunFunction("E_ChatMessage", incomingClient, message);

            if (message.StartsWith("/") && !message.StartsWith("//"))
                Hypercube.Commandholder.HandleCommand(incomingClient, message);
            else if (message.StartsWith("@")) {
                string client;

                try {
                    client = message.Substring(1, message.IndexOf(" ") - 1);
                } catch {
                    return;
                }

                NetworkClient tosend;

                if (Hypercube.Nh.LoggedClients.ContainsKey(client)) {
                    tosend = Hypercube.Nh.LoggedClients[client];
                } else {
                    SendClientChat(incomingClient, "§EPlayer '" + client + "' not found.");
                    return;
                }

                SendClientChat(incomingClient, "&c@" + tosend.CS.FormattedName + "&f: " + message.Substring(message.IndexOf(" ") + 1, message.Length - (message.IndexOf(" ") + 1)));
                SendClientChat(tosend, "&c@" + incomingClient.CS.FormattedName + "&f: " + message.Substring(message.IndexOf(" ") + 1, message.Length - (message.IndexOf(" ") + 1)));
            } else if (message.StartsWith("#")) {
                message = message.Substring(1, message.Length - 1);

                if (incomingClient.CS.Global) {
                    SendMapChat(incomingClient.CS.CurrentMap, incomingClient.CS.FormattedName + "&f: " + message);
                    Hypercube.Logger.Log(incomingClient.CS.CurrentMap.CWMap.MapName, incomingClient.CS.LoginName + ": " + message, LogType.Chat);
                } else {
                    SendGlobalChat("&c#&f " + incomingClient.CS.FormattedName + "&f: " + message);
                    Hypercube.Logger.Log("Global", incomingClient.CS.LoginName + ": " + message, LogType.Chat);
                }
            } else {
                if (incomingClient.CS.Global) {
                    SendGlobalChat("&c#&f " + incomingClient.CS.FormattedName + "&f: " + message);
                    Hypercube.Logger.Log("Global", incomingClient.CS.LoginName + ": " + message, LogType.Chat);
                } else {
                    SendMapChat(incomingClient.CS.CurrentMap, incomingClient.CS.FormattedName + "&f: " + message);
                    Hypercube.Logger.Log(incomingClient.CS.CurrentMap.CWMap.MapName, incomingClient.CS.LoginName + ": " + message, LogType.Chat);
                }
            }
        }

        public static List<string> SplitBrs(string input) {
            var builder = new List<string>();

            while (input.IndexOf("<br>", StringComparison.OrdinalIgnoreCase) >= 0) {
                var index = input.IndexOf("<br>", StringComparison.OrdinalIgnoreCase);
                builder.Add(input.Substring(0, index)); // -- Add to our string builder
                input = input.Substring(index + 4, input.Length - (index + 4)); // -- Remove from Input the string, and discard the <br>.
            }

            // -- If there's any leftovers that wern't split, we will need to go ahead and add that as well.
            if (input != "")
                builder.Add(input);

            // -- If we miracously made it here without having to break the line, we will need to do this.
            if (builder.Count == 0)
                builder.Add(input);

            return builder;
        }

        /// <summary>
        /// Replaces in-game text codes with emotes (unicode).
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string EmoteReplace(string message) {
            message = message.Replace("{:)}", "\u0001"); // ☺
            message = message.Replace("{smile}", "\u0001");

            message = message.Replace("{smile2}", "\u0002"); // ☻

            message = message.Replace("{heart}", "\u0003"); // ♥
            message = message.Replace("{hearts}", "\u0003");
            message = message.Replace("{<3}", "\u0003");

            message = message.Replace("{diamond}", "\u0004"); // ♦
            message = message.Replace("{diamonds}", "\u0004");
            message = message.Replace("{rhombus}", "\u0004");

            message = message.Replace("{club}", "\u0005"); // ♣
            message = message.Replace("{clubs}", "\u0005");
            message = message.Replace("{clover}", "\u0005");
            message = message.Replace("{shamrock}", "\u0005");

            message = message.Replace("{spade}", "\u0006"); // ♠
            message = message.Replace("{spades}", "\u0006");

            message = message.Replace("{*}", "\u0007"); // •
            message = message.Replace("{bullet}", "\u0007");
            message = message.Replace("{dot}", "\u0007");
            message = message.Replace("{point}", "\u0007");

            message = message.Replace("{hole}", "\u0008"); // ◘

            message = message.Replace("{circle}", "\u0009"); // ○
            message = message.Replace("{o}", "\u0009");

            message = message.Replace("{male}", "\u000B"); // ♂
            message = message.Replace("{mars}", "\u000B");

            message = message.Replace("{female}", "\u000C"); // ♀
            message = message.Replace("{venus}", "\u000C");

            message = message.Replace("{8}", "\u000D"); // ♪
            message = message.Replace("{note}", "\u000D");
            message = message.Replace("{quaver}", "\u000D");

            message = message.Replace("{notes}", "\u000E"); // ♫
            message = message.Replace("{music}", "\u000E");

            message = message.Replace("{sun}", "\u000F"); // ☼
            message = message.Replace("{celestia}", "\u000F");

            message = message.Replace("{>>}", "\u0010"); // ►
            message = message.Replace("{right2}", "\u0010");

            message = message.Replace("{<<}", "\u0011"); // ◄
            message = message.Replace("{left2}", "\u0011");

            message = message.Replace("{updown}", "\u0012"); // ↕
            message = message.Replace("{^v}", "\u0012");

            message = message.Replace("{!!}", "\u0013"); // ‼

            message = message.Replace("{p}", "\u0014"); // ¶
            message = message.Replace("{para}", "\u0014");
            message = message.Replace("{pilcrow}", "\u0014");
            message = message.Replace("{paragraph}", "\u0014");

            message = message.Replace("{s}", "\u0015"); // §
            message = message.Replace("{sect}", "\u0015");
            message = message.Replace("{section}", "\u0015");

            message = message.Replace("{-}", "\u0016"); // ▬
            message = message.Replace("{_}", "\u0016");
            message = message.Replace("{bar}", "\u0016");
            message = message.Replace("{half}", "\u0016");

            message = message.Replace("{updown2}", "\u0017"); // ↨
            message = message.Replace("{^v_}", "\u0017");

            message = message.Replace("{^}", "\u0018"); // ↑
            message = message.Replace("{up}", "\u0018");

            message = message.Replace("{v}", "\u0019"); // ↓
            message = message.Replace("{down}", "\u0019");

            message = message.Replace("{>}", "\u001A"); // →
            message = message.Replace("{->}", "\u001A");
            message = message.Replace("{right}", "\u001A");

            message = message.Replace("{<}", "\u001B"); // ←
            message = message.Replace("{<-}", "\u001B");
            message = message.Replace("{left}", "\u001B");

            message = message.Replace("{l}", "\u001C"); // ∟
            message = message.Replace("{angle}", "\u001C");
            message = message.Replace("{corner}", "\u001C");

            message = message.Replace("{<>}", "\u001D"); // ↔
            message = message.Replace("{<->}", "\u001D");
            message = message.Replace("{leftright}", "\u001D");

            message = message.Replace("{^^}", "\u001E"); // ▲
            message = message.Replace("{up2}", "\u001E");

            message = message.Replace("{vv}", "\u001F"); // ▼
            message = message.Replace("{down2}", "\u001F");

            message = message.Replace("{house}", "\u007F"); // ⌂
            
            message = message.Replace("{caret}", "^");
            message = message.Replace("{hat}", "^");

            message = message.Replace("{tilde}", "~");
            message = message.Replace("{wave}", "~");

            message = message.Replace("{grave}", "`");
            message = message.Replace("{\"}", "`");
            return message;
        }
        /// <summary>
        /// Splits a long message into multiple lines as needed. Appends ">>" as needed. This will also pad messages if they are of incorrect length.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string[] SplitLines(string input) {
            var builder = new List<string>();

            if (input.Length <= 64 && input.IndexOf("<br>", StringComparison.OrdinalIgnoreCase) <= 0)
                return new[] { input.PadRight(64) };

            // -- The string is longer than 64 characters, or contains '<br>'.
            builder.AddRange(SplitBrs(input));
            string temp;

            // -- First, going to insert our own <br>'s wherever the string is too long.
            for (var i = 0; i < builder.Count; i++) {
                temp = "";

                while (builder[i].Length > 0) { // -- Going to use temp here so we don't mess up our original string
                    if (builder[i].Length > 64) {
                        var thisIndex = builder[i].Substring(0, 60).LastIndexOf(' '); // -- Split by words.

                        if (thisIndex == -1) // -- Just incase it's one spaceless string.
                            thisIndex = 60;

                        temp += builder[i].Substring(0, thisIndex) + "&3>><br>"; // -- Put the string before, with the seperator, and our break.

                        // -- Finally, Remove this part of the string from the original Builder[i], and add our newline seperators.
                        builder[i] = "&3>>&f" + builder[i].Substring(thisIndex + 1, builder[i].Length - (thisIndex + 1)); // -- It will now loop again for any subsequent breaks.
                    } else {
                        // -- Since Builder[i] is not (or is no longer) greater than 64 characters long, we can simply remove the whole thing :)
                        temp += builder[i];
                        builder[i] = "";
                    }
                }

                builder[i] = temp;
            }

            // -- Next, remove any "<br>"'s, and split up the line on either side of it.
            for (var z = 0; z < builder.Count; z++) {

                while (builder[z].IndexOf("<br>", StringComparison.OrdinalIgnoreCase) >= 0) {
                    temp = builder[z];
                    var index = builder[z].IndexOf("<br>", StringComparison.OrdinalIgnoreCase);
                    builder[z] = temp.Substring(0, index).PadRight(64);
                    builder.Insert(z + 1, temp.Substring(index + 4, temp.Length - (index + 4)));
                }

                // -- If there's any leftovers that wern't split, we will need to go ahead and add that as well.
                if (builder[z] != "")
                    builder[z] = builder[z].PadRight(64);
            }

            // -- If we miracously made it here without having to break the line, we will need to do this.
            if (builder.Count == 0)
                builder.Add(input.PadRight(64));

            return builder.ToArray(); // -- Return our nice array'd string :)
        }
    }
}
