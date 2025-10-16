using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LineUpGame
{
  public abstract class Game
  {
    public required History history { get; set; } = new();

    public required Board board { get; set; }
    public required Player p1 { get; set; }
    public required Player p2 { get; set; }
    public required Player currentPlayer { get; set; }
    public required int winCondition { get; set; }
    public required string playMode { get; set; } = "HvH";
    public required int turnCount { get; set; } = 0;
    public required bool ordinaryOnly { get; set; } = false;
    public required bool spinEnabled { get; set; } = false;

    protected virtual int GetTotalCells()
    {
      return board.Rows * board.Columns;
    }

    protected virtual int GetPerPlayer()
    {
      return GetTotalCells() / 2;
    }

    protected virtual int GetOrdinaryCount()
    {
      return Math.Max(0, GetPerPlayer() - 6);
    }

    protected virtual void SaveState()
    {
      history.Add(CaptureCurrentState());
    }

    protected virtual GameState CaptureCurrentState()
    {
      return new GameState
      {
        BoardData = board.Serialize(),
        P1Inventory = p1.Inventories.ToDictionary(inv => inv.Type, inv => inv.Count()),
        P2Inventory = p2.Inventories.ToDictionary(inv => inv.Type, inv => inv.Count()),
        CurrentPlayer = currentPlayer == p1 ? "P1" : "P2",
        TurnCount = turnCount
      };
    }

    private void RestoreState(GameState st)
    {
      board.Deserialize(st.BoardData);
      p1.Inventories.Clear();
      foreach (var kv in st.P1Inventory)
      {
        var inv = InventoryFactory.CreateInventory(kv.Key, char.Parse("@"), kv.Value);
        p1.Inventories.Add(inv);
      }
      p2.Inventories.Clear();
      foreach (var kv in st.P2Inventory)
      {
        var inv = InventoryFactory.CreateInventory(kv.Key, char.Parse("#"), kv.Value);
        p2.Inventories.Add(inv);
      }
      currentPlayer = (st.CurrentPlayer == "P1") ? p1 : p2;
      turnCount = st.TurnCount;
    }

    protected bool ExecuteHumanCommand(string input)
    {
      /*
        Take move input, parse and execute.
        Handle conditionally depending on parsing results:
        1. Disc type + integer: full command that can drop any disc type
        2. Integer only: quick ordinary drop
      */
      bool isSymbolParsed = CharParser.TryParse(input[0..1], out char? symbol);
      if (isSymbolParsed)
      {
        bool isColParsed = IntParser.TryParse(input[1..], out int? col);
        if (!isColParsed)
        {
          Console.WriteLine("Invalid input.");
          return false;
        }
        string type = symbol switch
        {
          'O' or 'o' => "ordinary",
          'B' or 'b' => "boring",
          'M' or 'm' => "magnet",
          'E' or 'e' => "exploding",
          _ => ""
        };
        if (type == "")
        {
          Console.WriteLine("Invalid disc type.");
          return false;
        }
        if (ordinaryOnly)
        {
          type = "ordinary";
        }
        if (!currentPlayer.HasDisc(type))
        {
          Console.WriteLine($"No {type} discs remaining.");
          return false;
        }
        Disc disc = DiscFactory.CreateDisc(type, currentPlayer.Symbol);
        SaveState();
        if (board.DropDisc(col!.Value, disc))
        {
          currentPlayer.Consume(type);
          return true;
        }
        return false;
      }
      else
      {
        // Quick ordinary drop
        bool isColParsed = IntParser.TryParse(input, out int? col);
        if (!isColParsed)
        {
          Console.WriteLine("Invalid command.");
          return false;
        }
        string type = "ordinary";
        if (!currentPlayer.HasDisc(type))
        {
          Console.WriteLine("No ordinary discs.");
          return false;
        }
        Disc disc = DiscFactory.CreateDisc(type, currentPlayer.Symbol);
        SaveState();
        if (board.DropDisc(col!.Value, disc))
        {
          currentPlayer.Consume(type);
          return true;
        }
        return false;
      }
    }

    private char GetOpponentSymbol(Player currentPlayer)
    {
      return currentPlayer == p1 ? p2.Symbol : p1.Symbol;
    }

    public void TurnLoop()
    {
      turnCount = 0;
      while (!board.IsFull())
      {
        BoardRenderer.Render(board);
        Console.WriteLine($"Win if you connect {winCondition}.");

        string s = "Inv P1(O/B/M/E): ";

        foreach (var inv in p1.Inventories)
        {
          s += $"{inv.Count()}/";
        }

        s = s.TrimEnd('/');
        s += " | P2: Inv P2(O/B/M/E): ";

        foreach (var inv in p2.Inventories)
        {
          s += $"{inv.Count()}/";
        }
        Console.WriteLine(s.TrimEnd('/'));

        // Check if currentPlayer player is AI
        if (currentPlayer is AIPlayer ai)
        {
          char[,] grid = board.ExportGrid();
          int winN = winCondition;
          char opponent = GetOpponentSymbol(currentPlayer);

          int col = ai.ChooseColumn(grid, winN, opponent);
          if (col >= 0 && board.DropDisc(col, DiscFactory.CreateDisc("ordinary", ai.Symbol)))
          {
            ai.Consume("ordinary");
          }
        }
        else
        {
          // Human player - prompt for input
          Console.Write($"{currentPlayer.Name} ({currentPlayer.Symbol}): enter command: ");
          string? line = Console.ReadLine()?.Trim();
          if (string.IsNullOrWhiteSpace(line)) continue;

          // Handle 'help' command
          if (line.Equals("help", StringComparison.OrdinalIgnoreCase))
          {
            MenuRenderer.Render(new HelpMenu());
            continue;
          }

          // Handle undo
          if (line.Equals("undo", StringComparison.OrdinalIgnoreCase))
          {
            var prev = history.Undo(CaptureCurrentState());
            if (prev == null)
            {
              Console.WriteLine("No moves to undo.");
              continue;
            }
            RestoreState(prev);
            Console.WriteLine("Undo successful.");
          }

          // Handle redo
          if (line.Equals("redo", StringComparison.OrdinalIgnoreCase))
          {
            var next = history.Redo(CaptureCurrentState());
            if (next == null)
            {
              Console.WriteLine("No moves to redo.");
              continue;
            }
            RestoreState(next);
            Console.WriteLine("Redo successful.");
          }

          // Handle save
          if (line.StartsWith("save", StringComparison.OrdinalIgnoreCase))
          {
            string[] ps = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string file = ps.Length > 1 ? ps[1] : "savegame.txt";
            Save(file);
            continue;
          }

          // Handle load
          if (line.StartsWith("load", StringComparison.OrdinalIgnoreCase))
          {
            string[] ps = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string file = ps.Length > 1 ? ps[1] : "savegame.txt";
            if (Load(file)) Console.WriteLine("Game loaded.");
            continue;
          }

          // Try to execute the move
          bool moved = ExecuteHumanCommand(line);
          if (!moved) continue;
        }

        // Check for win after move
        if (board.CheckWin(currentPlayer.Symbol, winCondition))
        {
          BoardRenderer.Render(board);
          Console.WriteLine($"{currentPlayer.Name} wins!");
          return;
        }

        // Handle board rotation for Spin mode
        turnCount++;
        if (spinEnabled && turnCount % 5 == 0)
        {
          Console.WriteLine(">>> Board rotates 90° clockwise and gravity reapplies!");
          board.RotateClockwise();
          board.ApplyGravity();
        }

        // Switch to other player
        currentPlayer = (currentPlayer == p1) ? p2 : p1;
      }

      BoardRenderer.Render(board);
      Console.WriteLine("Board full. It's a tie.");
    }

    public void Save(string filename)
    {
      using var w = new System.IO.StreamWriter(filename);
      w.WriteLine(board.Rows);
      w.WriteLine(board.Columns);
      w.WriteLine(winCondition);
      w.WriteLine(currentPlayer == p1 ? "P1" : "P2");
      w.WriteLine(playMode);
      w.WriteLine(this.GetType().Name);

      string s = "";
      foreach (var inv in p1.Inventories)
      {
        s += $"{inv.Count()},";
      }
      s = s.TrimEnd(',');
      w.WriteLine(s);

      s = "";
      foreach (var inv in p2.Inventories)
      {
        s += $"{inv.Count()},";
      }
      s = s.TrimEnd(',');
      w.WriteLine(s);

      for (int r = 0; r < board.Rows; r++)
      {
        for (int c = 0; c < board.Columns; c++) w.Write(board.GetCell(r, c));
        w.WriteLine();
      }
      Console.WriteLine($"Game saved to {filename}");
    }

    public bool Load(string filename)
    {
      if (!System.IO.File.Exists(filename)) { Console.WriteLine($"File {filename} not found."); return false; }
      var lines = System.IO.File.ReadAllLines(filename);
      int idx = 0;
      int rows = int.Parse(lines[idx++]);
      int cols = int.Parse(lines[idx++]);
      winCondition = int.Parse(lines[idx++]);
      string curStr = lines[idx++];
      playMode = lines[idx++];
      string gameType = lines[idx++];
      var p1Inv = lines[idx++].Split(',');
      var p2Inv = lines[idx++].Split(',');

      p1 = new HumanPlayer('@', "Player1");
      p2 = playMode == "HvC" ? new AIPlayer('#', "Computer") : new HumanPlayer('#', "Player2");

      p1.Inventories = new List<Inventory>{
        InventoryFactory.CreateInventory("ordinary", char.Parse("@"), int.Parse(p1Inv[0])),
        InventoryFactory.CreateInventory("boring", char.Parse("@"), int.Parse(p1Inv[1])),
        InventoryFactory.CreateInventory("magnet", char.Parse("@"), int.Parse(p1Inv[2])),
        InventoryFactory.CreateInventory("exploding", char.Parse("@"), int.Parse(p1Inv[3]))
      };
      p2.Inventories = new List<Inventory>{
        InventoryFactory.CreateInventory("ordinary", char.Parse("#"), int.Parse(p2Inv[0])),
        InventoryFactory.CreateInventory("boring", char.Parse("#"), int.Parse(p2Inv[1])),
        InventoryFactory.CreateInventory("magnet", char.Parse("#"), int.Parse(p2Inv[2])),
        InventoryFactory.CreateInventory("exploding", char.Parse("#"), int.Parse(p2Inv[3]))
      };

      GameRegistry.P1 = p1;
      GameRegistry.P2 = p2;
      currentPlayer = curStr == "P1" ? p1 : p2;

      board = new Board(rows, cols);
      for (int r = 0; r < rows; r++)
      {
        string line = lines[idx++];
        for (int c = 0; c < cols && c < line.Length; c++) board.SetCell(r, c, line[c]);
      }
      Console.WriteLine($"Loaded: {gameType} {rows}x{cols} {playMode}, next={currentPlayer.Name}");
      return true;
    }
  }

  class LineUpClassic : Game
  {
    [SetsRequiredMembers]
    public LineUpClassic(int rows, int cols, string playMode_)
    {
      history = new History();
      board = new Board(rows, cols);
      playMode = playMode_;
      p1 = new HumanPlayer('@', "Player1");
      p2 = playMode == "HvC" ? new AIPlayer('#', "Computer") : new HumanPlayer('#', "Player2");
      currentPlayer = p1;
      GameRegistry.P1 = p1;
      GameRegistry.P2 = p2;
      p1.Inventories = new List<Inventory>{
        InventoryFactory.CreateInventory("ordinary", char.Parse("@"), GetOrdinaryCount()),
        InventoryFactory.CreateInventory("boring", char.Parse("@"), 2),
        InventoryFactory.CreateInventory("magnet", char.Parse("@"), 2),
        InventoryFactory.CreateInventory("exploding", char.Parse("@"), 2)
      };
      p2.Inventories = new List<Inventory>{
        InventoryFactory.CreateInventory("ordinary", char.Parse("#"), GetOrdinaryCount()),
        InventoryFactory.CreateInventory("boring", char.Parse("#"), 2),
        InventoryFactory.CreateInventory("magnet", char.Parse("#"), 2),
        InventoryFactory.CreateInventory("exploding", char.Parse("#"), 2)
      };
      winCondition = Math.Max(4, (int)(board.Rows * board.Columns * 0.1));
    }
  }

  class LineUpBasic : Game
  {
    [SetsRequiredMembers]
    public LineUpBasic(string playMode_)
    {
      history = new History();
      board = new Board(8, 9);
      playMode = playMode_;
      p1 = new HumanPlayer('@', "Player1");
      p2 = playMode == "HvC" ? new AIPlayer('#', "Computer") : new HumanPlayer('#', "Player2");
      currentPlayer = p1;
      GameRegistry.P1 = p1;
      GameRegistry.P2 = p2;
      p1.Inventories = new List<Inventory>{
        InventoryFactory.CreateInventory("ordinary", char.Parse("@"), GetOrdinaryCount()),
      };
      p2.Inventories = new List<Inventory>{
        InventoryFactory.CreateInventory("ordinary", char.Parse("#"), GetOrdinaryCount()),
      };
      winCondition = Math.Max(4, (int)(board.Rows * board.Columns * 0.1));
      ordinaryOnly = true;
    }

    protected override int GetOrdinaryCount()
    {
      return GetPerPlayer();
    }
  }

  class LineUpSpin : Game
  {
    [SetsRequiredMembers]
    public LineUpSpin(string playMode_)
    {
      history = new History();
      board = new Board(8, 9);
      playMode = playMode_;
      p1 = new HumanPlayer('@', "Player1");
      p2 = playMode == "HvC" ? new AIPlayer('#', "Computer") : new HumanPlayer('#', "Player2");
      currentPlayer = p1;
      GameRegistry.P1 = p1;
      GameRegistry.P2 = p2;
      p1.Inventories = new List<Inventory>{
        InventoryFactory.CreateInventory("ordinary", char.Parse("@"), GetOrdinaryCount()),
      };
      p2.Inventories = new List<Inventory>{
        InventoryFactory.CreateInventory("ordinary", char.Parse("#"), GetOrdinaryCount()),
      };
      winCondition = Math.Max(4, (int)(board.Rows * board.Columns * 0.1));
      ordinaryOnly = true;
      spinEnabled = true;
    }
  }
}
