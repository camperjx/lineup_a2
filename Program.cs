using System;

namespace LineUpGame
{
  class Program
  {
    static void Main()
    {
      Console.WriteLine("==== LineUp Game ====");

      bool shouldBreak = false;

      while (true)
      {
        bool isGameTypeValid = Utilities.GetGameType(out string? gameType);
        if (!isGameTypeValid)
        {
          Console.WriteLine("Invalid input.");
          continue;
        }

        switch (gameType)
        {
          case "1":
            bool isModeValid = Utilities.GetPlayMode(out string? playMode);

            if (!isModeValid)
            {
              Console.WriteLine("Invalid input.");
              break;
            }

            bool isRowsAndColsValid = Utilities.GetRowsAndCols(out int? rows, out int? cols);

            if (!isRowsAndColsValid)
            {
              Console.WriteLine("Invalid input.");
              break;
            }
            new LineUpClassic(rows!.Value, cols!.Value, playMode!).TurnLoop();
            break;
          case "2":
            isModeValid = Utilities.GetPlayMode(out playMode);

            if (!isModeValid)
            {
              Console.WriteLine("Invalid input.");
              break;
            }
            new LineUpBasic(playMode!).TurnLoop();
            break;
          case "3":
            isModeValid = Utilities.GetPlayMode(out playMode);

            if (!isModeValid)
            {
              Console.WriteLine("Invalid input.");
              break;
            }
            new LineUpSpin(playMode!).TurnLoop();
            break;
          case "4":
            new LineUpTest().TrunLoop();
            break;
          case "5":
            MenuRenderer.Render(new HelpMenu());
            break;
          case "6":
            shouldBreak = true;
            break;
          default:
            Console.WriteLine("Invalid option. Try again.");
            break;
        }
        if (shouldBreak)
        {
          break;
        }
      }
    }
  }
}
