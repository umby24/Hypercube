using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Hypercube_Classic.Libraries;
using Hypercube_Classic.Core;
using Hypercube_Classic.Client;

namespace Hypercube_Classic.Command {
    public interface Command {
        string Command { get;  }
        string Plugin { get;  }
        string Group { get;  }
        string Help { get; }
        string ShowRanks { get; }
        string UseRanks { get; }

        void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client);        
    }

   
   public class Commands {
        public Dictionary<string, Command> CommandDict;
        public Dictionary<string, List<string>> Groups;
        public Dictionary<string, List<string>> Aliases;
        public ISettings AliasLoader;
        public Hypercube ServerCore;

        public Commands(Hypercube Core) {
            CommandDict = new Dictionary<string, Command>(StringComparer.InvariantCultureIgnoreCase);

            CommandDict.Add("/addrank", new AddRankCommand());
            CommandDict.Add("/ban", new BanCommand());
            CommandDict.Add("/bind", new BindCommand());
            CommandDict.Add("/commands", new CommandsList());
            CommandDict.Add("/cmdhelp", new CommandHelp());
            CommandDict.Add("/delrank", new DelRankCommand());
            CommandDict.Add("/getrank", new GetRankCommand());
            CommandDict.Add("/global", new GlobalCommand());
            CommandDict.Add("/kick", new KickCommand());
            CommandDict.Add("/map", new MapCommand());
            CommandDict.Add("/mapadd", new MapAddCommand());
            CommandDict.Add("/maps", new MapsCommand());
            CommandDict.Add("/mapinfo", new MapInfoCommand());
            CommandDict.Add("/mapfill", new MapFillCommand());
            CommandDict.Add("/mapfills", new MapFillsCommand());
            CommandDict.Add("/mapload", new MapLoadCommand());
            CommandDict.Add("/mapresend", new MapResendCommand());
            CommandDict.Add("/mapresize", new MapResizeCommand());
            CommandDict.Add("/mapsave", new MapSaveCommand());
            CommandDict.Add("/material", new MaterialCommand());
            CommandDict.Add("/mute", new MuteCommand());
            CommandDict.Add("/pinfo", new PinfoCommand());
            CommandDict.Add("/place", new PlaceCommand());
            CommandDict.Add("/players", new PlayersCommand());
            CommandDict.Add("/pushrank", new PushRankCommand());
            CommandDict.Add("/ranks", new RanksCommand());
            CommandDict.Add("/redo", new RedoCommand());
            CommandDict.Add("/rules", new RulesCommand());
            CommandDict.Add("/setrank", new SetrankCommand());
            CommandDict.Add("/setspawn", new SetSpawnCommand());
            CommandDict.Add("/stop", new StopCommand());
            CommandDict.Add("/unban", new UnbanCommand());
            CommandDict.Add("/undo", new UndoCommand());
            CommandDict.Add("/unmute", new UnmuteCommand());
            CommandDict.Add("/unstop", new UnstopCommand());

            ServerCore = Core;

            AliasLoader = new ISettings();
            AliasLoader.Filename = "Aliases.txt";
            AliasLoader.Settings = new Dictionary<string, Dictionary<string, string>>();
            AliasLoader.LoadSettings = new Hypercube_Classic.Libraries.PBSettingsLoader.LoadSettings(LoadAliases);
            AliasLoader.Save = false;

            ServerCore.Settings.ReadSettings(AliasLoader);
            ServerCore.Settings.SettingsFiles.Add(AliasLoader);

            RegisterGroups();
        }

        public void HandleCommand(Hypercube Core, NetworkClient Client, string Message) {
            if (!Message.Contains(" "))
                Message += " ";

            var command = Message.Substring(0, Message.IndexOf(" "));
            var splits = Message.Substring(Message.IndexOf(" ") + 1, Message.Length - (Message.IndexOf(" ") + 1)).Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            var text = Message.Substring(Message.IndexOf(" ") + 1, Message.Length - (Message.IndexOf(" ") + 1));
            var text2 = text.Substring(text.IndexOf(" ") + 1, text.Length - (text.IndexOf(" ") + 1));

            if (!Core.LogArguments)
                Core.Logger._Log("Commands", "Player '" + Client.CS.LoginName + "' used command " + command, Libraries.LogType.Command);
            else
                Core.Logger._Log("Commands", "Player '" + Client.CS.LoginName + "' used command " + command + " { \"" + string.Join("\", \"",splits) + "\" }", Libraries.LogType.Command);

            string alias = GetAlias(command);

            if (!CommandDict.ContainsKey(command.ToLower()) && alias == "false") 
                Chat.SendClientChat(Client, "&4Error:&f Command '" + command + "' not found.");
            else {
                Command thisCommand = null;

                if (alias == "false")
                    thisCommand = CommandDict[command.ToLower()];
                else
                    thisCommand = CommandDict[alias.ToLower()];

                var Ranks = RankContainer.SplitRanks(Core, thisCommand.UseRanks);

                if (RankContainer.RankListContains(Ranks, Client.CS.PlayerRanks))
                    thisCommand.Run(command, splits, text, text2, Core, Client);
                else
                    Chat.SendClientChat(Client, "&4Error:&f You do not have permission to use this command.");
            }
        }

        public string GetAlias(string Command) {
            foreach (string s in Aliases.Keys) {
                if (Aliases[s].Contains(Command.ToLower()))
                    return s;
            }

            return "false";
        }

        public void AddCommand(string Command, string Plugin, string Group, string Help, string ShowRanks, string UseRanks) {
            var NewCommand = new ScriptedCommand(Command, Plugin, Group, Help, ShowRanks, UseRanks);
            CommandDict.Add(Command.ToLower(), NewCommand);
            RegisterGroups();
            LoadAliases();
        }

        void RegisterGroups() {
            if (Groups != null)
                Groups.Clear();
            else
                Groups = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);

            foreach (string command in CommandDict.Keys) {
                if (Groups.ContainsKey(CommandDict[command].Group)) {
                    Groups[CommandDict[command].Group].Add(command.Replace("/", ""));
                } else {
                    Groups.Add(CommandDict[command].Group, new List<string>() { command.Replace("/", "") });
                }
            }
        }

        void LoadAliases() {
            if (Aliases == null)
                Aliases = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);
            else
                Aliases.Clear();

            foreach (string c in CommandDict.Keys) // -- Create an entry for every command, and a list for its aliases.
                Aliases.Add(c, new List<string>());

            if (!File.Exists("Settings/Aliases.txt"))
                File.WriteAllText("Settings/Aliases.txt", "");

            using (var SR = new StreamReader("Settings/Aliases.txt")) {
                while (!SR.EndOfStream) {
                    var Myline = SR.ReadLine();

                    if (Myline.StartsWith(";")) // -- Comment
                        continue;

                    if (!Myline.Contains("=")) // -- Incorrect formatting
                        continue;

                    // -- Command = Alias
                    string command = Myline.Substring(0, Myline.IndexOf("=")).Replace(" ", "").ToLower();
                    string alias = Myline.Substring(Myline.IndexOf("=") + 1, Myline.Length - (Myline.IndexOf("=") + 1)).Replace(" ","").ToLower();

                    if (!command.StartsWith("/"))
                        command = "/" + command;

                    if (!alias.StartsWith("/"))
                        alias = "/" + alias;

                    if (!Aliases.ContainsKey(command))
                        continue;

                    if (Aliases[command].Contains(alias))
                        continue;

                    Aliases[command].Add(alias);
                }
            }

            ServerCore.Logger._Log("Commands", "Command aliases loaded.", Libraries.LogType.Info);
        }
    }

    struct CommandsList : Command {
        public string Command { get { return "/commands"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "General"; } }
        public string Help { get { return "Lists all commands"; } }

        public string ShowRanks { get { return "1,2"; } }
        public string UseRanks { get { return "1,2"; } }

        public void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            if (args.Length == 0) { // -- List command groups
                Chat.SendClientChat(Client, "&eCommand groups:");
                Chat.SendClientChat(Client, "&a    All");

                foreach (string a in Core.Commandholder.Groups.Keys) 
                    Chat.SendClientChat(Client, "&a    " + a);

            } else if (args.Length == 1) { // -- list a group.
                if (!Core.Commandholder.Groups.ContainsKey(args[0]) && args[0].ToLower() != "all") {
                    Chat.SendClientChat(Client, "§E&fGroup '" + args[0] + "' not found.");
                    return;
                }

                string commandString = "§D";

                if (args[0].ToLower() == "all") {
                    foreach (string b in Core.Commandholder.CommandDict.Keys) {
                        if (!RankContainer.RankListContains(RankContainer.SplitRanks(Core, Core.Commandholder.CommandDict[b].ShowRanks), Client.CS.PlayerRanks))
                            continue;

                        if (b != Core.Commandholder.CommandDict.Keys.Last())
                            commandString += b.Substring(1, b.Length - 1) + " §D";
                        else
                            commandString += b.Substring(1, b.Length - 1) + " §D";
                    }

                    Chat.SendClientChat(Client, "&aAll Commands:<br>" + commandString);
                    return;
                }                

                foreach (string b in Core.Commandholder.Groups[args[0]]) {
                    if (!RankContainer.RankListContains(RankContainer.SplitRanks(Core, Core.Commandholder.CommandDict["/" + b].ShowRanks), Client.CS.PlayerRanks))
                        continue;

                    if (b != Core.Commandholder.Groups[args[0]].Last())
                        commandString += b + " §D";
                    else
                        commandString += b + " §D";
                }

                Chat.SendClientChat(Client, "&aGroup " + args[0]);
                Chat.SendClientChat(Client, commandString);
            } else {
                Chat.SendClientChat(Client, "§E&fWrong number of arguments supplied.");
                return;
            }
        }
    }

    struct CommandHelp : Command {
        public string Command { get { return "/cmdhelp"; } }
        public string Plugin { get { return ""; } }
        public string Group { get { return "General"; } }
        public string Help { get { return "Shows help for a command.<br>Usage: /cmdhelp [command]"; } }

        public string ShowRanks { get { return "1,2"; } }
        public string UseRanks { get { return "1,2"; } }

        public void Run(string Command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            if (args.Length == 0) {
                Chat.SendClientChat(Client, "§E&fUsage of this command: /cmdhelp [command].");
                return;
            }
            if (Core.Commandholder.CommandDict.ContainsKey("/" + args[0].ToLower()) == false) {
                Chat.SendClientChat(Client, "§E&fCommand not found.");
                return;
            }

            var thisCommand = Core.Commandholder.CommandDict["/" + args[0].ToLower()];
            Chat.SendClientChat(Client, "&e/" + args[0]);
            Chat.SendClientChat(Client, thisCommand.Help);
        }
    }

    struct ScriptedCommand : Command {
        public ScriptedCommand(string Ccommand, string Pplugin, string Ggroup, string Hhelp, string SshowRanks, string UuseRanks) {
            _Command = Ccommand;
            _plugin = Pplugin;
            _group = Ggroup;
            _help = Hhelp;

            _Show = SshowRanks;
            _Use = UuseRanks;
        }

        private string _Command, _plugin, _group, _help, _Show, _Use;

        public string Command { get { return _Command; } }
        public string Plugin { get { return _plugin; } }
        public string Group { get { return _group; } }
        public string Help { get { return _help; } }

        public string ShowRanks { get { return _Show; } }
        public string UseRanks { get { return _Use; } }

        public void Run(string command, string[] args, string Text1, string Text2, Hypercube Core, NetworkClient Client) {
            if (Plugin.StartsWith("Lua:"))
                Core.LuaHandler.RunLuaFunction(Plugin.Substring(4, Plugin.Length - 4), new Object[] { command, args, Text1, Core, Client });
            else
                Activator.CreateInstance(Type.GetType(Plugin), command, args, Text1, Core, Client);
        }
    }
}
