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
                    ShowMainHelp();
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

        static void ShowMainHelp()
        {
            Console.WriteLine();
            Console.WriteLine("=== Help Menu (Main Menu) ===");
            Console.WriteLine("In-game commands:");
            Console.WriteLine("  move <col> <ordinary|boring|magnet|explode>");
            Console.WriteLine("     e.g., move 2 magnet");
            Console.WriteLine("  <col>            Quick ordinary drop (e.g., 3)");
            Console.WriteLine("  save <filename>  Save current game");
            Console.WriteLine("  load <filename>  Load saved game");
            Console.WriteLine("  undo             Undo last move");
            Console.WriteLine("  redo             Redo last undone move");
            Console.WriteLine("  help             Show this help menu");
            Console.WriteLine("  exit             Exit the game");
            Console.WriteLine();
            Console.WriteLine("Special modes:");
            Console.WriteLine("  Classic = full game with special discs");
            Console.WriteLine("  Basic   = ordinary discs only");
            Console.WriteLine("  Spin    = Basic + board rotates every 5 turns");
            Console.WriteLine("  Test    = run scripted sequence (e.g., O4,M5,B2)");
            Console.WriteLine("=============================");
            Console.WriteLine("Press Enter to return to main menu...");
            Console.ReadLine();
        }
    }
}
