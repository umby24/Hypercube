using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hypercube.Client;
using Hypercube.Network;
using Hypercube.Libraries;
using Hypercube.Map;

namespace Hypercube.Core {
    public class Chat {
        /// <summary>
        /// Sends a chat message to all players across all maps.
        /// </summary>
        /// <param name="Core"></param>
        public static void SendGlobalChat(Hypercube Core, string Message, sbyte MessageType = 0, bool Log = false) {
            var Chat = new Message();
            Chat.PlayerID = MessageType;

            Message = Text.CleanseString(Message, Core);

            if (Log)
                Core.Logger.Log("Global", Message, LogType.Chat);

            Message = EmoteReplace(Message);

            if (Message.Length > 0 && Text.StringMatches(Message.Substring(Message.Length - 1)))
                Message += ".";

            string[] Sending = SplitLines(Message);

            for (int i = 0; i < Core.nh.Clients.Count; i++) {
                if (!Core.nh.Clients[i].CS.LoggedIn)
                    continue;

                foreach (string b in Sending) {
                    Chat.Text = b;
                    Chat.Write(Core.nh.Clients[i]);
                }
            }
        }

        /// <summary>
        /// Sends a message to all clients on a certain map.
        /// </summary>
        public static void SendMapChat(HypercubeMap Map, Hypercube Core, string Message, sbyte MessageType = 0, bool Log = false) {
            var Chat = new Message();
            Chat.PlayerID = MessageType;

            Message = Text.CleanseString(Message, Core);

            if (Log)
                Core.Logger.Log(Map.CWMap.MapName, Message, LogType.Chat);

            Message = EmoteReplace(Message);

            if (Message.Length > 0 && Text.StringMatches(Message.Substring(Message.Length - 1)))
                Message += ".";

            string[] Sending = SplitLines(Message);

            for (int i = 0; i < Map.Clients.Count; i++) {
                foreach (string b in Sending) {
                    Chat.Text = b;
                    Chat.Write(Map.Clients[i]);
                }
            }
        }

        /// <summary>
        /// Sends chat to an individual client
        /// </summary>
        public static void SendClientChat(NetworkClient Client, string Message, sbyte MessageType = 0) {
            Message = Text.CleanseString(Message, Client.ServerCore);
            Message = EmoteReplace(Message);

            if (!Client.CS.CPEExtensions.ContainsKey("EmoteFix") && Text.StringMatches(Message.Substring(Message.Length - 1)))
                Message += ".";

            string[] Sending = SplitLines(Message);

            var Chat = new Message();
            Chat.PlayerID = MessageType;

            foreach (string b in Sending) {
                Chat.Text = b;
                Chat.Write(Client);
            }

        }

        /// <summary>
        /// Performs any required escaping operations on strings coming in from clients.
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static string FilterIncomingChat(string Message) {
            Message = Message.Replace("%%", "§"); // -- Double %, player actually wants to type %.

            for (int i = 0; i < 10; i++)
                Message = Message.Replace("%" + i.ToString(), "&" + i.ToString());

            for (int i = 97; i < 103; i++)
                Message = Message.Replace("%" + (char)i, "&" + (char)i);

            Message = Message.Replace("§", "%");
            Message = Message.Replace("<br>", ""); // -- Don't allow clients to create newlines :).
            Message = Message.Replace("^detail.user=", ""); // -- Filter out WoM Messages from clients.

            Message = Text.CleanseString(Message);

            return Message;
        }

        public static void HandleIncomingChat(NetworkClient IncomingClient, string Message) {
            Message = FilterIncomingChat(Message);

            if (IncomingClient.CS.MuteTime > Hypercube.GetCurrentUnixTime()) {
                SendClientChat(IncomingClient, "§EYou are muted.");
                return;
            }

            IncomingClient.ServerCore.Luahandler.RunFunction("E_ChatMessage", IncomingClient, Message);

            if (Message.StartsWith("/") && !Message.StartsWith("//"))
                IncomingClient.ServerCore.Commandholder.HandleCommand(IncomingClient, Message);
            else if (Message.StartsWith("@")) {
                string Client = Message.Substring(1, Message.IndexOf(" ") - 1);
                NetworkClient Tosend = null;

                foreach (NetworkClient c in IncomingClient.ServerCore.nh.Clients) {
                    if (c.CS.LoginName.ToLower() == Client.ToLower()) {
                        Tosend = c;
                        break;
                    }
                }

                if (Tosend == null) {
                    SendClientChat(IncomingClient, "§EPlayer '" + Client + "' not found.");
                    return;
                }

                SendClientChat(IncomingClient, "&c@" + Tosend.CS.FormattedName + "&f: " + Message.Substring(Message.IndexOf(" ") + 1, Message.Length - (Message.IndexOf(" ") + 1)));
                SendClientChat(Tosend, "&c@" + IncomingClient.CS.FormattedName + "&f: " + Message.Substring(Message.IndexOf(" ") + 1, Message.Length - (Message.IndexOf(" ") + 1)));
            } else if (Message.StartsWith("#")) {
                Message = Message.Substring(1, Message.Length - 1);

                if (IncomingClient.CS.Global) {
                    SendMapChat(IncomingClient.CS.CurrentMap, IncomingClient.ServerCore, IncomingClient.CS.FormattedName + "&f: " + Message);
                    IncomingClient.ServerCore.Logger.Log(IncomingClient.CS.CurrentMap.CWMap.MapName, IncomingClient.CS.LoginName + ": " + Message, LogType.Chat);
                } else {
                    SendGlobalChat(IncomingClient.ServerCore, "&c#&f " + IncomingClient.CS.FormattedName + "&f: " + Message);
                    IncomingClient.ServerCore.Logger.Log("Global", IncomingClient.CS.LoginName + ": " + Message, LogType.Chat);
                }
            } else {
                if (IncomingClient.CS.Global) {
                    SendGlobalChat(IncomingClient.ServerCore, "&c#&f " + IncomingClient.CS.FormattedName + "&f: " + Message);
                    IncomingClient.ServerCore.Logger.Log("Global", IncomingClient.CS.LoginName + ": " + Message, LogType.Chat);
                } else {
                    SendMapChat(IncomingClient.CS.CurrentMap, IncomingClient.ServerCore, IncomingClient.CS.FormattedName + "&f: " + Message);
                    IncomingClient.ServerCore.Logger.Log(IncomingClient.CS.CurrentMap.CWMap.MapName, IncomingClient.CS.LoginName + ": " + Message, LogType.Chat);
                }
            }
        }

