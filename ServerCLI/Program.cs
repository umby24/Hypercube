using System;

namespace ServerCLI {
    class Program {
        static void Main(string[] args) {
            Console.Title = "Hypercube";

            Hypercube.Hypercube.Setup();
            Hypercube.Hypercube.Start();

            var input = "";
            
            while (input != null && input.ToLower() != "end") 
                input = Console.ReadLine();

            Hypercube.Hypercube.Stop();
        }
    }
}
