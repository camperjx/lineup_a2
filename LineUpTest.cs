using System.Diagnostics.CodeAnalysis;

namespace LineUpGame
{
  class LineUpTest : Game
  {
    [SetsRequiredMembers]
    public LineUpTest()
    {
      history = new History();
      board = new Board(6, 7);
      p1 = new HumanPlayer('@', "Player1");
      p2 = playMode == "HvC" ? new AIPlayer('#', "Computer") : new HumanPlayer('#', "Player2");
      p1.Inventories = new List<Inventory>{
        InventoryFactory.CreateInventory("ordinary", char.Parse("@"), 42),
        InventoryFactory.CreateInventory("boring", char.Parse("@"), 42),
        InventoryFactory.CreateInventory("magnet", char.Parse("@"), 42),
        InventoryFactory.CreateInventory("exploding", char.Parse("@"), 42)
      };
      p2.Inventories = new List<Inventory>{
        InventoryFactory.CreateInventory("ordinary", char.Parse("#"), 42),
        InventoryFactory.CreateInventory("boring", char.Parse("#"), 42),
        InventoryFactory.CreateInventory("magnet", char.Parse("#"), 42),
        InventoryFactory.CreateInventory("exploding", char.Parse("#"), 42)
      };
      currentPlayer = p1;
      winCondition = 4;
    }
    public void TrunLoop()
    {
      Console.WriteLine("Enter test sequence (e.g., O4,M5,B2,O6):");
      string? line = Console.ReadLine();
      if (string.IsNullOrWhiteSpace(line)) return;

      var moves = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
      BoardRenderer.Render(board);

      foreach (var move in moves)
      {
        string trimmed = move.Trim();
        if (trimmed.Length < 2) continue;

        char typeChar = char.ToUpper(trimmed[0]);
        if (!int.TryParse(trimmed.Substring(1), out int col)) continue;

        // Convert from 1-based to 0-based column index
        col -= 1;

        string discType = typeChar switch
        {
          'O' => "ordinary",
          'B' => "boring",
          'M' => "magnet",
          'E' => "exploding",
          _ => "ordinary"
        };

        Disc d = DiscFactory.CreateDisc(discType, currentPlayer.Symbol);
        if (board.DropDisc(col, d))
        {
          currentPlayer.Consume(discType);
          BoardRenderer.Render(board);

          if (board.CheckWin(currentPlayer.Symbol, winCondition))
          {
            Console.WriteLine($"{currentPlayer.Name} wins!");
            return;
          }

          currentPlayer = (currentPlayer == p1) ? p2 : p1;
        }
        else
        {
          Console.WriteLine($"Invalid move or column full: {trimmed}");
        }
      }

      Console.WriteLine("Test sequence complete.");
    }
  }
}
