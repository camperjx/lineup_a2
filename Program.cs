using System;

namespace LineUpGame
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("==== LineUp Game ====");
                Console.WriteLine("1. LineUp Classic (with special discs)");
                Console.WriteLine("2. LineUp Basic (ordinary only)");
                Console.WriteLine("3. LineUp Spin (ordinary only + board rotates every 5 turns)");
                Console.WriteLine("4. Test Mode (run scripted sequence)");
                Console.WriteLine("5. Exit");
                Console.WriteLine("Type 'help' for command guide");
                Console.Write("Select option: ");

                string? opt = Console.ReadLine()?.Trim();

                if (opt != null && opt.Equals("help", StringComparison.OrdinalIgnoreCase))
                {
                    Utilities.ShowMainHelp();
                    continue;
                }

                if (opt == "1") new LineUpClassic().Run();
                else if (opt == "2") new LineUpBasic().Run();
                else if (opt == "3") new LineUpSpin().Run();
                else if (opt == "4") new LineUpTest().Run();
                else if (opt == "5" || opt?.ToLower() == "exit") break;
                else Console.WriteLine("Invalid option. Try again.");
            }
        }


    }
}
