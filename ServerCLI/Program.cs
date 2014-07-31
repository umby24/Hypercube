using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hypercube;
using Hypercube.Core;

namespace ServerCLI {
    class Program {
        static void Main(string[] args) {
            var Server = new Hypercube.Hypercube();
            Console.Title = "Hypercube";

            Server.Start();

            string Input = "";
            
            while (Input.ToLower() != "end") 
                Input = Console.ReadLine();
            

            Server.Stop();
        }
    }
}
