using System;
using System.Collections.Generic;
using System.Linq;
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
        public SortedDictionary<string, Permission> UsePermissions; 
        public SortedDictionary<string, Permission> ShowPermissions;
        public CommandInvoker Handler;

        public bool CanBeCalled(Rank rank) {
            return rank.HasAllPermissions(UsePermissions);
        }

        public bool CanBeCalled(NetworkClient client) {
            return client.HasAllPermissions(UsePermissions);
        }

        public bool CanBeSeen(Rank rank) {
            return rank.HasAllPermissions(ShowPermissions);
        }

        public bool CanBeSeen(NetworkClient client) {
            return client.HasAllPermissions(ShowPermissions);
        }

        public void PrintHelp(NetworkClient client) {
            Chat.SendClientChat(client, Help);
        }

        public void Call(NetworkClient client, string[] args, string text1, string text2) {
            if (!CanBeCalled(client)) {
                Chat.SendClientChat(client, "§EYou do not have permission to use this command.");
                return;
            }

            if (!string.IsNullOrEmpty(Plugin))
                ServerCore.Luahandler.RunFunction(Plugin, client, args, text1, text2); // -- Run lua plugin for this command.
            else 
                Handler(client, args, text1, text2);
        }
    }

    public class CommandHandler {
        public Dictionary<string, Command> CommandDict;
        public Dictionary<string, List<string>> Groups;
        public Dictionary<string, List<string>> Aliases;
        public Settings AliasLoader;
        public Settings CommandSettings;

        public CommandHandler() {
            CommandDict = new Dictionary<string, Command>(StringComparer.InvariantCultureIgnoreCase);

            Populate();
            RegisterGroups();

            AliasLoader = new Settings("Aliases.txt", LoadAliases, "Settings/", false);
            ServerCore.Setting.RegisterFile(AliasLoader);
            AliasLoader.LoadFile();

            CommandSettings = new Settings("Commands.txt", LoadCommands);
            ServerCore.Setting.RegisterFile(CommandSettings);
            CommandSettings.LoadFile();

            FillFile();
        }

        /// <summary>
        /// Populates the server with the default command sets.
        /// </summary>
        public void Populate() {
            GeneralCommands.Init(this);
            BuildCommands.Init(this);
            MapCommands.Init(this);
            OpCommands.Init(this);
        }

        #region Command management
        /// <summary>
        /// Function for internal command registration.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="newCommand"></param>
        public void AddCommand(string name, Command newCommand) {
            Command myCmd;

            if (CommandDict.TryGetValue(name, out myCmd))
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
        /// <param name="console"></param>
        public void RegisterCommand(string command, string plugin, string group, string help, string showPermissions, string usePermissions, bool allPerms, bool console) {
            var newCommand = new Command {
                Plugin = plugin,
                Group = group,
                Help = help,
                AllPerms = allPerms,
                Console = console,

                UsePermissions = PermissionContainer.SplitPermissions(usePermissions),
                ShowPermissions = PermissionContainer.SplitPermissions(showPermissions),

            };

            if (!command.StartsWith("/"))
                command = "/" + command;

            AddCommand(command, newCommand);
            UpdateCommand(command, newCommand);
            RegisterGroups();
        }

        /// <summary>
        /// Loads a list of all command groups
        /// </summary>
        void RegisterGroups() {
            if (Groups != null)
                Groups.Clear();
            else
                Groups = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var command in CommandDict.Keys) {
                if (Groups.ContainsKey(CommandDict[command].Group)) 
                    Groups[CommandDict[command].Group].Add(command.Replace("/", ""));
                 else 
                    Groups.Add(CommandDict[command].Group, new List<string> { command.Replace("/", "") });
            }
        }

        public void HandleConsoleCommand(string message) {
            
        }

        /// <summary>
        /// Handles a command incoming from a client, and executes it.
        /// </summary>
        /// <param name="client">The client sending this message</param>
        /// <param name="message">The text the client sent</param>
        public void HandleCommand(NetworkClient client, string message) {
            if (!message.Contains(" "))
                message += " ";

            // -- Split the message into its subsections.
            // -- The command (ex. /help)
            var command = message.Substring(0, message.IndexOf(" "));
            // -- An array of arguments provided to the command.
            var splits = message.Substring(message.IndexOf(" ") + 1, message.Length - (message.IndexOf(" ") + 1)).Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            // -- All text after the actual command
            var text = message.Substring(message.IndexOf(" ") + 1, message.Length - (message.IndexOf(" ") + 1));
            // -- All text starting after the first argument of a command.
            var text2 = text.Substring(text.IndexOf(" ") + 1, text.Length - (text.IndexOf(" ") + 1));

            // -- Log the command usage
            if (!ServerCore.LogArguments)
                ServerCore.Logger.Log("Commands", "Player '" + client.CS.LoginName + "' used command " + command, LogType.Command);
            else
                ServerCore.Logger.Log("Commands", "Player '" + client.CS.LoginName + "' used command " + command + " { \"" + string.Join("\", \"", splits) + "\" }", LogType.Command);

            var alias = GetAlias(command); // -- Determine if this is an alias of a command.

            if (!CommandDict.ContainsKey(command) && alias == "false") // -- Command is not an alias, and is not in our list (It doesn't exist)
                Chat.SendClientChat(client, "§ECommand '" + command + "' not found.");
            else {
                var thisCommand = alias == "false" ? CommandDict[command.ToLower()] : CommandDict[alias.ToLower()];

                if (!thisCommand.CanBeSeen(client)) { // -- If it cannot be seen by this user, then it doesn't exist.
                    Chat.SendClientChat(client, "§ECommand '" + command + "' not found.");
                    return;
                }

                thisCommand.Call(client, splits, text, text2); // -- All is good! run the command.
            }
        }
        #endregion
        #region Aliases
        /// <summary>
        /// Loads command aliases from file.
        /// </summary>
        public void LoadAliases() {
            // -- File format is actual = alias
            // -- You may use /actual = /alias as well
            if (Aliases == null)
                Aliases = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);
            else
                Aliases.Clear();

            foreach (var c in CommandDict.Keys) // -- Create an entry for every command, and a list for its aliases.
                Aliases.Add(c, new List<string>());

            using (var sr = new StreamReader("Settings/Aliases.txt")) {
                while (!sr.EndOfStream) {
                    var myline = sr.ReadLine();

                    if (myline != null && myline.StartsWith(";")) // -- Comment
                        continue;

                    if (myline != null && !myline.Contains("=")) // -- Incorrect formatting
                        continue;

                    // -- Command = Alias
                    if (myline == null) 
                        continue;

                    var command = myline.Substring(0, myline.IndexOf("=")).Replace(" ", "").ToLower();
                    var alias = myline.Substring(myline.IndexOf("=") + 1, myline.Length - (myline.IndexOf("=") + 1)).Replace(" ", "").ToLower();

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

        /// <summary>
        /// Returns the actual command for the given alias
        /// </summary>
        /// <param name="command">alias command</param>
        /// <returns>false if no alias found, else the actual command will be returned</returns>
        public string GetAlias(string command) {
            foreach (var s in Aliases.Keys.Where(s => Aliases[s].Contains(command.ToLower()))) {
                return s;
            }

            return "false";
        }
        #endregion
        #region Command File
        /// <summary>
        /// Loads scripted commands, and command modifications from file.
        /// </summary>
        public void LoadCommands() {
            foreach (var id in CommandSettings.SettingsDictionary.Keys) {
                CommandSettings.SelectGroup(id);
                Command myCmd;

                if (!CommandDict.TryGetValue(id, out myCmd)) {
                    myCmd = new Command {
                        Plugin = CommandSettings.Read("Plugin", ""),
                        Group = CommandSettings.Read("Group", "General"),
                        Help = CommandSettings.Read("Help", ""),
                        Console = bool.Parse(CommandSettings.Read("Console", "false")),
                        AllPerms = bool.Parse(CommandSettings.Read("AllPerms", "false")),

                        UsePermissions = PermissionContainer.SplitPermissions(CommandSettings.Read("UsePerms",
                            "player.chat")),

                        ShowPermissions = PermissionContainer.SplitPermissions(CommandSettings.Read("ShowPerms",
                            "player.chat")),
                    };

                    AddCommand(id, myCmd);
                    continue;
                }

                myCmd.Plugin = CommandSettings.Read("Plugin", "");
                myCmd.Group = CommandSettings.Read("Group", "General");
                myCmd.Help = CommandSettings.Read("Help", "");
                myCmd.Console = bool.Parse(CommandSettings.Read("Console", "false"));
                myCmd.AllPerms = bool.Parse(CommandSettings.Read("AllPerms", "false"));
                myCmd.UsePermissions =
                    PermissionContainer.SplitPermissions(CommandSettings.Read("UsePerms",
                        "player.chat"));
                myCmd.ShowPermissions =
                    PermissionContainer.SplitPermissions(CommandSettings.Read("ShowPerms",
                        "player.chat"));

                CommandDict[id] = myCmd;
            }

            ServerCore.Logger.Log("Commands", "Commands Loaded", LogType.Info);
        }

        /// <summary>
        /// Checks for missing built in commands, and adds them to the commands file.
        /// </summary>
        public void FillFile() {
            foreach (var command in CommandDict) { // Iterate the list
                Dictionary<string, string> temp;

                if (!CommandSettings.SettingsDictionary.TryGetValue(command.Key, out temp)) // -- If we don't have the value in the file
                    UpdateCommand(command.Key, command.Value); // -- Add it to the file.
            }
        }

        /// <summary>
        /// Adds / Updates a command to the command phone
        /// </summary>
        /// <param name="cmdTrig">The 'command' used to trigger this command. ex. /rules</param>
        /// <param name="cmd">The command class for this command.</param>
        public void UpdateCommand(string cmdTrig, Command cmd) {
            CommandSettings.SelectGroup(cmdTrig);

            CommandSettings.Write("Plugin", cmd.Plugin);
            CommandSettings.Write("Group", cmd.Group);
            CommandSettings.Write("Help", cmd.Help);
            CommandSettings.Write("Console", cmd.Console.ToString());
            CommandSettings.Write("AllPerms", cmd.AllPerms.ToString());
            CommandSettings.Write("UsePerms",
                PermissionContainer.PermissionsToString(cmd.UsePermissions));
            CommandSettings.Write("ShowPerms",
            PermissionContainer.PermissionsToString(cmd.ShowPermissions));

            CommandSettings.SaveFile();
        }
        #endregion
    }
}
