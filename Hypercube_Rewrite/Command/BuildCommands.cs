using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hypercube.Client;
using Hypercube.Core;

namespace Hypercube.Command {
    internal static class BuildCommands {
        public static void Init(CommandHandler holder) {
            holder.AddCommand("/bind", cBind);
            holder.AddCommand("/cancel", cCancel);
            holder.AddCommand("/material", cMaterial);
            holder.AddCommand("/place", cPlace);
            holder.AddCommand("/redo", cRedo);
            holder.AddCommand("/undo", cUndo);
        }

        #region Bind
        static readonly Command cBind = new Command {
            Plugin = "",
            Group = "Build",
            Help = "§SChanges the block you have bound for using /material.<br>§SUsage: /bind [material] [build material]<br>§SEx. /bind Stone, or /bind Stone Fire",
            Console = false,
            AllPerms = false,
            
            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
            },

            Handler = BindHandler,
        };

        static void BindHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            switch (args.Length) {
                case 0:
                    Chat.SendClientChat(Client, "§SYour currently bound block is &f" + Client.CS.MyEntity.Boundblock.Name + "§S.");
                    Chat.SendClientChat(Client, "§SLooking for fCraft style bind? See /cmdhelp bind and /cmdhelp material.");
                    break;
                case 1:
                    // -- Change the Bound block only.
                    var newBlock = ServerCore.Blockholder.GetBlock(args[0]);

                    if (newBlock == null) {
                        Chat.SendClientChat(Client, "§ECouldn't find a block called '" + args[0] + "'.");
                        return;
                    }

                    Client.CS.MyEntity.Boundblock = newBlock;
                    ServerCore.DB.SetDatabase(Client.CS.LoginName, "PlayerDB", "BoundBlock", newBlock.Id);
                    Chat.SendClientChat(Client, "§SYour bound block is now " + newBlock.Name);
                    break;
                case 2:
                    var newBlocka = ServerCore.Blockholder.GetBlock(args[0]);

                    if (newBlocka == null) {
                        Chat.SendClientChat(Client, "§ECouldn't find a block called '" + args[0] + "'.");
                        return;
                    }

                    var materialBlock = ServerCore.Blockholder.GetBlock(args[1]);

                    if (materialBlock == null) {
                        Chat.SendClientChat(Client, "§ECouldn't find a block called '" + args[1] + "'.");
                        return;
                    }

                    Client.CS.MyEntity.Boundblock = newBlocka;
                    ServerCore.DB.SetDatabase(Client.CS.LoginName, "PlayerDB", "BoundBlock", newBlocka.Id);
                    Chat.SendClientChat(Client, "§SYour bound block is now " + newBlocka.Name);

                    Client.CS.MyEntity.BuildMaterial = materialBlock;
                    Chat.SendClientChat(Client, "§SYour build material is now " + materialBlock.Name);
                    break;
                default:
                    Chat.SendClientChat(Client, "§EWrong number of arguments supplied. See /cmdhelp bind");
                    break;
            }
        }
        #endregion
        #region Cancel
        static readonly Command cCancel = new Command {
            Plugin = "",
            Group = "Build",
            Help = "&eCancel any mode. You build normal after it.",
            Console = false,
            AllPerms = false,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
            },

            Handler = CancelHandler,
        };

        static void CancelHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            Client.CS.MyEntity.SetBuildmode("");
            Chat.SendClientChat(Client, "§SBuildmodes canceled.");
        }

        #endregion
        #region Material
        static readonly Command cMaterial = new Command {
            Plugin = "",
            Group = "Build",
            Help = "§SChanges your building material. Build it with your bound block.<br>§SYou get a list of materials with /materials<br>§SUsage: /material [material]",
            Console = false,
            AllPerms = false,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
            },

            Handler = MaterialHandler,
        };

        static void MaterialHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length == 0) {
                Chat.SendClientChat(Client, "§SYour build material has been reset.");
                Client.CS.MyEntity.BuildMaterial = ServerCore.Blockholder.GetBlock("");
                return;
            }

            var newBlock = ServerCore.Blockholder.GetBlock(args[0]);

            if (newBlock == null) {
                Chat.SendClientChat(Client, "§ECouldn't find a block called '" + args[0] + "'.");
                return;
            }

            Client.CS.MyEntity.BuildMaterial = newBlock;
            Chat.SendClientChat(Client, "§SYour build material is now " + newBlock.Name);
        }
        #endregion
        #region Place
        static readonly Command cPlace = new Command {
            Plugin = "",
            Group = "Build",
            Help = "§SPlaces a block under you. The material is your last built<br>§SUsage: /place <material>",
            Console = false,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
            },

            Handler = PlaceHandler,
        };

        static void PlaceHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length == 0) {
                Client.CS.CurrentMap.ClientChangeBlock(Client, (short)(Client.CS.MyEntity.X / 32), (short)(Client.CS.MyEntity.Y / 32), (short)((Client.CS.MyEntity.Z / 32) - 2), 1, Client.CS.MyEntity.Lastmaterial);
                Chat.SendClientChat(Client, "§SBlock placed.");
            } else if (args.Length == 1) {
                var newBlock = ServerCore.Blockholder.GetBlock(args[0]);

                if (newBlock == null) {
                    Chat.SendClientChat(Client, "§ECouldn't find a block called '" + args[0] + "'.");
                    return;
                }

                Client.CS.MyEntity.Lastmaterial = newBlock;
                Client.CS.CurrentMap.ClientChangeBlock(Client, (short)(Client.CS.MyEntity.X / 32), (short)(Client.CS.MyEntity.Y / 32), (short)((Client.CS.MyEntity.Z / 32) - 2), 1, Client.CS.MyEntity.Lastmaterial);
                Chat.SendClientChat(Client, "§SBlock placed.");
            }
        }
        #endregion
        #region Redo
        static readonly Command cRedo = new Command {
            Plugin = "",
            Group = "Build",
            Help = "§SRedoes changes you have undone with /undo.<br>§SUsage: /redo [steps <optional>]",
            Console = false,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
            },

            Handler = RedoHandler,
        };

        static void RedoHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length == 0) {
                Client.Redo(30000);
                return;
            }

            var myInt = 30000;
            int.TryParse(args[0], out myInt);

            Client.Redo(myInt);
        }
        #endregion
        #region Undo
        static readonly Command cUndo = new Command {
            Plugin = "",
            Group = "Build",
            Help = "§SUndoes changes you have made.<br>§SUsage: /undo [steps <optional>]",
            Console = false,
            AllPerms = true,

            UsePermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
            },

            ShowPermissions = new List<Permission> {
                new Permission {Fullname = "player.build", Group = "player", Perm = "build"},
                new Permission {Fullname = "player.delete", Group = "player", Perm = "delete"},
            },

            Handler = UndoHandler,
        };

        static void UndoHandler(NetworkClient Client, string[] args, string Text1, string Text2) {
            if (args.Length == 0) {
                Client.Undo(30000);
                return;
            }

            var myInt = 30000;
            int.TryParse(args[0], out myInt);

            Client.Undo(myInt);
        }
        #endregion
    }
}
