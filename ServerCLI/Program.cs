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
            var server = new Hypercube.Hypercube();
            Console.Title = "Hypercube";

            server.Start();

            var input = "";
            
            while (input.ToLower() != "end") 
                input = Console.ReadLine();
            

            server.Stop();
        }
    }
}
