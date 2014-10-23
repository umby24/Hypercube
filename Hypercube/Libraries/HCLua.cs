using System;
using System.Collections.Generic;
using System.IO;
using Hypercube.Map;
using Hypercube.Network;
using NLua;
using NLua.Exceptions;
using Hypercube.Core;

namespace Hypercube.Libraries {
    public class HCLua {
        public Lua LuaHandler;

        Dictionary<string, DateTime> _scripts;

        public HCLua() {
            LuaHandler = new Lua();
        }

        public void LoadScripts() {
            _scripts = new Dictionary<string, DateTime>();

            if (Directory.Exists("Lua") == false)
                Directory.CreateDirectory("Lua");

            var files = Directory.GetFiles("Lua", "*.lua", SearchOption.AllDirectories);

            foreach (var file in files) {
                _scripts.Add(file, File.GetLastWriteTime(file));

                try {
                    LuaHandler.DoFile(file);
                } catch (LuaScriptException e) {
                    ServerCore.Logger.Log("Lua", "Lua Error: " + e.Message, LogType.Error);
                }
            }

            ServerCore.Logger.Log("Lua", "Lua scripts loaded.", LogType.Info);
        }

        /// <summary>
        /// Exposes server functions to Lua.
        /// </summary>
        public void RegisterFunctions() {
            var luaChat = new Chat();
            var luaCPE = new CPE();

            // -- Functions
            LuaHandler.RegisterFunction("Log", ServerCore.Logger, ServerCore.Logger.GetType().GetMethod("Log"));
            // -- Command creation functions
            LuaHandler.RegisterFunction("RegCmd", ServerCore.Commandholder, ServerCore.Commandholder.GetType().GetMethod("RegisterCommand"));
            LuaHandler.RegisterFunction("GetCmdAlias", ServerCore.Commandholder, ServerCore.Commandholder.GetType().GetMethod("GetAlias"));
            // -- DB functions
            LuaHandler.RegisterFunction("DBPlayerExists", ServerCore.DB, ServerCore.DB.GetType().GetMethod("ContainsPlayer"));
            LuaHandler.RegisterFunction("DBGetPlayerName", ServerCore.DB, ServerCore.DB.GetType().GetMethod("GetPlayerName"));
            LuaHandler.RegisterFunction("DBBanPlayer", ServerCore.DB, ServerCore.DB.GetType().GetMethod("BanPlayer"));
            LuaHandler.RegisterFunction("DBUnbanPlayer", ServerCore.DB, ServerCore.DB.GetType().GetMethod("UnbanPlayer"));
            LuaHandler.RegisterFunction("DBStopPlayer", ServerCore.DB, ServerCore.DB.GetType().GetMethod("StopPlayer"));
            LuaHandler.RegisterFunction("DBUnstopPlayer", ServerCore.DB, ServerCore.DB.GetType().GetMethod("UnstopPlayer"));
            LuaHandler.RegisterFunction("GetDBInt", ServerCore.DB, ServerCore.DB.GetType().GetMethod("GetDatabaseInt"));
            LuaHandler.RegisterFunction("GetDBString", ServerCore.DB, ServerCore.DB.GetType().GetMethod("GetDatabaseString"));
            LuaHandler.RegisterFunction("SetDBInt", ServerCore.DB, ServerCore.DB.GetType().GetMethod("SetDatabase", new[] {typeof(string), typeof(string), typeof(string), typeof(int)}));
            LuaHandler.RegisterFunction("SetDBStr", ServerCore.DB, ServerCore.DB.GetType().GetMethod("SetDatabase", new[] { typeof(string), typeof(string), typeof(string), typeof(string) }));
            LuaHandler.RegisterFunction("SetDBBool", ServerCore.DB, ServerCore.DB.GetType().GetMethod("SetDatabase", new[] { typeof(string), typeof(string), typeof(string), typeof(bool) }));
            LuaHandler.RegisterFunction("SendGlobalChat", luaChat, luaChat.GetType().GetMethod("SendGlobalChat"));
            LuaHandler.RegisterFunction("SendMapChat", luaChat, luaChat.GetType().GetMethod("SendMapChat"));
            LuaHandler.RegisterFunction("SendClientChat", luaChat, luaChat.GetType().GetMethod("SendClientChat"));
            LuaHandler.RegisterFunction("CreateFill", ServerCore.Fillholder, ServerCore.Fillholder.GetType().GetMethod("CreateFill"));
            LuaHandler.RegisterFunction("CreateSelection", luaCPE, luaCPE.GetType().GetMethod("CreateSelection"));
            LuaHandler.RegisterFunction("DeleteSelection", luaCPE, luaCPE.GetType().GetMethod("DeleteSelection"));
            LuaHandler.RegisterFunction("GetBlockI", ServerCore.Blockholder,
                ServerCore.Blockholder.GetType().GetMethod("GetBlock", new[] {typeof (int)}));
            LuaHandler.RegisterFunction("GetBlockS", ServerCore.Blockholder,
                ServerCore.Blockholder.GetType().GetMethod("GetBlock", new[] { typeof(string) }));
            LuaHandler.RegisterFunction("Setblock", ServerCore.Luahandler,
                ServerCore.Luahandler.GetType().GetMethod("Setblock"));
            LuaHandler.RegisterFunction("GetArr", luaCPE, luaCPE.GetType().GetMethod("GetAt"));
            // -- Variables

            LuaHandler["G_ServerName"] = ServerCore.ServerName;
            LuaHandler["G_MOTD"] = ServerCore.Motd;
            LuaHandler["G_Welcome"] = ServerCore.WelcomeMessage;
            LuaHandler["G_MainMap"] = ServerCore.MapMain;
            LuaHandler["LogType_Info"] = LogType.Info;
        }

        #region Functions
        // -- These functions will provide Lua scripts an interface with the server
        // -- Allowing them to have a great deal on control over the server itself.
        public void Setblock(short clientId, HypercubeMap map, short x, short y, short z, byte type, bool undo, bool physics, bool send, short priority) {
            map.BlockChange(clientId, x, y, z, ServerCore.Blockholder.GetBlock(type), map.GetBlock(x, y, z), undo, physics, send, priority);
            
        }
        #endregion
        public void RunFunction(string function, params object[] args) {
            var luaF = LuaHandler.GetFunction(function);

            try {
                if (luaF != null && args != null)
                    luaF.Call(args);
                else if (luaF != null)
                    luaF.Call();
            } catch (LuaScriptException e) {
                ServerCore.Logger.Log("Lua", "Lua Error: " + e.Message, LogType.Error);
                ServerCore.Logger.Log("Lua", e.StackTrace, LogType.Debug);
            }
        }

        public void Main() {
            var files = Directory.GetFiles("Lua", "*.lua", SearchOption.AllDirectories);

            foreach (var file in files) {
                if (!_scripts.ContainsKey(file)) { // -- New file, add it and load it.
                    _scripts.Add(file, File.GetLastWriteTime(file));

                    try {
                        LuaHandler.DoFile(file);
                    } catch (LuaScriptException e) {
                        ServerCore.Logger.Log("Lua", "Lua Error: " + e.Message, LogType.Error);
                    }

                    continue;
                }

                if (File.GetLastWriteTime(file) != _scripts[file]) {
                    try {
                        LuaHandler.DoFile(file);
                    } catch (LuaScriptException e) {
                        ServerCore.Logger.Log("Lua", "Lua Error: " + e.Message, LogType.Error);
                    }

                    _scripts[file] = File.GetLastWriteTime(file);
                }
            }
            
        }
    }
}
