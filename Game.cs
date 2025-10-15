using System;
using System.Collections.Generic;

namespace LineUpGame
{
  abstract class Game
  {
    private History history = new();

    protected Board board = null!;
    protected Player p1 = null!;
    protected Player p2 = null!;
    protected Player current = null!;
    protected int winCondition;
    protected string playMode = "HvH";
    protected int turnCount = 0;

    // ═══════════════════════════════════════════════════════════
    // TEMPLATE METHOD - Defines the algorithm structure
    // ═══════════════════════════════════════════════════════════
    public void Run()
    {
      SetupPlayers();
      ConfigureBoard();
      ConfigureInventory();
      ConfigureRules();
      TurnLoop(UseOnlyOrdinary(), EnableSpin());
    }

    // ═══════════════════════════════════════════════════════════
    // TEMPLATE HOOKS - To be implemented by subclasses
    // ═══════════════════════════════════════════════════════════
    protected abstract void ConfigureBoard();
    protected abstract void ConfigureInventory();
    protected abstract void ConfigureRules();
    protected abstract bool UseOnlyOrdinary();
    protected abstract bool EnableSpin();

    // ═══════════════════════════════════════════════════════════
    // COMMON METHODS - Shared by all game modes
    // ═══════════════════════════════════════════════════════════

    private void SaveState()
    {
      history.Add(CaptureCurrentState());
    }

    private GameState CaptureCurrentState()
    {
      return new GameState
      {
        BoardData = board.Serialize(),
        P1Inventory = new Dictionary<string, int>(p1.Inventory),
        P2Inventory = new Dictionary<string, int>(p2.Inventory),
        CurrentPlayer = current == p1 ? "P1" : "P2",
        TurnCount = turnCount
      };
    }

    private void RestoreState(GameState st)
    {
      board.Deserialize(st.BoardData);
      p1.Inventory.Clear(); foreach (var kv in st.P1Inventory) p1.Inventory[kv.Key] = kv.Value;
      p2.Inventory.Clear(); foreach (var kv in st.P2Inventory) p2.Inventory[kv.Key] = kv.Value;
      current = (st.CurrentPlayer == "P1") ? p1 : p2;
      turnCount = st.TurnCount;
    }

    protected void SetupPlayers()
    {
      Console.WriteLine("Select play mode:");
      Console.WriteLine("  1. Human vs Human");
      Console.WriteLine("  2. Human vs Computer");
      Console.Write("Enter option (1-2): ");
      string? opt = Console.ReadLine()?.Trim();
      playMode = opt == "2" ? "HvC" : "HvH";

      p1 = new HumanPlayer('@', "Player1");
      p2 = playMode == "HvC" ? new AIPlayer('#', "Computer") : new HumanPlayer('#', "Player2");

      p1.ResetDefaultInventory();
      p2.ResetDefaultInventory();
      current = p1;
      GameRegistry.P1 = p1;
      GameRegistry.P2 = p2;
    }