        public static List<string> SplitBrs(string Input) {
            var Builder = new List<string>();

            while (Input.IndexOf("<br>", StringComparison.OrdinalIgnoreCase) >= 0) {
                int index = Input.IndexOf("<br>", StringComparison.OrdinalIgnoreCase);
                Builder.Add(Input.Substring(0, index)); // -- Add to our string builder
                Input = Input.Substring(index + 4, Input.Length - (index + 4)); // -- Remove from Input the string, and discard the <br>.
            }

            // -- If there's any leftovers that wern't split, we will need to go ahead and add that as well.
            if (Input != "")
                Builder.Add(Input);

            // -- If we miracously made it here without having to break the line, we will need to do this.
            if (Builder.Count == 0)
                Builder.Add(Input);

            return Builder;
        }

        /// <summary>
        /// Replaces in-game text codes with emotes (unicode).
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static string EmoteReplace(string Message) {
            Message = Message.Replace("{:)}", "\u0001"); // ☺
            Message = Message.Replace("{smile}", "\u0001");

            Message = Message.Replace("{smile2}", "\u0002"); // ☻

            Message = Message.Replace("{heart}", "\u0003"); // ♥
            Message = Message.Replace("{hearts}", "\u0003");
            Message = Message.Replace("{<3}", "\u0003");

            Message = Message.Replace("{diamond}", "\u0004"); // ♦
            Message = Message.Replace("{diamonds}", "\u0004");
            Message = Message.Replace("{rhombus}", "\u0004");

            Message = Message.Replace("{club}", "\u0005"); // ♣
            Message = Message.Replace("{clubs}", "\u0005");
            Message = Message.Replace("{clover}", "\u0005");
            Message = Message.Replace("{shamrock}", "\u0005");

            Message = Message.Replace("{spade}", "\u0006"); // ♠
            Message = Message.Replace("{spades}", "\u0006");

            Message = Message.Replace("{*}", "\u0007"); // •
            Message = Message.Replace("{bullet}", "\u0007");
            Message = Message.Replace("{dot}", "\u0007");
            Message = Message.Replace("{point}", "\u0007");

            Message = Message.Replace("{hole}", "\u0008"); // ◘

            Message = Message.Replace("{circle}", "\u0009"); // ○
            Message = Message.Replace("{o}", "\u0009");

            Message = Message.Replace("{male}", "\u000B"); // ♂
            Message = Message.Replace("{mars}", "\u000B");

            Message = Message.Replace("{female}", "\u000C"); // ♀
            Message = Message.Replace("{venus}", "\u000C");

            Message = Message.Replace("{8}", "\u000D"); // ♪
            Message = Message.Replace("{note}", "\u000D");
            Message = Message.Replace("{quaver}", "\u000D");

            Message = Message.Replace("{notes}", "\u000E"); // ♫
            Message = Message.Replace("{music}", "\u000E");

            Message = Message.Replace("{sun}", "\u000F"); // ☼
            Message = Message.Replace("{celestia}", "\u000F");

            Message = Message.Replace("{>>}", "\u0010"); // ►
            Message = Message.Replace("{right2}", "\u0010");

            Message = Message.Replace("{<<}", "\u0011"); // ◄
            Message = Message.Replace("{left2}", "\u0011");

            Message = Message.Replace("{updown}", "\u0012"); // ↕
            Message = Message.Replace("{^v}", "\u0012");

            Message = Message.Replace("{!!}", "\u0013"); // ‼

            Message = Message.Replace("{p}", "\u0014"); // ¶
            Message = Message.Replace("{para}", "\u0014");
            Message = Message.Replace("{pilcrow}", "\u0014");
            Message = Message.Replace("{paragraph}", "\u0014");

            Message = Message.Replace("{s}", "\u0015"); // §
            Message = Message.Replace("{sect}", "\u0015");
            Message = Message.Replace("{section}", "\u0015");

            Message = Message.Replace("{-}", "\u0016"); // ▬
            Message = Message.Replace("{_}", "\u0016");
            Message = Message.Replace("{bar}", "\u0016");
            Message = Message.Replace("{half}", "\u0016");

            Message = Message.Replace("{updown2}", "\u0017"); // ↨
            Message = Message.Replace("{^v_}", "\u0017");

            Message = Message.Replace("{^}", "\u0018"); // ↑
            Message = Message.Replace("{up}", "\u0018");

            Message = Message.Replace("{v}", "\u0019"); // ↓
            Message = Message.Replace("{down}", "\u0019");

            Message = Message.Replace("{>}", "\u001A"); // →
            Message = Message.Replace("{->}", "\u001A");
            Message = Message.Replace("{right}", "\u001A");

            Message = Message.Replace("{<}", "\u001B"); // ←
            Message = Message.Replace("{<-}", "\u001B");
            Message = Message.Replace("{left}", "\u001B");

            Message = Message.Replace("{l}", "\u001C"); // ∟
            Message = Message.Replace("{angle}", "\u001C");
            Message = Message.Replace("{corner}", "\u001C");

            Message = Message.Replace("{<>}", "\u001D"); // ↔
            Message = Message.Replace("{<->}", "\u001D");
            Message = Message.Replace("{leftright}", "\u001D");

            Message = Message.Replace("{^^}", "\u001E"); // ▲
            Message = Message.Replace("{up2}", "\u001E");

            Message = Message.Replace("{vv}", "\u001F"); // ▼
            Message = Message.Replace("{down2}", "\u001F");

            Message = Message.Replace("{house}", "\u007F"); // ⌂
            
            Message = Message.Replace("{caret}", "^");
            Message = Message.Replace("{hat}", "^");

            Message = Message.Replace("{tilde}", "~");
            Message = Message.Replace("{wave}", "~");

            Message = Message.Replace("{grave}", "`");
            Message = Message.Replace("{\"}", "`");
            return Message;
        }
        /// <summary>
        /// Splits a long message into multiple lines as needed. Appends ">>" as needed. This will also pad messages if they are of incorrect length.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string[] SplitLines(string Input) {
            var Builder = new List<string>();

