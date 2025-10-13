public class History
{
  private readonly Stack<Move> moves;
  private readonly Stack<Move> undoneMoves;

  public History()
  {
    moves = new Stack<Move>();
    undoneMoves = new Stack<Move>();
  }

  public void Add(Move move)
  {
    moves.Push(move);
  }

  public Move Undo()
  {
    if (moves.Count == 0)
    {
      throw new InvalidOperationException("No moves to undo.");
    }
    Move move = moves.Pop();
    undoneMoves.Push(move);
    return move;
  }

  public Move Redo()
  {
    if (undoneMoves.Count == 0)
    {
      throw new InvalidOperationException("No moves to redo.");
    }
    Move move = undoneMoves.Pop();
    moves.Push(move);
    return move;
  }
}