    protected bool ExecuteHumanCommand(string input, bool onlyOrdinary)
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
          Console.WriteLine("Invalid column.");
          return false;
        }
        string type = symbol switch
        {
          'O' or 'o' => "ordinary",
          'B' or 'b' => "boring",
          'M' or 'm' => "magnet",
          'E' or 'e' => "explode",
          _ => ""
        };
        if (type == "")
        {
          Console.WriteLine("Invalid disc type.");
          return false;
        }
        if (onlyOrdinary)
        {
          type = "ordinary";
        }
        if (!current.HasDisc(type))
        {
          Console.WriteLine($"No {type} discs remaining.");
          return false;
        }
        Disc disc = current.CreateDisc(type);
        SaveState();
        if (board.DropDisc(col!.Value, disc))
        {
          current.Consume(type);
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
        if (!current.HasDisc(type))
        {
          Console.WriteLine("No ordinary discs.");
          return false;
        }
        Disc disc = current.CreateDisc(type);
        SaveState();
        if (board.DropDisc(col!.Value, disc))
        {
          current.Consume(type);
          return true;
        }
        return false;
      }
    }

    private char GetOpponentSymbol(Player current)
    {
      return current == p1 ? p2.Symbol : p1.Symbol;
    }

    protected void TurnLoop(bool onlyOrdinary, bool doSpin = false)
    {
      turnCount = 0;
      while (!board.IsFull())
      {
        board.Display();
        Console.WriteLine($"Win if you connect {winCondition}.");
        Console.WriteLine($"Inv P1(O/B/M/E): {p1.Inventory["ordinary"]}/{p1.Inventory["boring"]}/{p1.Inventory["magnet"]}/{p1.Inventory["explode"]} | " +
                          $"P2: {p2.Inventory["ordinary"]}/{p2.Inventory["boring"]}/{p2.Inventory["magnet"]}/{p2.Inventory["explode"]}");

        // Check if current player is AI
        if (current is AIPlayer ai)
        {
          char[,] grid = board.ExportGrid();
          int winN = winCondition;
          char opponent = GetOpponentSymbol(current);

          int col = ai.ChooseColumn(grid, winN, opponent);
          if (col >= 0 && board.DropDisc(col, ai.CreateDisc("ordinary")))
          {
            ai.Consume("ordinary");
          }
        }
        else
        {
          // Human player - prompt for input
          Console.Write($"{current.Name} ({current.Symbol}): enter command: ");
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
          bool moved = ExecuteHumanCommand(line, onlyOrdinary);
          if (!moved) continue;
        }

        // Check for win after move
        if (board.CheckWin(current.Symbol, winCondition))
        {
          board.Display();
          Console.WriteLine($"{current.Name} wins!");
          return;
        }

        // Handle board rotation for Spin mode
        turnCount++;
        if (doSpin && turnCount % 5 == 0)
        {
          Console.WriteLine(">>> Board rotates 90° clockwise and gravity reapplies!");
          board.RotateClockwise();
          board.ApplyGravity();
        }

        // Switch to other player
        current = (current == p1) ? p2 : p1;
      }

      board.Display();
      Console.WriteLine("Board full. It's a tie.");
    }

    public void Save(string filename)
    {
      using var w = new System.IO.StreamWriter(filename);
      w.WriteLine(board.Rows);
      w.WriteLine(board.Columns);
      w.WriteLine(winCondition);
      w.WriteLine(current == p1 ? "P1" : "P2");
      w.WriteLine(playMode);
      w.WriteLine(this.GetType().Name);
      w.WriteLine($"{p1.Inventory["ordinary"]},{p1.Inventory["boring"]},{p1.Inventory["magnet"]},{p1.Inventory["explode"]}");
      w.WriteLine($"{p2.Inventory["ordinary"]},{p2.Inventory["boring"]},{p2.Inventory["magnet"]},{p2.Inventory["explode"]}");
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
      p1.ResetDefaultInventory();
      p2.ResetDefaultInventory();
      p1.Inventory["ordinary"] = int.Parse(p1Inv[0]);
      p1.Inventory["boring"] = int.Parse(p1Inv[1]);
      p1.Inventory["magnet"] = int.Parse(p1Inv[2]);
      p1.Inventory["explode"] = int.Parse(p1Inv[3]);
      p2.Inventory["ordinary"] = int.Parse(p2Inv[0]);
      p2.Inventory["boring"] = int.Parse(p2Inv[1]);
      p2.Inventory["magnet"] = int.Parse(p2Inv[2]);
      p2.Inventory["explode"] = int.Parse(p2Inv[3]);

      GameRegistry.P1 = p1;
      GameRegistry.P2 = p2;
      current = curStr == "P1" ? p1 : p2;

      board = new Board(rows, cols);
      for (int r = 0; r < rows; r++)
      {
        string line = lines[idx++];
        for (int c = 0; c < cols && c < line.Length; c++) board.SetCell(r, c, line[c]);
      }
      Console.WriteLine($"Loaded: {gameType} {rows}x{cols} {playMode}, next={current.Name}");
      return true;
    }
  }

  // ═══════════════════════════════════════════════════════════
  // CONCRETE GAME IMPLEMENTATIONS
  // ═══════════════════════════════════════════════════════════

  class LineUpClassic : Game
  {
    private int rows;
    private int cols;

    protected override void ConfigureBoard()
    {
      Console.Write("Rows (>=6): ");
      rows = int.Parse(Console.ReadLine()!);
      Console.Write("Cols (>=7, cols>=rows): ");
      cols = int.Parse(Console.ReadLine()!);
      board = new Board(rows, cols);
    }

    protected override void ConfigureInventory()
    {
      int totalCells = rows * cols;
      int perPlayer = totalCells / 2;
      int ordinary = Math.Max(0, perPlayer - 6);
      p1.Inventory["ordinary"] = ordinary;
      p2.Inventory["ordinary"] = ordinary;
      // Special discs (boring, magnet, explode) already set to 2 by ResetDefaultInventory()
    }

    protected override void ConfigureRules()
    {
      winCondition = Math.Max(4, (int)Math.Floor(rows * cols * 0.1));
    }

    protected override bool UseOnlyOrdinary() => false;
    protected override bool EnableSpin() => false;
  }

  class LineUpBasic : Game
  {
    private const int ROWS = 8;
    private const int COLS = 9;

    protected override void ConfigureBoard()
    {
      board = new Board(ROWS, COLS);
    }

    protected override void ConfigureInventory()
    {
      int totalCells = ROWS * COLS;
      int perPlayer = totalCells / 2;
      p1.Inventory["ordinary"] = perPlayer;
      p2.Inventory["ordinary"] = perPlayer;

      // Disable special discs
      p1.Inventory["boring"] = 0;
      p1.Inventory["magnet"] = 0;
      p1.Inventory["explode"] = 0;
      p2.Inventory["boring"] = 0;
      p2.Inventory["magnet"] = 0;
      p2.Inventory["explode"] = 0;
    }

    protected override void ConfigureRules()
    {
      winCondition = Math.Max(4, (int)Math.Floor(ROWS * COLS * 0.1));
    }

    protected override bool UseOnlyOrdinary() => true;
    protected override bool EnableSpin() => false;
  }

  class LineUpSpin : Game
  {
    private const int ROWS = 8;
    private const int COLS = 9;

    protected override void ConfigureBoard()
    {
      board = new Board(ROWS, COLS);
    }

    protected override void ConfigureInventory()
    {
      int totalCells = ROWS * COLS;
      int perPlayer = totalCells / 2;
      p1.Inventory["ordinary"] = perPlayer;
      p2.Inventory["ordinary"] = perPlayer;

      // Disable special discs
      p1.Inventory["boring"] = 0;
      p1.Inventory["magnet"] = 0;
      p1.Inventory["explode"] = 0;
      p2.Inventory["boring"] = 0;
      p2.Inventory["magnet"] = 0;
      p2.Inventory["explode"] = 0;
    }

    protected override void ConfigureRules()
    {
      winCondition = Math.Max(4, (int)Math.Floor(ROWS * COLS * 0.1));
    }

    protected override bool UseOnlyOrdinary() => true;
    protected override bool EnableSpin() => true;  // Key difference - enables rotation!
  }
}