            if (Input.Length <= 64 && Input.IndexOf("<br>", StringComparison.OrdinalIgnoreCase) <= 0)
                return new string[] { Input.PadRight(64) };

            // -- The string is longer than 64 characters, or contains '<br>'.
            Builder.AddRange(SplitBrs(Input));
            string temp = "";

            // -- First, going to insert our own <br>'s wherever the string is too long.
            for (int i = 0; i < Builder.Count; i++) {
                temp = "";

                while (Builder[i].Length > 0) { // -- Going to use temp here so we don't mess up our original string
                    if (Builder[i].Length > 64) {
                        int thisIndex = Builder[i].Substring(0, 60).LastIndexOf(' '); // -- Split by words.

                        if (thisIndex == -1) // -- Just incase it's one spaceless string.
                            thisIndex = 60;

                        temp += Builder[i].Substring(0, thisIndex) + "&3>><br>"; // -- Put the string before, with the seperator, and our break.

                        // -- Finally, Remove this part of the string from the original Builder[i], and add our newline seperators.
                        Builder[i] = "&3>>&f" + Builder[i].Substring(thisIndex + 1, Builder[i].Length - (thisIndex + 1)); // -- It will now loop again for any subsequent breaks.
                    } else {
                        // -- Since Builder[i] is not (or is no longer) greater than 64 characters long, we can simply remove the whole thing :)
                        temp += Builder[i];
                        Builder[i] = "";
                    }
                }

                Builder[i] = temp;
            }

            // -- Next, remove any "<br>"'s, and split up the line on either side of it.
            for (int z = 0; z < Builder.Count; z++) {

                while (Builder[z].IndexOf("<br>", StringComparison.OrdinalIgnoreCase) >= 0) {
                    temp = Builder[z];
                    int index = Builder[z].IndexOf("<br>", StringComparison.OrdinalIgnoreCase);
                    Builder[z] = temp.Substring(0, index).PadRight(64);
                    Builder.Insert(z + 1, temp.Substring(index + 4, temp.Length - (index + 4)));
                }

                // -- If there's any leftovers that wern't split, we will need to go ahead and add that as well.
                if (Builder[z] != "")
                    Builder[z] = Builder[z].PadRight(64);
            }

            // -- If we miracously made it here without having to break the line, we will need to do this.
            if (Builder.Count == 0)
                Builder.Add(Input.PadRight(64));

            return Builder.ToArray(); // -- Return our nice array'd string :)
        }
    }
}
