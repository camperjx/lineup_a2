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
                Console.WriteLine("4. Exit");
                Console.WriteLine("Type 'help' for command guide");
                Console.Write("Select option: ");

                string? opt = Console.ReadLine()?.Trim();

                // 🔧 新增：在主菜单阶段也能输入 help
                if (opt != null && opt.Equals("help", StringComparison.OrdinalIgnoreCase))
                {
                    ShowMainHelp();
                    continue;
                }

                if (opt == "1") new LineUpClassic().Run();
                else if (opt == "2") new LineUpBasic().Run();
                else if (opt == "3") new LineUpSpin().Run();
                else if (opt == "4" || opt?.ToLower() == "exit") break;
                else Console.WriteLine("Invalid option. Try again.");
            }
        }

        /// <summary>
        /// 显示帮助菜单（在主菜单阶段调用）
        /// </summary>
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
            Console.WriteLine("=============================");
            Console.WriteLine("Press Enter to return to main menu...");
            Console.ReadLine(); // 暂停，避免刷掉
        }
    }
}
