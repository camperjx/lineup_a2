/*
  Using an interface instead of abstract class to define a contract for renderers.
  Type parameter T allows for flexibility in input types.
*/
public interface IRenderer<T>
{
  static abstract void Render(T obj);
}

public class BoardRenderer : IRenderer<Board>
{
  public static void Render(Board board)
  {
    for (int r = 0; r < board.Rows; r++)
    {
      Console.Write("|");
      for (int c = 0; c < board.Cols; c++)
      {
        if (board.Discs[r][c] != null)
        {
          Disc disc = board.Discs[r][c]!;
          string symbol = disc.Symbol;
          Console.Write($" {symbol} |");
        }
        else
        {
          Console.Write($"   |");
        }
      }
      Console.WriteLine();
    }
    Console.Write("|");
    for (int c = 0; c < board.Cols; c++)
    {
      Console.Write($" {c} |");
    }
    Console.WriteLine();
  }
}

public class GameStatsRenderer : IRenderer<Game>
{
  public static void Render(Game game)
  {
    Console.WriteLine("======== Game Stats ========");
    foreach (Player player in game.Players)
    {
      Console.WriteLine($"{player.Name}'s discs:");
      foreach (DiscInventory inventory in player.Inventories)
      {
        Console.WriteLine(inventory.ToString());
      }
    }
    Console.WriteLine("============================");
  }
}

public class MenuRenderer : IRenderer<Menu>
{
  public static void Render(Menu menu)
  {
    if (menu.isMultipleSelection)
    {
      Console.WriteLine("Select options (separated by space):");
    }
    else
    {
      Console.WriteLine("Select an option:");
    }
    foreach (var item in menu.Items)
    {
      Console.WriteLine($"{item.Key}: {item.Description}");
    }
  }
}
