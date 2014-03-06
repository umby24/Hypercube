using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using NLua;
using NLua.Exceptions;

using Hypercube_Classic.Core;

namespace Hypercube_Classic.Libraries {
    public struct LuaEvent {
        string ID;
        string Function;
        string Type;
        string Map;
        int Time;
    }

    /// <summary>
    /// The NLua Wrapper used for error checking and loading lua scripts.
    /// </summary>
    public class LuaWrapper {
        public Lua LuaHandler;
        Hypercube ServerCore;
        Dictionary<string, DateTime> Scripts;

        public LuaWrapper(Hypercube systemCore) {
            LuaHandler = new Lua();
            ServerCore = systemCore;
        }

        /// <summary>
        /// Exposes server functions to Lua.
        /// </summary>
        public void RegisterFunctions() {
            var luaChat = new Chat();

            // -- Functions
            LuaHandler.RegisterFunction("Log", ServerCore.Logger, ServerCore.Logger.GetType().GetMethod("_Log"));
            // -- Command creation functions
            LuaHandler.RegisterFunction("RegCmd", ServerCore.Commandholder, ServerCore.Commandholder.GetType().GetMethod("AddCommand"));
            LuaHandler.RegisterFunction("GetCmdAlias", ServerCore.Commandholder, ServerCore.Commandholder.GetType().GetMethod("GetAlias"));
            // -- Database functions
            LuaHandler.RegisterFunction("DBPlayerExists", ServerCore.Database, ServerCore.Database.GetType().GetMethod("ContainsPlayer"));
            LuaHandler.RegisterFunction("DBGetPlayerName", ServerCore.Database, ServerCore.Database.GetType().GetMethod("GetPlayerName"));
            LuaHandler.RegisterFunction("DBBanPlayer", ServerCore.Database, ServerCore.Database.GetType().GetMethod("BanPlayer"));
            LuaHandler.RegisterFunction("DBUnbanPlayer", ServerCore.Database, ServerCore.Database.GetType().GetMethod("UnbanPlayer"));
            LuaHandler.RegisterFunction("DBStopPlayer", ServerCore.Database, ServerCore.Database.GetType().GetMethod("StopPlayer"));
            LuaHandler.RegisterFunction("DBUnstopPlayer", ServerCore.Database, ServerCore.Database.GetType().GetMethod("UnstopPlayer"));
            LuaHandler.RegisterFunction("GetDBInt", ServerCore.Database, ServerCore.Database.GetType().GetMethod("GetDatabaseInt"));
            LuaHandler.RegisterFunction("GetDBString", ServerCore.Database, ServerCore.Database.GetType().GetMethod("GetDatabaseString"));
            //LuaHandler.RegisterFunction("SetDatabase", ServerCore.Database, ServerCore.Database.GetType().GetMethod("SetDatabase"));
            LuaHandler.RegisterFunction("SendGlobalChat", luaChat, luaChat.GetType().GetMethod("SendGlobalChat"));
            LuaHandler.RegisterFunction("SendMapChat", luaChat, luaChat.GetType().GetMethod("SendMapChat"));

            // -- Variables
            LuaHandler["G_ServerName"] = ServerCore.ServerName;
            LuaHandler["G_MOTD"] = ServerCore.MOTD;
            LuaHandler["G_Welcome"] = ServerCore.WelcomeMessage;
            LuaHandler["G_MainMap"] = ServerCore.MapMain;
            LuaHandler["G_Core"] = ServerCore;
        }

        /// <summary>
        /// Loads all lua scripts from the "Lua" Directory. Creates this directory if it does not exist.
        /// </summary>
        public void LoadLuaScripts() {
            Scripts = new Dictionary<string, DateTime>();

            if (!Directory.Exists("Lua"))
                Directory.CreateDirectory("Lua");

            string[] files = Directory.GetFiles("Lua", "*.lua", SearchOption.AllDirectories);

            foreach (string file in files) {
                Scripts.Add(file, File.GetLastWriteTime(file));
                
                try {
                    LuaHandler.DoFile(file);
                } catch (LuaScriptException e) {
                    ServerCore.Logger._Log("Error", "Lua", "Lua Error: " + e.Message);
                }
            }

            ServerCore.Logger._Log("INFO", "Lua", "Lua scripts loaded.");
        }

        /// <summary>
        /// Runs an individual lua function. This is File-agnostic.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="args"></param>
        public void RunLuaFunction(string function, params object[] args) {
            LuaFunction LuaF = LuaHandler.GetFunction(function);

            if (LuaF != null && args != null)
                LuaF.Call(args);
            else if (function != null)
                LuaF.Call();

        }

        /// <summary>
        /// Loops, checking for updates for currently loaded lua files. May also load newly added lua files.
        /// </summary>
        public void LuaMain() {
            while (ServerCore.Running) {
                string[] files = Directory.GetFiles("Lua", "*.lua", SearchOption.AllDirectories);

                foreach (string file in files) {
                    if (!Scripts.ContainsKey(file)) { // -- New file, add it and load it.
                        Scripts.Add(file, File.GetLastWriteTime(file));

                        try {
                            LuaHandler.DoFile(file);
                        } catch (LuaScriptException e) {
                            ServerCore.Logger._Log("Error", "Lua", "Lua Error: " + e.Message);
                        }

                        continue;
                    }

                    if (File.GetLastWriteTime(file) != Scripts[file]) { 
                        try {
                            LuaHandler.DoFile(file);
                        } catch (LuaScriptException e) {
                            ServerCore.Logger._Log("Error", "Lua", "Lua Error: " + e.Message);
                        }

                        Scripts[file] = File.GetLastWriteTime(file);
                    }
                }

                Thread.Sleep(1000);
            }
        }
    }
}
