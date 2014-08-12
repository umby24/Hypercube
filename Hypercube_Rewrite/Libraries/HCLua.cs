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
                    Hypercube.Logger.Log("Lua", "Lua Error: " + e.Message, LogType.Error);
                }
            }

            Hypercube.Logger.Log("Lua", "Lua scripts loaded.", LogType.Info);
        }

        /// <summary>
        /// Exposes server functions to Lua.
        /// </summary>
        public void RegisterFunctions() {
            var luaChat = new Chat();

            // -- Functions
            LuaHandler.RegisterFunction("Log", Hypercube.Logger, Hypercube.Logger.GetType().GetMethod("Log"));
            // -- Command creation functions
            LuaHandler.RegisterFunction("RegCmd", Hypercube.Commandholder, Hypercube.Commandholder.GetType().GetMethod("RegisterCommand"));
            LuaHandler.RegisterFunction("GetCmdAlias", Hypercube.Commandholder, Hypercube.Commandholder.GetType().GetMethod("GetAlias"));
            // -- DB functions
            LuaHandler.RegisterFunction("DBPlayerExists", Hypercube.DB, Hypercube.DB.GetType().GetMethod("ContainsPlayer"));
            LuaHandler.RegisterFunction("DBGetPlayerName", Hypercube.DB, Hypercube.DB.GetType().GetMethod("GetPlayerName"));
            LuaHandler.RegisterFunction("DBBanPlayer", Hypercube.DB, Hypercube.DB.GetType().GetMethod("BanPlayer"));
            LuaHandler.RegisterFunction("DBUnbanPlayer", Hypercube.DB, Hypercube.DB.GetType().GetMethod("UnbanPlayer"));
            LuaHandler.RegisterFunction("DBStopPlayer", Hypercube.DB, Hypercube.DB.GetType().GetMethod("StopPlayer"));
            LuaHandler.RegisterFunction("DBUnstopPlayer", Hypercube.DB, Hypercube.DB.GetType().GetMethod("UnstopPlayer"));
            LuaHandler.RegisterFunction("GetDBInt", Hypercube.DB, Hypercube.DB.GetType().GetMethod("GetDatabaseInt"));
            LuaHandler.RegisterFunction("GetDBString", Hypercube.DB, Hypercube.DB.GetType().GetMethod("GetDatabaseString"));
            //LuaHandler.RegisterFunction("SetDB", Servercore.DB, Servercore.DB.GetType().GetMethod("SetDatabase"));
            LuaHandler.RegisterFunction("SendGlobalChat", luaChat, luaChat.GetType().GetMethod("SendGlobalChat"));
            LuaHandler.RegisterFunction("SendMapChat", luaChat, luaChat.GetType().GetMethod("SendMapChat"));
            LuaHandler.RegisterFunction("SendClientChat", luaChat, luaChat.GetType().GetMethod("SendClientChat"));
            
            // -- Variables
            LuaHandler["G_ServerName"] = Hypercube.ServerName;
            LuaHandler["G_MOTD"] = Hypercube.Motd;
            LuaHandler["G_Welcome"] = Hypercube.WelcomeMessage;
            LuaHandler["G_MainMap"] = Hypercube.MapMain;
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
                Hypercube.Logger.Log("Lua", "Lua Error: " + e.Message, LogType.Error);
            }
        }

        public void Main() {
            while (Hypercube.Running) {
                var files = Directory.GetFiles("Lua", "*.lua", SearchOption.AllDirectories);

                foreach (var file in files) {
                    if (!_scripts.ContainsKey(file)) { // -- New file, add it and load it.
                        _scripts.Add(file, File.GetLastWriteTime(file));

                        try {
                            LuaHandler.DoFile(file);
                        } catch (LuaScriptException e) {
                            Hypercube.Logger.Log("Lua", "Lua Error: " + e.Message, LogType.Error);
                        }

                        continue;
                    }

                    if (File.GetLastWriteTime(file) != _scripts[file]) {
                        try {
                            LuaHandler.DoFile(file);
                        } catch (LuaScriptException e) {
                            Hypercube.Logger.Log("Lua", "Lua Error: " + e.Message, LogType.Error);
                        }

                        _scripts[file] = File.GetLastWriteTime(file);
                    }
                }

                Thread.Sleep(1000);
            }
        }
    }
}
