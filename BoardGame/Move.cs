public readonly struct Move
{
  public Disc Disc { get; }
  public int Col { get; }

  public Move(Disc disc, int col)
  {
    Col = col;
    Disc = disc;
  }
}
