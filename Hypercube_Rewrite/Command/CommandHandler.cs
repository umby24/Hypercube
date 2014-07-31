using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Hypercube.Client;
using Hypercube.Libraries;
using Hypercube.Core;

namespace Hypercube.Command {
    public class Command {
        public string Plugin;
        public string Group;
        public string Help;
        public bool Console;
        public bool AllPerms; // -- if true, a user must have every permission in the list to use the command. false, they only need one.
        public List<Permission> UsePermissions;
        public List<Permission> ShowPermissions;
        public CommandInvoker Handler;

        public bool CanBeCalled(Rank rank) {
            return PermissionContainer.RankMatchesPermissions(rank, UsePermissions, AllPerms);
        }

        public bool CanBeCalled(NetworkClient client) {
            bool result = false;

            foreach (Rank r in client.CS.PlayerRanks) {
                if (PermissionContainer.RankMatchesPermissions(r, UsePermissions, AllPerms)) {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public bool CanBeSeen(Rank rank) {
            return PermissionContainer.RankMatchesPermissions(rank, ShowPermissions, AllPerms);
        }

        public bool CanBeSeen(NetworkClient client) {
            bool result = false;

            foreach (Rank r in client.CS.PlayerRanks) {
                if (PermissionContainer.RankMatchesPermissions(r, ShowPermissions, AllPerms)) {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public void PrintHelp(NetworkClient client) {
            Chat.SendClientChat(client, Help);
        }

        public void Call(NetworkClient client, string[] args, string Text1, string Text2) {
            if (Plugin != null && Plugin != "")
                client.ServerCore.Luahandler.RunFunction(Plugin, client, args, Text1, Text2); // -- Run lua plugin for this command.
            else 
                Handler(client, args, Text1, Text2);
        }
    }

    public class CommandHandler {
        public Dictionary<string, Command> CommandDict;
        public Dictionary<string, List<string>> Groups;
        public Dictionary<string, List<string>> Aliases;
        public ISettings AliasLoader;
        public Hypercube ServerCore;

        public CommandHandler(Hypercube Core) {
            ServerCore = Core;
            CommandDict = new Dictionary<string, Command>(StringComparer.InvariantCultureIgnoreCase);

            Populate();
            RegisterGroups();

            AliasLoader = Core.Settings.RegisterFile("Aliases.txt", false, new PBSettingsLoader.LoadSettings(LoadAliases));
            ServerCore.Settings.ReadSettings(AliasLoader);
        }

        public void Populate() {
            GeneralCommands.Init(this);
            BuildCommands.Init(this);
            MapCommands.Init(this);
            OpCommands.Init(this);
        }

        /// <summary>
        /// Function for internal command registration.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="newCommand"></param>
        public void AddCommand(string name, Command newCommand) {
            if (CommandDict.ContainsKey(name))
                CommandDict[name] = newCommand;
            else
                CommandDict.Add(name, newCommand);
        }

        /// <summary>
        /// Function for command registration by lua scripts.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="plugin"></param>
        /// <param name="group"></param>
        /// <param name="help"></param>
        /// <param name="showPermissions"></param>
        /// <param name="usePermissions"></param>
        /// <param name="allPerms"></param>
        /// <param name="handler"></param>
        public void RegisterCommand(string command, string plugin, string group, string help, string showPermissions, string usePermissions, bool allPerms, bool console) {
            var newCommand = new Command {
                Plugin = plugin,
                Group = group,
                Help = help,
                AllPerms = allPerms,
                Console = console,

                UsePermissions = PermissionContainer.SplitPermissions(ServerCore, usePermissions).Values.ToList(),
                ShowPermissions = PermissionContainer.SplitPermissions(ServerCore, showPermissions).Values.ToList()

            };

            if (!command.StartsWith("/"))
                command = "/" + command;

            AddCommand(command, newCommand);
            RegisterGroups();
        }

        void RegisterGroups() {
            if (Groups != null)
                Groups.Clear();
            else
                Groups = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);

            foreach (string command in CommandDict.Keys) {
                if (Groups.ContainsKey(CommandDict[command].Group)) 
                    Groups[CommandDict[command].Group].Add(command.Replace("/", ""));
                 else 
                    Groups.Add(CommandDict[command].Group, new List<string>() { command.Replace("/", "") });
            }
        }

        public void HandleCommand(NetworkClient Client, string Message) {
            if (!Message.Contains(" "))
                Message += " ";

            // -- Split the message into its subsections.
            // -- The command (ex. /help)
            var command = Message.Substring(0, Message.IndexOf(" "));
            // -- An array of arguments provided to the command.
            var splits = Message.Substring(Message.IndexOf(" ") + 1, Message.Length - (Message.IndexOf(" ") + 1)).Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            // -- All text after the actual command
            var text = Message.Substring(Message.IndexOf(" ") + 1, Message.Length - (Message.IndexOf(" ") + 1));
            // -- All text starting after the first argument of a command.
            var text2 = text.Substring(text.IndexOf(" ") + 1, text.Length - (text.IndexOf(" ") + 1));

            // -- Log the command usage
            if (!Client.ServerCore.LogArguments)
                Client.ServerCore.Logger.Log("Commands", "Player '" + Client.CS.LoginName + "' used command " + command, LogType.Command);
            else
                Client.ServerCore.Logger.Log("Commands", "Player '" + Client.CS.LoginName + "' used command " + command + " { \"" + string.Join("\", \"", splits) + "\" }", LogType.Command);

            var alias = GetAlias(command); // -- Determine if this is an alias of a command.

            if (!CommandDict.ContainsKey(command) && alias == "false") // -- Command is not an alias, and is not in our list (It doesn't exist)
                Chat.SendClientChat(Client, Client.ServerCore.TextFormats.ErrorMessage + "Command '" + command + "' not found.");
            else {
                Command thisCommand = null;

                if (alias == "false") // -- If an alias, get the proper command for it.
                    thisCommand = CommandDict[command.ToLower()];
                else
                    thisCommand = CommandDict[alias.ToLower()];

                if (!thisCommand.CanBeSeen(Client)) { // -- If it cannot be seen by this user, then it doesn't exist.
                    Chat.SendClientChat(Client, Client.ServerCore.TextFormats.ErrorMessage + "Command '" + command + "' not found.");
                    return;
                }

                if (!thisCommand.CanBeCalled(Client)) { // -- If it cannot be called by this user, tell them.
                    Chat.SendClientChat(Client, Client.ServerCore.TextFormats.ErrorMessage + "You do not have permission to use this command.");
                    return;
                }

                thisCommand.Call(Client, splits, text, text2); // -- All is good! run the command.
            }
        }

        public void LoadAliases() {
            if (Aliases == null)
                Aliases = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);
            else
                Aliases.Clear();

            foreach (string c in CommandDict.Keys) // -- Create an entry for every command, and a list for its aliases.
                Aliases.Add(c, new List<string>());

            using (var SR = new StreamReader("Settings/Aliases.txt")) {
                while (!SR.EndOfStream) {
                    var Myline = SR.ReadLine();

                    if (Myline.StartsWith(";")) // -- Comment
                        continue;

                    if (!Myline.Contains("=")) // -- Incorrect formatting
                        continue;

                    // -- Command = Alias
                    string command = Myline.Substring(0, Myline.IndexOf("=")).Replace(" ", "").ToLower();
                    string alias = Myline.Substring(Myline.IndexOf("=") + 1, Myline.Length - (Myline.IndexOf("=") + 1)).Replace(" ", "").ToLower();

                    if (!command.StartsWith("/")) // -- Just a check incase the user didn't include a /.
                        command = "/" + command;

                    if (!alias.StartsWith("/"))
                        alias = "/" + alias;

                    if (!Aliases.ContainsKey(command)) // -- If the command doesn't exist.
                        continue;

                    if (Aliases[command].Contains(alias)) // -- if this alias already exists.
                        continue;

                    Aliases[command].Add(alias);
                }
            }

            ServerCore.Logger.Log("Commands", "Command aliases loaded.", LogType.Info);
        }

        public string GetAlias(string command) {
            foreach (string s in Aliases.Keys) {
                if (Aliases[s].Contains(command.ToLower()))
                    return s;
            }

            return "false";
        }
    }
}
