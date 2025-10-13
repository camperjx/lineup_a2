public static class Utilities
{
  public static string[] Selected(Menu menu)
  {
    MenuRenderer.Render(menu);
    string? input = Console.ReadLine();
    try
    {
      return CommandParser.Parse(input);
    }
    catch (ArgumentException ex)
    {
      Console.WriteLine(ex.Message);
    }
    return Array.Empty<string>();
  }

  public static int OrdinaryDiscCount(Game game)
  {
    return game.Board.Rows * game.Board.Cols / 2 - game.CurrentPlayer.GetSpecialInventories().Count * 2;
  }

  private static string GetDir()
  {
    string? dir;
    while (true)
    {
      Console.WriteLine("Enter directory name: ");
      dir = Console.ReadLine();
      if (string.IsNullOrEmpty(dir))
      {
        Console.WriteLine("Directory name cannot be empty.");
        continue;
      }
      else
      {
        break;
      }
    }
    return dir;
  }

  private static void CreateDir(string dir)
  {
    if (!Directory.Exists(dir))
    {
      Directory.CreateDirectory(dir);
    }
  }

  private static string GetFilename()
  {
    string? filename;
    while (true)
    {
      Console.WriteLine("Enter file name: ");
      filename = Console.ReadLine();
      if (string.IsNullOrEmpty(filename))
      {
        Console.WriteLine("File name cannot be empty.");
        continue;
      }
      else
      {
        break;
      }
    }
    return filename;
  }

  public static string GetFullPath(bool shouldCreateDir = true)
  {
    string dir = GetDir();
    if (shouldCreateDir)
    {
      CreateDir(dir);
    }
    string filename = GetFilename();
    if (!Path.HasExtension(filename))
    {
      filename += ".json";
    }
    string fullPath = Path.Combine(dir, filename);
    return fullPath;
  }

  public static bool IsValidSymbol(Player player, string symbol)
  {
    foreach (DiscInventory inventory in player.Inventories)
    {
      if (inventory.Symbol == symbol)
      {
        return true;
      }
    }
    return false;
  }

  public static Disc CreateDisc(DiscType type, string symbol)
  {
    switch (type)
    {
      case DiscType.Ordinary:
        return new OrdinaryDisc(symbol);
      case DiscType.Boring:
        return new BoringDisc(symbol);
      case DiscType.Magnetic:
        return new MagneticDisc(symbol);
      case DiscType.Exploding:
        return new ExplodingDisc(symbol);
      default:
        throw new ArgumentException("Invalid disc type");
    }
  }

  public static int _alignedInRow(Board board, Disc disc)
  {
    return board.AlignedCountInDirection(disc, disc.Position!.Row, disc.Position!.Col, 0, 1) + board.AlignedCountInDirection(disc, disc.Position!.Row, disc.Position!.Col, 0, -1) - 1;
  }
  public static int _alignedInCol(Board board, Disc disc)
  {
    return board.AlignedCountInDirection(disc, disc.Position!.Row, disc.Position!.Col, 1, 0);
  }
  public static int _alignedInDiag1(Board board, Disc disc)
  {
    return board.AlignedCountInDirection(disc, disc.Position!.Row, disc.Position!.Col, 1, 1) + board.AlignedCountInDirection(disc, disc.Position!.Row, disc.Position!.Col, -1, -1) - 1;
  }
  public static int _alignedInDiag2(Board board, Disc disc)
  {
    return board.AlignedCountInDirection(disc, disc.Position!.Row, disc.Position!.Col, 1, -1) + board.AlignedCountInDirection(disc, disc.Position!.Row, disc.Position!.Col, -1, 1) - 1;
  }

  public static bool isWin(Game game, Board board, Disc disc)
  {
    return _alignedInRow(board, disc) >= game.countToWin() || _alignedInCol(board, disc) >= game.countToWin() || _alignedInDiag1(board, disc) >= game.countToWin() || _alignedInDiag2(board, disc) >= game.countToWin();
  }
  public static bool isDraw(Board board)
  {
    return board.IsFull();
  }
}
