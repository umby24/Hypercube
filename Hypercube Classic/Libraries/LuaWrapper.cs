using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using NLua;

namespace Hypercube_Classic.Libraries {
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
        /// Loads all lua scripts from the "Lua" Directory. Creates this directory if it does not exist.
        /// </summary>
        public void LoadLuaScripts() {
            Scripts = new Dictionary<string, DateTime>();

            if (!Directory.Exists("Lua"))
                Directory.CreateDirectory("Lua");

            string[] files = Directory.GetFiles("Lua", "*.lua", SearchOption.AllDirectories);

            foreach (string file in files) {
                Scripts.Add(file, File.GetLastWriteTime(file));
                LuaHandler.DoFile(file);
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
                    if (File.GetLastWriteTime(file) != Scripts[file]) {
                        LuaHandler.DoFile(file);
                        Scripts[file] = File.GetLastWriteTime(file);
                    }
                }
                Thread.Sleep(1000);
            }
        }
    }
}
