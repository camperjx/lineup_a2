public class BoardGame
{
  public static void Main()
  {
    // In the future, when more strategies are added, the dictionary can be extended into a separate class.
    Dictionary<string, IStrategy<Game>> Strategies = new Dictionary<string, IStrategy<Game>>()
    {
      { "1", new SavedGameStrategy() },
      { "2", new NewGameStrategy() },
    };

    string initOption = Utilities.Selected(new GameInitMenu())[0];

    Game game;

    if (initOption == "3")
    {
      return;
    }
    while (true)
    {
      try
      {
        game = Strategies[initOption].InitGame();
        break;
      }
      catch (KeyNotFoundException)
      {
        Console.WriteLine("Invalid option");
        continue;
      }
    }

    BoardRenderer.Render(game.Board);

    // Game loop
    while (true)
    {
      // GameStatsRenderer.Render(game);
      Console.WriteLine();
      Console.WriteLine($"====== Player {game.CurrentPlayer.Name}'s turn ======");

      if (game.CurrentPlayer is HumanPlayer)
      {
        MenuRenderer.Render(new PlayMenu());
        string? input = Console.ReadLine();
        switch (input)
        {
          case "1":
            Console.WriteLine("Move:");
            break;
          case "2":
            // TODO: Undo
            break;
          case "3":
            // TODO: Redo
            break;
          case "4":
            // TODO: Save game
            break;
          case "5":
            // TODO: Save and quit game
            return;
          case "6":
            return;
          default:
            Console.WriteLine("Invalid option.");
            continue;
        }
        // Player moves
        input = Console.ReadLine();
        string? symbol;
        int col;
        try
        {
          (symbol, col) = MoveParser.Parse(input);
        }
        catch (ArgumentException ex)
        {
          Console.WriteLine(ex.Message);
          continue;
        }
        if (symbol == null)
        {
          Console.WriteLine("Symbol cannot be empty.");
          continue;
        }
        if (!Utilities.IsValidSymbol(game.CurrentPlayer, symbol))
        {
          Console.WriteLine("Invalid symbol.");
          continue;
        }
        if (col < 0 || col >= game.Board.Cols)
        {
          Console.WriteLine("Invalid column");
          continue;
        }
        try
        {
          if (game.Board.AvailableRow(col) == null)
          {
            Console.WriteLine("Column is full");
            continue;
          }
          Disc disc = game.CurrentPlayer.GetInventoryBySymbol(symbol)!.CreateDisc();
          game.Play(disc, col);
        }
        catch (InvalidOperationException ex)
        {
          Console.WriteLine(ex.Message);
          if (ex.Message == "No discs available")
          {
            Console.WriteLine("You ran out of discs of this type. Choose another type.");
            continue;
          }
          continue;
        }
      }
      else if (game.CurrentPlayer is AIPlayer)
      {
        game.Play();
      }

      BoardRenderer.Render(game.Board);

      // Game over conditions are checked automatically after each move,
      // thanks to the Observer pattern.
      if (game.isGameOver)
      {
        break;
      }

      game.SwitchPlayer();
    }
  }
}
