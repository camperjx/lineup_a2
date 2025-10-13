/*
  Enum is a structured data type that makes it easier to read, extend, and maintain code.
*/
public enum EffectType
{
  None,
  Boring,
  Magnetic,
  Exploding,
}

public class EffectParams
{
    public Disc Disc { get; set; }
    public Board Board { get; set; }
    public Player Player { get; set; }
}

public interface IEffect
{
  void ApplyEffect(EffectParams effectParams);
}

public class BoringEffect : IEffect
{
  public  void ApplyEffect(EffectParams effectParams)
  {
    Disc disc = effectParams.Disc;
    Board board = effectParams.Board;
    int currentRow = disc.Position!.Row;
    int col = disc.Position!.Col;
    for (int row = currentRow + 1; row < board.Rows; row++)
    {
      if (board.Discs[row][col] != null)
      {
        board.Remove(new Position(row, col));
      }
      else
      {
        break;
      }
    }
    board.Relocate(disc, new Position(board.Rows - 1, col));
  }
}

public class MagneticEffect : IEffect
{
  public void ApplyEffect(EffectParams effectParams)
  {
    Disc disc = effectParams.Disc;
    Board board = effectParams.Board;
    Player player = effectParams.Player;

    int currentRow = disc.Position!.Row;
    // If less than 3 discs below, do nothing
    if (currentRow >= board.Rows - 2)
    {
      return;
    }

    int col = disc.Position!.Col;

    for (int row = currentRow + 2; row < board.Rows; row++)
    {
      if (board.Discs[row][col]!.Symbol == player.GetOrdinaryInventory()!.Symbol)
      {
        (board.Discs[row - 1][col], board.Discs[row][col]) = (board.Discs[row][col], board.Discs[row - 1][col]);
        break;
      }
    }

    for (int row = currentRow - 1; row >= 0; row--)
    {
      if (board.Discs[row][col] != null)
      {
        board.Remove(new Position(row, col));
      }
      else
      {
        break;
      }
    }
  }
}

public class ExplodingEffect : IEffect
{
  public void ApplyEffect(EffectParams effectParams) {}
}
