using System.Text.Json;

/*
  Strategy pattern is used to encapsulate different game initialization algorithms.
  This allows for easy extension and maintenance of the code.
*/
public interface IStrategy<T>
{
  public T InitGame();
}

public class SavedGameStrategy : IStrategy<Game>
{
  public Game InitGame()
  {
    string fullPath;
    while (true)
    {
      fullPath = Utilities.GetFullPath(shouldCreateDir: false);
      if (!File.Exists(fullPath))
      {
        Console.WriteLine("File not found.");
        continue;
      }
      else
      {
        string jsonStr = File.ReadAllText(fullPath);
        Game? savedGame = JsonSerializer.Deserialize<Game>(jsonStr);
        if (savedGame != null)
        {
          if (savedGame.isGameOver)
          {
            Console.WriteLine("Game is over. Choose another game.");
            continue;
          }
          else
          {
            return savedGame;
          }
        }
      }
    }
  }
}

public class NewGameStrategy : IStrategy<Game>
{
  public Game InitGame()
  {
    // Set board dimension
    string? input;
    Console.WriteLine("Enter board rows (min 6):");
    int rows, cols;
    while (true)
    {
      input = Console.ReadLine();
      rows = IntParser.Parse(input);
      if (rows < 6)
      {
        Console.WriteLine("Rows must be at least 6");
        continue;
      }
      else
      {
        break;
      }
    }
    while (true)
    {
      Console.WriteLine("Enter board columns (min 7):");
      input = Console.ReadLine();
      cols = IntParser.Parse(input);
      if (cols < 7)
      {
        Console.WriteLine("Columns must be at least 7");
        continue;
      }
      else
      {
        break;
      }
    }

    // Choose game mode
    string modeOption;
    Mode mode;
    while (true)
    {
      modeOption = Utilities.Selected(new GameModeMenu())[0];
      bool shouldBreak = false;
      switch (modeOption)
      {
        case "1":
          mode = Mode.HumanToHuman;
          shouldBreak = true;
          break;
        case "2":
          mode = Mode.HumanToAI;
          shouldBreak = true;
          break;
        case "3":
          return InitGame();
        case "4":
          Environment.Exit(0);
          return null;
        default:
          Console.WriteLine("Invalid option");
          return InitGame();
      }
      if (shouldBreak)
      {
        break;
      }
    }

    // Choose game type
    string typeOption;
    bool isStandard = false;
    while (true)
    {
      bool shouldBreak = false;
      typeOption = Utilities.Selected(new GameTypeMenu())[0];
      switch (typeOption)
      {
        case "1":
          isStandard = true;
          shouldBreak = true;
          break;
        case "2":
          shouldBreak = true;
          break;
        case "3":
          return InitGame();
        case "4":
          Environment.Exit(0);
          return null;
        default:
          Console.WriteLine("Invalid option");
          return InitGame();
      }
      if (shouldBreak)
      {
        break;
      }
    }

    Board board = new Board(rows, cols);
    Game game = new TwoPlayerGame(board, mode);
    game.Subscribe();

    // If standard mode, use all types of discs
    if (isStandard)
    {
      game.Players[0].Inventories = new List<DiscInventory>()
      {
        new OrdinaryDiscInventory("@"),
        new BoringDiscInventory("B"),
        new MagneticDiscInventory("M"),
        new ExplodingDiscInventory("E"),
      };
      game.Players[1].Inventories = new List<DiscInventory>()
      {
        new OrdinaryDiscInventory("#"),
        new BoringDiscInventory("b"),
        new MagneticDiscInventory("m"),
        new ExplodingDiscInventory("e"),
      };
    }
    // Otherwise, only use ordinary discs
    else
    {
      game.Players[0].Inventories = new List<DiscInventory>() { new OrdinaryDiscInventory("@") };
      game.Players[1].Inventories = new List<DiscInventory>() { new OrdinaryDiscInventory("#") };
    }
    // Assign discs to each player's inventories
    foreach (Player player in game.Players)
    {
      foreach (DiscInventory inventory in player.Inventories)
      {
        if (inventory is not OrdinaryDiscInventory)
        {
          for (int i = 0; i < 2; i++)
          {
            inventory.Add();
          }
        }
        else
        {
          for (int i = 0; i < Utilities.OrdinaryDiscCount(game); i++)
          {
            inventory.Add();
          }
        }
      }
    }

    return game;
  }
}
