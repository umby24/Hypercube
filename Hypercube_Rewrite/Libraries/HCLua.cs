using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

using NLua;
using NLua.Exceptions;
using Hypercube.Core;

namespace Hypercube.Libraries {
    public class HCLua {
        public Lua LuaHandler;
        public Thread luaThread;

        Hypercube Servercore;
        Dictionary<string, DateTime> Scripts;

        public HCLua(Hypercube Core) {
            Servercore = Core;
            LuaHandler = new Lua();
        }

        public void LoadScripts() {
            Scripts = new Dictionary<string, DateTime>();

            if (Directory.Exists("Lua") == false)
                Directory.CreateDirectory("Lua");

            var files = Directory.GetFiles("Lua", "*.lua", SearchOption.AllDirectories);

            foreach (var file in files) {
                Scripts.Add(file, File.GetLastWriteTime(file));

                try {
                    LuaHandler.DoFile(file);
                } catch (LuaScriptException e) {
                    Servercore.Logger.Log("Lua", "Lua Error: " + e.Message, LogType.Error);
                }
            }

            Servercore.Logger.Log("Lua", "Lua scripts loaded.", LogType.Info);
        }

        /// <summary>
        /// Exposes server functions to Lua.
        /// </summary>
        public void RegisterFunctions() {
            var luaChat = new Chat();

            // -- Functions
            LuaHandler.RegisterFunction("Log", Servercore.Logger, Servercore.Logger.GetType().GetMethod("Log"));
            // -- Command creation functions
            LuaHandler.RegisterFunction("RegCmd", Servercore.Commandholder, Servercore.Commandholder.GetType().GetMethod("RegisterCommand"));
            LuaHandler.RegisterFunction("GetCmdAlias", Servercore.Commandholder, Servercore.Commandholder.GetType().GetMethod("GetAlias"));
            // -- DB functions
            LuaHandler.RegisterFunction("DBPlayerExists", Servercore.DB, Servercore.DB.GetType().GetMethod("ContainsPlayer"));
            LuaHandler.RegisterFunction("DBGetPlayerName", Servercore.DB, Servercore.DB.GetType().GetMethod("GetPlayerName"));
            LuaHandler.RegisterFunction("DBBanPlayer", Servercore.DB, Servercore.DB.GetType().GetMethod("BanPlayer"));
            LuaHandler.RegisterFunction("DBUnbanPlayer", Servercore.DB, Servercore.DB.GetType().GetMethod("UnbanPlayer"));
            LuaHandler.RegisterFunction("DBStopPlayer", Servercore.DB, Servercore.DB.GetType().GetMethod("StopPlayer"));
            LuaHandler.RegisterFunction("DBUnstopPlayer", Servercore.DB, Servercore.DB.GetType().GetMethod("UnstopPlayer"));
            LuaHandler.RegisterFunction("GetDBInt", Servercore.DB, Servercore.DB.GetType().GetMethod("GetDatabaseInt"));
            LuaHandler.RegisterFunction("GetDBString", Servercore.DB, Servercore.DB.GetType().GetMethod("GetDatabaseString"));
            //LuaHandler.RegisterFunction("SetDB", Servercore.DB, Servercore.DB.GetType().GetMethod("SetDatabase"));
            LuaHandler.RegisterFunction("SendGlobalChat", luaChat, luaChat.GetType().GetMethod("SendGlobalChat"));
            LuaHandler.RegisterFunction("SendMapChat", luaChat, luaChat.GetType().GetMethod("SendMapChat"));
            LuaHandler.RegisterFunction("SendClientChat", luaChat, luaChat.GetType().GetMethod("SendClientChat"));
            
            // -- Variables
            LuaHandler["G_ServerName"] = Servercore.ServerName;
            LuaHandler["G_MOTD"] = Servercore.MOTD;
            LuaHandler["G_Welcome"] = Servercore.WelcomeMessage;
            LuaHandler["G_MainMap"] = Servercore.MapMain;
            LuaHandler["G_Core"] = Servercore;
            LuaHandler["LogType_Info"] = LogType.Info;
        }

        public void RunFunction(string function, params object[] args) {
            var LuaF = LuaHandler.GetFunction(function);

            try {
                if (LuaF != null && args != null)
                    LuaF.Call(args);
                else if (LuaF != null)
                    LuaF.Call();
            } catch (LuaScriptException e) {
                Servercore.Logger.Log("Lua", "Lua Error: " + e.Message, LogType.Error);
            }
        }

        public void Main() {
            while (Servercore.Running) {
                var files = Directory.GetFiles("Lua", "*.lua", SearchOption.AllDirectories);

                foreach (var file in files) {
                    if (!Scripts.ContainsKey(file)) { // -- New file, add it and load it.
                        Scripts.Add(file, File.GetLastWriteTime(file));

                        try {
                            LuaHandler.DoFile(file);
                        } catch (LuaScriptException e) {
                            Servercore.Logger.Log("Lua", "Lua Error: " + e.Message, LogType.Error);
                        }

                        continue;
                    }

                    if (File.GetLastWriteTime(file) != Scripts[file]) {
                        try {
                            LuaHandler.DoFile(file);
                        } catch (LuaScriptException e) {
                            Servercore.Logger.Log("Lua", "Lua Error: " + e.Message, LogType.Error);
                        }

                        Scripts[file] = File.GetLastWriteTime(file);
                    }
                }

                Thread.Sleep(1000);
            }
        }
    }
}
