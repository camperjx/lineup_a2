using System;

namespace LineUpGame
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("==== LineUp Game ====");

      bool shouldBreak = false;

      while (true)
      {
        Console.WriteLine();
        MenuRenderer.Render(new GameTypeMenu());

        string? opt = Console.ReadLine()?.Trim();

        switch (opt)
        {
          case "1":
            new LineUpClassic().Run();
            break;
          case "2":
            new LineUpBasic().Run();
            break;
          case "3":
            new LineUpSpin().Run();
            break;
          case "4":
            new LineUpTest().Run();
            break;
          case "h":
            MenuRenderer.Render(new HelpMenu());
            break;
          case "x":
            shouldBreak = true;
            break;
          default:
            Console.WriteLine("Invalid option. Try again.");
            break;
        }
        if (shouldBreak) break;
      }
    }


  }
}
