public abstract class Disc
{
  public DiscType Type { get; set; } = DiscType.Ordinary;
  public string Symbol { get; set; } = "";
  public Position? Position { get; set; } = null;
  public EffectType Effect { get; set; } = EffectType.None;
}

public class OrdinaryDisc : Disc
{
  public OrdinaryDisc(string symbol) : base()
  {
    Symbol = symbol;
  }
}

public class BoringDisc : Disc
{
  public BoringDisc(string symbol) : base()
  {
    Type = DiscType.Boring;
    Symbol = symbol;
    Effect = EffectType.Boring;
  }
}

public class MagneticDisc : Disc
{
  public MagneticDisc(string symbol) : base()
  {
    Type = DiscType.Magnetic;
    Symbol = symbol;
    Effect = EffectType.Magnetic;
  }
}

public class ExplodingDisc : Disc
{
  public ExplodingDisc(string symbol) : base()
  {
    Type = DiscType.Exploding;
    Symbol = symbol;
    Effect = EffectType.Exploding;
  }
}
