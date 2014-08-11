using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using NLua;
using NLua.Exceptions;
using Hypercube.Core;

namespace Hypercube.Libraries {
    public class HCLua {
        public Lua LuaHandler;
        public Thread LuaThread;

        readonly Hypercube _servercore;
        Dictionary<string, DateTime> _scripts;

        public HCLua(Hypercube core) {
            _servercore = core;
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
                    _servercore.Logger.Log("Lua", "Lua Error: " + e.Message, LogType.Error);
                }
            }

            _servercore.Logger.Log("Lua", "Lua scripts loaded.", LogType.Info);
        }

        /// <summary>
        /// Exposes server functions to Lua.
        /// </summary>
        public void RegisterFunctions() {
            var luaChat = new Chat();

            // -- Functions
            LuaHandler.RegisterFunction("Log", _servercore.Logger, _servercore.Logger.GetType().GetMethod("Log"));
            // -- Command creation functions
            LuaHandler.RegisterFunction("RegCmd", _servercore.Commandholder, _servercore.Commandholder.GetType().GetMethod("RegisterCommand"));
            LuaHandler.RegisterFunction("GetCmdAlias", _servercore.Commandholder, _servercore.Commandholder.GetType().GetMethod("GetAlias"));
            // -- DB functions
            LuaHandler.RegisterFunction("DBPlayerExists", _servercore.DB, _servercore.DB.GetType().GetMethod("ContainsPlayer"));
            LuaHandler.RegisterFunction("DBGetPlayerName", _servercore.DB, _servercore.DB.GetType().GetMethod("GetPlayerName"));
            LuaHandler.RegisterFunction("DBBanPlayer", _servercore.DB, _servercore.DB.GetType().GetMethod("BanPlayer"));
            LuaHandler.RegisterFunction("DBUnbanPlayer", _servercore.DB, _servercore.DB.GetType().GetMethod("UnbanPlayer"));
            LuaHandler.RegisterFunction("DBStopPlayer", _servercore.DB, _servercore.DB.GetType().GetMethod("StopPlayer"));
            LuaHandler.RegisterFunction("DBUnstopPlayer", _servercore.DB, _servercore.DB.GetType().GetMethod("UnstopPlayer"));
            LuaHandler.RegisterFunction("GetDBInt", _servercore.DB, _servercore.DB.GetType().GetMethod("GetDatabaseInt"));
            LuaHandler.RegisterFunction("GetDBString", _servercore.DB, _servercore.DB.GetType().GetMethod("GetDatabaseString"));
            //LuaHandler.RegisterFunction("SetDB", Servercore.DB, Servercore.DB.GetType().GetMethod("SetDatabase"));
            LuaHandler.RegisterFunction("SendGlobalChat", luaChat, luaChat.GetType().GetMethod("SendGlobalChat"));
            LuaHandler.RegisterFunction("SendMapChat", luaChat, luaChat.GetType().GetMethod("SendMapChat"));
            LuaHandler.RegisterFunction("SendClientChat", luaChat, luaChat.GetType().GetMethod("SendClientChat"));
            
            // -- Variables
            LuaHandler["G_ServerName"] = _servercore.ServerName;
            LuaHandler["G_MOTD"] = _servercore.Motd;
            LuaHandler["G_Welcome"] = _servercore.WelcomeMessage;
            LuaHandler["G_MainMap"] = _servercore.MapMain;
            LuaHandler["G_Core"] = _servercore;
            LuaHandler["LogType_Info"] = LogType.Info;
        }

        public void RunFunction(string function, params object[] args) {
            var luaF = LuaHandler.GetFunction(function);

            try {
                if (luaF != null && args != null)
                    luaF.Call(args);
                else if (luaF != null)
                    luaF.Call();
            } catch (LuaScriptException e) {
                _servercore.Logger.Log("Lua", "Lua Error: " + e.Message, LogType.Error);
            }
        }

        public void Main() {
            while (_servercore.Running) {
                var files = Directory.GetFiles("Lua", "*.lua", SearchOption.AllDirectories);

                foreach (var file in files) {
                    if (!_scripts.ContainsKey(file)) { // -- New file, add it and load it.
                        _scripts.Add(file, File.GetLastWriteTime(file));

                        try {
                            LuaHandler.DoFile(file);
                        } catch (LuaScriptException e) {
                            _servercore.Logger.Log("Lua", "Lua Error: " + e.Message, LogType.Error);
                        }

                        continue;
                    }

                    if (File.GetLastWriteTime(file) != _scripts[file]) {
                        try {
                            LuaHandler.DoFile(file);
                        } catch (LuaScriptException e) {
                            _servercore.Logger.Log("Lua", "Lua Error: " + e.Message, LogType.Error);
                        }

                        _scripts[file] = File.GetLastWriteTime(file);
                    }
                }

                Thread.Sleep(1000);
            }
        }
    }
}
