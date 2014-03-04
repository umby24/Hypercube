using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hypercube_Classic.Map;
using Hypercube_Classic.Client;
using Hypercube_Classic.Libraries;
using Hypercube_Classic.Packets;

namespace Hypercube_Classic.Core {
    public class Chat {
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

            for (int i = 0; i < Core.nh.Clients.Count; i++) {
                foreach (string b in Sending) {
                    Chat.Text = b;
                    Chat.Write(Core.nh.Clients[i]);
                }
            }
                //foreach (NetworkClient c in Core.nh.Clients) {
                //    foreach (string b in Sending) {
                //        Chat.Text = b;
                //        Chat.Write(c);
                //    }
                //}

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

            foreach (NetworkClient c in Map.Clients) {
                foreach (string b in Sending) {
                    Chat.Text = b;
                    Chat.Write(c);
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
            Message = Message.Replace("^detail.user=", ""); // -- Filter out WoM Messages from clients.

            Message = Text.CleanseString(Message);
            //TODO: Emote_Replace

            return Message;
        }

        public static void HandleIncomingChat(NetworkClient IncomingClient, string Message) {
            Message = FilterIncomingChat(Message);

            if (IncomingClient.CS.MuteTime > Hypercube.GetCurrentUnixTime()) {
                SendClientChat(IncomingClient, "&4Error:&f You are muted.");
                return;
            }

            if (Message.StartsWith("/")) 
                IncomingClient.ServerCore.Commandholder.HandleCommand(IncomingClient.ServerCore, IncomingClient, Message);
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
                    SendClientChat(IncomingClient, "&4Error: &fPlayer '" + Client + "' not found.");
                    return;
                }

                SendClientChat(IncomingClient, "&c@" + Tosend.CS.FormattedName + "&f: " + Message.Substring(Message.IndexOf(" ") + 1, Message.Length - (Message.IndexOf(" ") + 1)));
                SendClientChat(Tosend, "&c@" + IncomingClient.CS.FormattedName + "&f: " + Message.Substring(Message.IndexOf(" ") + 1, Message.Length - (Message.IndexOf(" ") + 1)));
            } else if (Message.StartsWith("#")) {
                Message = Message.Substring(1, Message.Length - 1);

                if (IncomingClient.CS.Global) {
                    SendMapChat(IncomingClient.CS.CurrentMap, IncomingClient.ServerCore, IncomingClient.CS.FormattedName + "&f: " + Message);
                    IncomingClient.ServerCore.Logger._Log("Chat", IncomingClient.CS.CurrentMap.Map.MapName, IncomingClient.CS.LoginName + ": " + Message);
                } else {
                    SendGlobalChat(IncomingClient.ServerCore, "&c#&f " + IncomingClient.CS.FormattedName + "&f: " + Message);
                    IncomingClient.ServerCore.Logger._Log("Chat", "Global", IncomingClient.CS.LoginName + ": " + Message);
                }
            } else {
                if (IncomingClient.CS.Global) {
                    SendGlobalChat(IncomingClient.ServerCore, "&c#&f " + IncomingClient.CS.FormattedName + "&f: " + Message);
                    IncomingClient.ServerCore.Logger._Log("Chat", "Global", IncomingClient.CS.LoginName + ": " + Message);
                } else {
                    SendMapChat(IncomingClient.CS.CurrentMap, IncomingClient.ServerCore, IncomingClient.CS.FormattedName + "&f: " + Message);
                    IncomingClient.ServerCore.Logger._Log("Chat", IncomingClient.CS.CurrentMap.Map.MapName, IncomingClient.CS.LoginName + ": " + Message);
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
                        int thisIndex = Builder[i].Substring(0, 64).LastIndexOf(' '); // -- Split by words.

                        if (thisIndex == -1 || thisIndex > 59) // -- Just incase it's one spaceless string.
                            thisIndex = 59;

                        temp += Builder[i].Substring(0, thisIndex) + "&3>><br>"; // -- Put the string before, with the seperator, and our break.

                        // -- Finally, Remove this part of the string from the original Builder[i], and add our newline seperators.
                        Builder[i] = "&3>>&f" + Builder[i].Substring(thisIndex, Builder[i].Length - (thisIndex)); // -- It will now loop again for any subsequent breaks.
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
                    //Builder.Add(Builder[z].Substring(0, index).PadRight(64)); // -- Add to our string builder
                    //Builder[z] = Builder[z].Substring(index + 4, Builder[z].Length - (index + 4)); // -- Remove from Builder[z] the string, and discard the <br>.
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
