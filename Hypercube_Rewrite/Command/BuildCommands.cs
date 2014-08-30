﻿using System.Collections.Generic;

using Hypercube.Client;
using Hypercube.Core;

namespace Hypercube.Command {
    internal static class BuildCommands {
        public static void Init(CommandHandler holder) {
            holder.AddCommand("/bind", CBind);
            holder.AddCommand("/cancel", CCancel);
            holder.AddCommand("/material", CMaterial);
            holder.AddCommand("/place", CPlace);
            holder.AddCommand("/redo", CRedo);
            holder.AddCommand("/undo", CUndo);
        }

        #region Bind
        static readonly Command CBind = new Command {
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

        static void BindHandler(NetworkClient client, string[] args, string text1, string text2) {
            switch (args.Length) {
                case 0:
                    Chat.SendClientChat(client, "§SYour currently bound block is &f" + client.CS.MyEntity.Boundblock.Name + "§S.");
                    Chat.SendClientChat(client, "§SLooking for fCraft style bind? See /cmdhelp bind and /cmdhelp material.");
                    break;
                case 1:
                    // -- Change the Bound block only.
                    var newBlock = ServerCore.Blockholder.GetBlock(args[0]);

                    if (newBlock == null) {
                        Chat.SendClientChat(client, "§ECouldn't find a block called '" + args[0] + "'.");
                        return;
                    }

                    client.CS.MyEntity.Boundblock = newBlock;
                    ServerCore.DB.SetDatabase(client.CS.LoginName, "PlayerDB", "BoundBlock", newBlock.Id);
                    Chat.SendClientChat(client, "§SYour bound block is now " + newBlock.Name);
                    break;
                case 2:
                    var newBlocka = ServerCore.Blockholder.GetBlock(args[0]);

                    if (newBlocka == null) {
                        Chat.SendClientChat(client, "§ECouldn't find a block called '" + args[0] + "'.");
                        return;
                    }

                    var materialBlock = ServerCore.Blockholder.GetBlock(args[1]);

                    if (materialBlock == null) {
                        Chat.SendClientChat(client, "§ECouldn't find a block called '" + args[1] + "'.");
                        return;
                    }

                    client.CS.MyEntity.Boundblock = newBlocka;
                    ServerCore.DB.SetDatabase(client.CS.LoginName, "PlayerDB", "BoundBlock", newBlocka.Id);
                    Chat.SendClientChat(client, "§SYour bound block is now " + newBlocka.Name);

                    client.CS.MyEntity.BuildMaterial = materialBlock;
                    Chat.SendClientChat(client, "§SYour build material is now " + materialBlock.Name);
                    break;
                default:
                    Chat.SendClientChat(client, "§EWrong number of arguments supplied. See /cmdhelp bind");
                    break;
            }
        }
        #endregion
        #region Cancel
        static readonly Command CCancel = new Command {
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

        static void CancelHandler(NetworkClient client, string[] args, string text1, string text2) {
            client.CS.MyEntity.SetBuildmode("");
            Chat.SendClientChat(client, "§SBuildmodes canceled.");
        }

        #endregion
        #region Material
        static readonly Command CMaterial = new Command {
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

        static void MaterialHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length == 0) {
                Chat.SendClientChat(client, "§SYour build material has been reset.");
                client.CS.MyEntity.BuildMaterial = ServerCore.Blockholder.GetBlock("");
                return;
            }

            var newBlock = ServerCore.Blockholder.GetBlock(args[0]);

            if (newBlock == null) {
                Chat.SendClientChat(client, "§ECouldn't find a block called '" + args[0] + "'.");
                return;
            }

            client.CS.MyEntity.BuildMaterial = newBlock;
            Chat.SendClientChat(client, "§SYour build material is now " + newBlock.Name);
        }
        #endregion
        #region Place
        static readonly Command CPlace = new Command {
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

        static void PlaceHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length == 0) {
                client.CS.CurrentMap.ClientChangeBlock(client, (short)(client.CS.MyEntity.X / 32), (short)(client.CS.MyEntity.Y / 32), (short)((client.CS.MyEntity.Z / 32) - 2), 1, client.CS.MyEntity.Lastmaterial);
                Chat.SendClientChat(client, "§SBlock placed.");
            } else if (args.Length == 1) {
                var newBlock = ServerCore.Blockholder.GetBlock(args[0]);

                if (newBlock == null) {
                    Chat.SendClientChat(client, "§ECouldn't find a block called '" + args[0] + "'.");
                    return;
                }

                client.CS.MyEntity.Lastmaterial = newBlock;
                client.CS.CurrentMap.ClientChangeBlock(client, (short)(client.CS.MyEntity.X / 32), (short)(client.CS.MyEntity.Y / 32), (short)((client.CS.MyEntity.Z / 32) - 2), 1, client.CS.MyEntity.Lastmaterial);
                Chat.SendClientChat(client, "§SBlock placed.");
            }
        }
        #endregion
        #region Redo
        static readonly Command CRedo = new Command {
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

        static void RedoHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length == 0) {
                client.Redo(30000);
                return;
            }

            int myInt;
            int.TryParse(args[0], out myInt);

            client.Redo(myInt);
        }
        #endregion
        #region Undo
        static readonly Command CUndo = new Command {
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

        static void UndoHandler(NetworkClient client, string[] args, string text1, string text2) {
            if (args.Length == 0) {
                client.Undo(30000);
                return;
            }

            int myInt;
            int.TryParse(args[0], out myInt);

            client.Undo(myInt);
        }
        #endregion
    }
}
