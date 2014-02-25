using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public Commands() {
            CommandDict = new Dictionary<string, Command>(StringComparer.InvariantCultureIgnoreCase);

            CommandDict.Add("/ban", new BanCommand());
            CommandDict.Add("/commands", new CommandsList());
            CommandDict.Add("/cmdhelp", new CommandHelp());
            CommandDict.Add("/kick", new KickCommand());
            CommandDict.Add("/map", new MapCommand());
            CommandDict.Add("/maps", new MapsCommand());
            CommandDict.Add("/mute", new MuteCommand());
            CommandDict.Add("/pinfo", new PinfoCommand());
            CommandDict.Add("/players", new PlayersCommand());
            CommandDict.Add("/ranks", new RanksCommand());
            CommandDict.Add("/setrank", new SetrankCommand());
            CommandDict.Add("/stop", new StopCommand());
            CommandDict.Add("/unban", new UnbanCommand());
            CommandDict.Add("/unmute", new UnmuteCommand());
            CommandDict.Add("/unstop", new UnstopCommand());

            CommandDict.Add("/temp", new TempCommand());
            RegisterGroups();
        }

        public void HandleCommand(Hypercube Core, NetworkClient Client, string Message) {
            if (!Message.Contains(" "))
                Message += " ";

            var command = Message.Substring(0, Message.IndexOf(" "));
            var splits = Message.Substring(Message.IndexOf(" ") + 1, Message.Length - (Message.IndexOf(" ") + 1)).Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            var text = Message.Substring(Message.IndexOf(" ") + 1, Message.Length - (Message.IndexOf(" ") + 1));
            var text2 = text.Substring(text.IndexOf(" ") + 1, text.Length - (text.IndexOf(" ") + 1));

            Core.Logger._Log("Info", "Commands", "Player '" + Client.CS.LoginName + "' used command " + command);

            if (!CommandDict.ContainsKey(command.ToLower())) 
                Chat.SendClientChat(Client, "&4Error:&f Command '" + command + "' not found.");
            else {
                var thisCommand = CommandDict[command.ToLower()];
                var Ranks = RankContainer.SplitRanks(Core, thisCommand.UseRanks);

                if (Ranks.Contains(Client.CS.PlayerRank))
                    thisCommand.Run(command, splits, text, text2, Core, Client);
                else
                    Chat.SendClientChat(Client, "&4Error:&f You do not have permission to use this command.");
            }
        }

        public void AddCommand(string Command, string Plugin, string Group, string Help, string ShowRanks, string UseRanks) {
            var NewCommand = new ScriptedCommand(Command, Plugin, Group, Help, ShowRanks, UseRanks);
            CommandDict.Add(Command.ToLower(), NewCommand);
            RegisterGroups();
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

                foreach (string a in Core.Commandholder.Groups.Keys) 
                    Chat.SendClientChat(Client, "&e/commands " + a);
                
            } else if (args.Length == 1) { // -- list a group.
                if (!Core.Commandholder.Groups.ContainsKey(args[0])) {
                    Chat.SendClientChat(Client, "&4Error: &fGroup '" + args[0] + "' not found.");
                    return;
                }

                string commandString = "&3| &f";

                foreach (string b in Core.Commandholder.Groups[args[0]]) {
                    if (b != Core.Commandholder.Groups[args[0]].Last())
                        commandString += b + " | ";
                    else
                        commandString += b + " &3|";
                }

                Chat.SendClientChat(Client, "Group " + args[0]);
                Chat.SendClientChat(Client, commandString);
            } else {
                Chat.SendClientChat(Client, "&4Error: &fWrong number of arguments supplied.");
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
            if (Core.Commandholder.CommandDict.ContainsKey("/" + args[0].ToLower()) == false) {
                Chat.SendClientChat(Client, "&4Error: &fCommand not found.");
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
