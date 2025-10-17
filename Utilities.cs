namespace LineUpGame
{
  public class Utilities
  {
    public static bool GetRowsAndCols(out int? rows, out int? cols)
    {
      rows = 0;
      cols = 0;
      Console.WriteLine("Rows (min 6): ");
      string? rowsOpt = Console.ReadLine()?.Trim();

      bool isRowsValid = IntParser.TryParse(rowsOpt, out rows);

      if (!isRowsValid || rows < 6)
      {
        Console.WriteLine("Invalid rows. Please try again.");
        return false;
      }

      Console.WriteLine("Cols (min 7): ");
      string? colsOpt = Console.ReadLine()?.Trim();

      bool isColsValid = IntParser.TryParse(colsOpt, out cols);

      if (!isColsValid || cols < 7)
      {
        Console.WriteLine("Invalid cols. Please try again.");
        return false;
      }
      return true;
    }

    public static bool GetGameType(out string? gameType)
    {
      gameType = "0";
      Console.WriteLine();
      MenuRenderer.Render(new GameTypeMenu());
      string? gameTypeOpt = Console.ReadLine()?.Trim();

      bool isValid = IntParser.TryParse(gameTypeOpt, out int? opt);

      if (!isValid)
      {
        Console.WriteLine("Invalid game type. Please try again.");
        return false;
      }
      gameType = opt.ToString();
      return true;
    }

    public static bool GetPlayMode(out string? mode)
    {
      mode = "HvH";
      MenuRenderer.Render(new PlayModeMenu());
      string? playModeOpt = Console.ReadLine()?.Trim();

      bool isPlayModeValid = IntParser.TryParse(playModeOpt, out int? opt);

      if (!isPlayModeValid)
      {
        Console.WriteLine("Invalid play mode. Please try again.");
        return false;
      }
      mode = opt == 1 ? "HvH" : "HvC";
      return true;
    }
  }
}
