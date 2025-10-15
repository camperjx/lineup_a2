public static class Utilities
{
  public static void ShowMainHelp()
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
