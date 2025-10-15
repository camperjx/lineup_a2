public class History
{
  public Stack<GameState> undoStack { get; set; } = new();
  public Stack<GameState> redoStack { get; set; } = new();
  public History() { }
  public void Add(GameState state)
  {
    undoStack.Push(state);
    redoStack.Clear();
  }
  public GameState? Undo(GameState current)
  {
    if (undoStack.Count == 0)
    {
      return null;
    }
    var state = undoStack.Pop();
    redoStack.Push(current);
    return state;
  }
  public GameState? Redo(GameState current)
  {
    if (redoStack.Count == 0)
    {
      return null;
    }
    var state = redoStack.Pop();
    undoStack.Push(current);
    return state;
  }
}
