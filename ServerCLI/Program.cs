﻿using System;
using Hypercube;

namespace ServerCLI {
    class Program {
        static void Main(string[] args) {
            Console.Title = "Hypercube";

            ServerCore.Setup();
            ServerCore.Start();

            var input = "";
            
            while (input != null && input.ToLower() != "end") 
                input = Console.ReadLine();

            ServerCore.Stop();
        }
    }
}
