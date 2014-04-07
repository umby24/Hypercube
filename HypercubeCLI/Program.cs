using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Resources;

using Hypercube_Classic;
using Hypercube_Classic.Core;

namespace HypercubeCLI {
    class Program {

        static void Main(string[] args) {
            if (!File.Exists("lua52.dll")) {
                
            }

            var Server = new Hypercube();
            Server.Start();

            string Input = "";

            while (Input != "END") {
                Input = Console.ReadLine();

                if (Input.ToLower().StartsWith("chat ")) 
                    Chat.SendGlobalChat(Server, "&c[Server]:&f " + Input.Substring(5, Input.Length - 5));
                
            }

            Server.Stop();
        }
    }
}
