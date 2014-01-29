using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hypercube_Classic.Map;
using Hypercube_Classic.Client;
using Hypercube_Classic.Libraries;
using Hypercube_Classic.Packets;

namespace Hypercube_Classic.Core {
    class Chat {
        /// <summary>
        /// Sends a chat message to all players across all maps.
        /// </summary>
        /// <param name="Core"></param>
        public static void SendGlobalChat(Hypercube Core, string Message, sbyte MessageType = 0, bool Log = false) {
            var Chat = new Message();
            Chat.PlayerID = MessageType;
            
            Message = Text.CleanseString(Message);

            if (Log)
                Core.Logger._Log("Chat", "Global", Message);

            //TODO: Emote Replace
            string[] Sending = SplitLines(Message);

            foreach (NetworkClient c in Core.nh.Clients) {
                foreach (string b in Sending) {
                    Chat.Text = b;
                    Chat.Write(c);
                }
            }

        }

        /// <summary>
        /// Sends a message to all clients on a certain map.
        /// </summary>
        public static void SendMapChat(HypercubeMap Map, Hypercube Core, string Message, sbyte MessageType = 0, bool Log = false) {
            var Chat = new Message();
            Chat.PlayerID = MessageType;

            Message = Text.CleanseString(Message);

            if (Log)
                Core.Logger._Log("Chat", "Global", Message);

            //TODO: Emote Replace
            string[] Sending = SplitLines(Message);

            foreach (NetworkClient c in Core.nh.Clients) {
                if (c.CS.CurrentMap.ToLower() == Map.Map.MapName.ToLower()) {
                    foreach (string b in Sending) {
                        Chat.Text = b;
                        Chat.Write(c);
                    }
                }
            }
        }

        /// <summary>
        /// Sends chat to an individual client
        /// </summary>
        public static void SendClientChat(NetworkClient Client, string Message, sbyte MessageType = 0) {
            Message = Text.CleanseString(Message);
            //TODO: Emote Replace
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

            Message = Text.CleanseString(Message);
            //TODO: Emote_Replace

            return Message;
        }

        public static void HandleIncomingChat(NetworkClient IncomingClient, string Message) {
            Message = FilterIncomingChat(Message);

            //TODO: If Client.GlobalChat..
            if (Message.StartsWith("/"))
                Message = "Potato";
            else if (Message.StartsWith("@"))
                Message = "Potato";
            else {
                SendGlobalChat(IncomingClient.ServerCore, IncomingClient.CS.FormattedName + "&f: " + Message);
                IncomingClient.ServerCore.Logger._Log("Chat", "Placeholder", IncomingClient.CS.LoginName + ": " + Message);
            }
        }

        /// <summary>
        /// Splits a long message into multiple lines as needed. Appends ">>" as needed. This will also pad messages if they are of incorrect length.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string[] SplitLines(string Input) {
            List<string> Builder = new List<string>();

            if (Input.Length <= 64 && Input.IndexOf("<br>", StringComparison.OrdinalIgnoreCase) >= 0) 
                return new string[] { Input.PadRight(64) };
            
            // -- The string is longer than 64 characters, or contains '<br>'.
            // -- First, going to insert our own <br>'s wherever the string is too long.
            string temp = "";

            while (Input.Length > 0) { // -- Going to use temp here so we don't mess up our original string
                if (Input.Length > 64) {
                    int thisIndex = Input.Substring(0, 64).LastIndexOf(' '); // -- Split by words.

                    if (thisIndex == null || thisIndex > 60) // -- Just incase it's one spaceless string.
                        thisIndex = 60;

                    temp += Input.Substring(0, thisIndex) + "&3>><br>"; // -- Put the string before, with the seperator, and our break.

                    // -- Finally, Remove this part of the string from the original Input, and add our newline seperators.
                    Input = "&3>>&f" + Input.Substring(thisIndex + 1, Input.Length - (thisIndex + 1)); // -- It will now loop again for any subsequent breaks.
                } else {
                    // -- Since input is not (or is no longer) greater than 64 characters long, we can simply remove the whole thing :)
                    temp += Input;
                    Input = "";
                }
            }

            Input = temp;

            // -- Next, remove any "<br>"'s, and split up the line on either side of it.

            while (Input.IndexOf("<br>", StringComparison.OrdinalIgnoreCase) >= 0) {
                int index = Input.IndexOf("<br>", StringComparison.OrdinalIgnoreCase);
                Builder.Add(Input.Substring(0, index + 1).PadRight(64)); // -- Add to our string builder
                Input = Input.Substring(index + 3, Input.Length - (index + 3)); // -- Remove from Input the string, and discard the <br>.
            }

            // -- If we miracously made it here without having to break the line, we will need to do this.
            if (Builder.Count == 0)
                Builder.Add(Input.PadRight(64));

            return Builder.ToArray(); // -- Return our nice array'd string :)
        }
    }
}
