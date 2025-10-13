using System.Diagnostics.CodeAnalysis;

public abstract class Game : IObserver<Disc>
{
  public required Board Board { get; set; }
  public List<Player> Players { get; set; }
  public Player CurrentPlayer { get; set; }
  public Mode Mode { get; set; } = Mode.HumanToHuman;
  public bool isGameOver { get; set; } = false;
  public virtual void Play(Disc disc, int col) { }
  public virtual void Play() { }
  protected IDisposable? _cancellation;
  public virtual int countToWin()
  {
    return (int)(Board.Rows * Board.Cols * 0.1);
  }

  public virtual void SwitchPlayer()
  {
    int currentIndex = Players.IndexOf(CurrentPlayer);
    int nextIndex;
    if (currentIndex == Players.Count - 1)
    {
      nextIndex = 0;
    }
    else
    {
      nextIndex = currentIndex + 1;
    }
    CurrentPlayer = Players[nextIndex];
  }
  protected abstract Player CreatePlayer(string playerType);
  public virtual void Subscribe()
  {
    _cancellation = Board.Subscribe(this);
  }
  public virtual void Unsubscribe()
  {
    _cancellation?.Dispose();
  }
  public abstract void OnNext(Disc value);
  public abstract void OnError(Exception error);
  public abstract void OnCompleted();
}

public class TwoPlayerGame : Game
{
  [SetsRequiredMembers]
  public TwoPlayerGame(Board board, Mode mode)
  {
    Board = board;
    Mode = mode;
    switch (Mode)
    {
      case Mode.HumanToHuman:
        Players = new List<Player>
        {
          CreatePlayer("Human"),
          CreatePlayer("Human")
        };
        break;
      case Mode.HumanToAI:
        Players = new List<Player>
        {
          CreatePlayer("Human"),
          CreatePlayer("AI")
        };
        break;
    }
    // TODO: Remove hardcoded names
    Players[0].Name = "Alice";
    Players[1].Name = "Bob";
    CurrentPlayer = Players![0];
    foreach (Player player in Players)
    {
      player.Subscribe(Board);
    }
  }
  public override void Play(Disc disc, int col)
  {
    if (CurrentPlayer is HumanPlayer)
    {
      Board.Add(disc, new Position(Board.AvailableRow(col)!.Value, col));
    }
  }

  public override void Play()
  {
    // Try every disc type for every column
    foreach (DiscType discType in Enum.GetValues(typeof(DiscType)))
    {
      if (CurrentPlayer.GetInventoryByType(discType)!.AvailableDiscs() == 0)
      {
        continue;
      }
      for (int c = 0; c < Board.Cols; c++)
      {
        if (Board.AvailableRow(c) == null)
        {
          continue;
        }
        Board simulatedBoard = Board.Clone();
        Disc d = Utilities.CreateDisc(discType, CurrentPlayer.GetInventoryByType(discType)!.Symbol);
        simulatedBoard.Add(d, new Position(simulatedBoard.AvailableRow(c)!.Value, c));
        // Handle special effects
        if (d.Effect != EffectType.None)
        {
          EffectParams effectParams = new EffectParams
          {
            Board = simulatedBoard,
            Disc = d,
            Player = CurrentPlayer
          };
          IEffect effect = EffectFactory.GetEffect(d.Effect);
          effect.ApplyEffect(effectParams);
        }
        if (Utilities.isWin(this, simulatedBoard, d))
        {
          Board.Add(d, new Position(Board.AvailableRow(c)!.Value, c));
          return;
        }
      }
    }
    // Otherwise pick a random disc and1 column
    Random rand = new Random();
    int col;
    Disc disc = null!;
    while (true)
    {
      col = rand.Next(0, Board.Cols);
      if (Board.AvailableRow(col) != null)
      {
        break;
      }
    }
    foreach (DiscInventory inventory in CurrentPlayer.Inventories)
    {
      if (inventory.AvailableDiscs() > 0)
      {
        disc = inventory.CreateDisc();
        break;
      }
    }
    // Finally make the move on real board
    Board.Add(disc, new Position(Board.AvailableRow(col)!.Value, col));
  }
  protected override Player CreatePlayer(string playerType)
  {
    switch (playerType.ToLower())
    {
      case "human":
        return new HumanPlayer([]);
      case "ai":
        return new AIPlayer([]);
      default:
        throw new ArgumentException("Invalid player type");
    }
  }
  public override void OnNext(Disc disc)
  {
    /*
      1. Check special effects
      2. Check game over condition
    */
    if (disc.Position == null)
    {
      return;
    }
    // Check special effects
    // TODO: Implement other effects
    if (disc.Effect != EffectType.None)
    {
      EffectParams effectParams = new EffectParams
      {
        Board = Board,
        Disc = disc,
        Player = CurrentPlayer
      };
      IEffect effect = EffectFactory.GetEffect(disc.Effect);
      effect.ApplyEffect(effectParams);
    }
    // Check for game over condition
    bool isDraw = Utilities.isDraw(Board);
    bool isWin = Utilities.isWin(this, Board, disc);
    if (isDraw)
    {
      Console.WriteLine("The game is a draw!");
      isGameOver = true;
    }
    if (isWin)
    {
      Console.WriteLine($"Player {CurrentPlayer.Name} wins!");
      isGameOver = true;
    }
  }

  public override void OnError(Exception error)
  {
    // Handle errors if necessary
  }

  public override void OnCompleted()
  {
    // Handle completion if necessary
  }
}
