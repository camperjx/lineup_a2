public abstract class DiscInventory
{
  protected List<Disc> Discs = new List<Disc>();
  public DiscType Type { get; set; } = DiscType.Ordinary;
  public string Symbol = " ";

  public abstract Disc CreateDisc();
  public virtual void Add()
  {
    Discs.Add(CreateDisc());
  }
  public virtual Disc Remove()
  {
    if (Discs.Count == 0)
    {
      throw new InvalidOperationException("No discs available");
    }
    Disc disc = Discs[Discs.Count - 1];
    Discs.RemoveAt(Discs.Count - 1);
    return disc;
  }
  public virtual int AvailableDiscs()
  {
    return Discs.Count;
  }
}

public class OrdinaryDiscInventory : DiscInventory
{
  public OrdinaryDiscInventory(string symbol)
  {
    Type = DiscType.Ordinary;
    Symbol = symbol;
  }
  public override Disc CreateDisc()
  {
    return new OrdinaryDisc(Symbol);
  }
  public override string ToString()
  {
    return $"Type: Ordinary, Symbol: {Symbol}, Available: {AvailableDiscs()}";
  }
}

public class BoringDiscInventory : DiscInventory
{
  public BoringDiscInventory(string symbol)
  {
    Type = DiscType.Boring;
    Symbol = symbol;
  }
  public override Disc CreateDisc()
  {
    return new BoringDisc(Symbol);
  }
  public override string ToString()
  {
    return $"Type: Boring, Symbol: {Symbol}, Available: {AvailableDiscs()}";
  }
}

public class MagneticDiscInventory : DiscInventory
{
  public MagneticDiscInventory(string symbol)
  {
    Type = DiscType.Magnetic;
    Symbol = symbol;
  }
  public override Disc CreateDisc()
  {
    return new MagneticDisc(Symbol);
  }
  public override string ToString()
  {
    return $"Type: Magnetic, Symbol: {Symbol}, Available: {AvailableDiscs()}";
  }
}

public class ExplodingDiscInventory : DiscInventory
{
  public ExplodingDiscInventory(string symbol)
  {
    Type = DiscType.Exploding;
    Symbol = symbol;
  }
  public override Disc CreateDisc()
  {
    return new ExplodingDisc(Symbol);
  }
  public override string ToString()
  {
    return $"Type: Exploding, Symbol: {Symbol}, Available: {AvailableDiscs()}";
  }
}
