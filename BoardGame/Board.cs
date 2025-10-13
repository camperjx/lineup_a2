public class Board : IObservable<Disc>
{
  public int Rows { get; set; }
  public int Cols { get; set; }
  public Disc?[][] Discs { get; set; }
  private readonly HashSet<IObserver<Disc>> _observers = new();
  public Board(int rows, int cols)
  {
    Rows = rows;
    Cols = cols;
    Discs = new Disc[rows][];
    for (int i = 0; i < rows; i++)
    {
      Discs[i] = new Disc[cols];
    }
  }
  public int? AvailableRow(int col)
  {
    for (int row = Rows - 1; row >= 0; row--)
    {
      if (Discs[row][col] == null)
      {
        return row;
      }
    }
    return null;
  }

  public Board Clone()
  {
    var clonedDiscs = new Disc?[Rows][];
    for (int r = 0; r < Rows; r++)
    {
      clonedDiscs[r] = new Disc?[Cols];
      for (int c = 0; c < Cols; c++)
      {
        if (Discs[r][c] != null)
        {
          var d = Discs[r][c]!;
          var newDisc = Utilities.CreateDisc(d.Type, d.Symbol);
          clonedDiscs[r][c] = newDisc;
        }
      }
    }
    var clonedBoard = new Board(Rows, Cols);
    clonedBoard.Discs = clonedDiscs;
    return clonedBoard;
  }

  public IDisposable Subscribe(IObserver<Disc> observer)
  {
    _observers.Add(observer);
    return new Unsubscriber<Disc>(_observers, observer);
  }
  public void Add(Disc disc, Position position)
  {
    int row = position.Row;
    int col = position.Col;
    Discs[row][col] = disc;
    disc.Position = new Position(row, col);
    foreach (IObserver<Disc> observer in _observers)
    {
      observer.OnNext(disc);
    }
  }

  public void Remove(Position position)
  {
    int row = position.Row;
    int col = position.Col;
    if (row < 0 || row >= Rows || col < 0 || col >= Cols)
    {
      throw new ArgumentOutOfRangeException("Position is out of bounds");
    }
    if (Discs[row][col] == null)
    {
      throw new InvalidOperationException("No disc at the given position");
    }
    Disc disc = Discs[row][col]!;
    disc.Position = null;
    foreach (IObserver<Disc> observer in _observers)
    {
      observer.OnNext(disc);
    }
    Discs[row][col] = null;
  }

  public void Relocate(Disc disc, Position newPosition)
  {
    int newRow = newPosition.Row;
    int newCol = newPosition.Col;
    Discs[disc.Position!.Row][disc.Position!.Col] = null;
    Discs[newRow][newCol] = disc;
    disc.Position = newPosition;
  }

  public bool IsFull()
  {
    for (int col = 0; col < Cols; col++)
    {
      if (AvailableRow(col) != null)
      {
        return false;
      }
    }
    return true;
  }

  public int AlignedCountInDirection(Disc disc, int startRow, int startCol, int offsetRow, int offsetCol)
  {
    int count = 1;
    while (true)
    {
      startRow += offsetRow;
      startCol += offsetCol;
      if (startRow < 0 || startRow >= Rows || startCol < 0 || startCol >= Cols)
      {
        break;
      }
      if (Discs[startRow][startCol] == null || Discs[startRow][startCol]!.Symbol != disc.Symbol)
      {
        break;
      }
      count++;
    }
    return count;
  }
}

internal sealed class Unsubscriber<Disc> : IDisposable
{
  private readonly ISet<IObserver<Disc>> _observers;
  private readonly IObserver<Disc> _observer;

  internal Unsubscriber(ISet<IObserver<Disc>> observers, IObserver<Disc> observer)
  {
    _observers = observers;
    _observer = observer;
  }

  public void Dispose()
  {
    _observers.Remove(_observer);
  }
}
