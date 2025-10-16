
using System;
using System.Collections.Generic;

namespace LineUpGame
{
  public class Player
  {
    public char Symbol { get; }
    public string Name { get; }

    public List<Inventory> Inventories { get; set; } = new();

    public Player(char symbol, string name) { Symbol = symbol; Name = name; }

    public Inventory? GetInventoryByType(string type)
    {
      return Inventories.Find(i => i.Type == type);
    }

    public Inventory? GetInventoryBySymbol(char symbol)
    {
      return Inventories.Find(i => char.ToUpper(i.Symbol) == char.ToUpper(symbol));
    }

    public void ResetInventory(Inventory inv, int Count)
    {
      inv.Clear();
      InventoryFactory.CreateInventory(inv.Type, inv.Symbol, Count);
    }

    public bool HasDisc(string type)
    {
      var inv = GetInventoryByType(type);
      return inv != null && inv.Count() > 0;
    }
    public void Consume(string type)
    {
      var inv = GetInventoryByType(type);
      if (inv != null)
      {
        inv.Decrement();
      }
    }

    public void ReturnFromChar(char ch)
    {
      var inv = GetInventoryBySymbol(ch);
      if (inv != null)
      {
        inv.Increment(DiscFactory.CreateDisc(inv.Type, Symbol));
      }
    }

    public override string ToString() => Name;
  }
  public interface IAIPolicy
  {
    int ChooseColumn(char[,] grid, int winN, char self, char opponent, Random rng);
  }
  public sealed class WinBlockCenterPolicy : IAIPolicy
  {
    public bool CenterBias { get; init; } = true;
    public int StrongMinEmpty { get; init; } = 0;

    public int ChooseColumn(char[,] grid, int winN, char self, char opponent, Random rng)
    {
      int cols = grid.GetLength(1);
      for (int c = 0; c < cols; c++)
        if (SimWin(grid, c, self, winN)) return c;

      for (int c = 0; c < cols; c++)
        if (SimWin(grid, c, opponent, winN)) return c;

      var candidates = new List<(int col, double dist)>();
      double center = (cols - 1) / 2.0;
      for (int c = 0; c < cols; c++)
        if (Board.CanDropOnGrid(grid, c))
          candidates.Add((c, Math.Abs(c - center)));

      if (candidates.Count == 0) return -1;

      var strong = new List<(int col, double dist)>();
      foreach (var it in candidates)
        if (RemainingEmptySlots(grid, it.col) >= winN) strong.Add(it);

      int PickByCenter(List<(int col, double dist)> list)
      {
        double best = double.MaxValue;
        var bestCols = new List<int>();
        foreach (var (col, dist) in list)
        {
          if (dist < best - 1e-9) { best = dist; bestCols.Clear(); bestCols.Add(col); }
          else if (Math.Abs(dist - best) <= 1e-9) { bestCols.Add(col); }
        }
        return bestCols[rng.Next(bestCols.Count)];
      }

      return (strong.Count > 0) ? PickByCenter(strong) : PickByCenter(candidates);
    }
    private static bool SimWin(char[,] grid, int col, char sym, int winN)
    {
      if (!Board.CanDropOnGrid(grid, col)) return false;
      int row = Board.FindDropRowOnGrid(grid, col);
      if (row < 0) return false;

      grid[row, col] = sym;
      bool win = Board.CheckWinOnGrid(grid, row, col, winN, sym);
      grid[row, col] = ' ';
      return win;
    }
    private static int RemainingEmptySlots(char[,] grid, int col)
    {
      int rows = grid.GetLength(0);
      int count = 0;
      for (int r = 0; r < rows; r++)
      {
        if (grid[r, col] == ' ') count++;
        else break;
      }
      return count;
    }
  }
  class AIPlayer : Player
  {
    private readonly Random rng;
    private readonly IAIPolicy policy;

    public AIPlayer(char symbol, string name, Random? random = null, IAIPolicy? policy = null)
        : base(symbol, name)
    {
      rng = random ?? new Random();
      this.policy = policy ?? new WinBlockCenterPolicy();
    }

    public int ChooseColumn(char[,] grid, int winN, char opponent)
        => policy.ChooseColumn(grid, winN, this.Symbol, opponent, rng);

    public void MakeMove(Board board, int winN, char opponent)
    {
      if (!HasDisc("ordinary")) return;
      var grid = board.ExportGrid();
      int col = ChooseColumn(grid, winN, opponent);
      if (col >= 0 && board.DropDisc(col, DiscFactory.CreateDisc("ordinary", Symbol)))
        Consume("ordinary");
    }
  }
  class HumanPlayer : Player
  {
    public HumanPlayer(char symbol, string name) : base(symbol, name) { }
  }
}
