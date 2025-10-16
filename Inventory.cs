namespace LineUpGame
{
  public abstract class Inventory
  {
    protected string _type = "";
    protected char _symbol;
    public bool isSpecial { get; set; } = false;
    public List<Disc> Discs { get; set; } = new();

    public string Type
    {
      get => _type;
      protected set => _type = value;
    }
    public char Symbol
    {
      get => _symbol;
      protected set => _symbol = value;
    }
    public virtual void Clear()
    {
      Discs.Clear();
    }
    public virtual void Decrement()
    {
      if (Discs.Count > 0)
      {
        Discs.RemoveAt(Discs.Count - 1);
      }
    }
    public virtual void Increment(Disc disc)
    {
      Discs.Add(disc);
    }
    public int Count()
    {
      return Discs.Count;
    }
  }

  public class OrdinaryInventory : Inventory
  {
    public OrdinaryInventory(char symbol, int n)
    {
      _type = "ordinary";
      _symbol = symbol;
      for (int i = 0; i < n; i++)
      {
        Discs.Add(new OrdinaryDisc(symbol));
      }
    }
  }

  public class BoringInventory : Inventory
  {
    public BoringInventory(char symbol, int n)
    {
      _type = "boring";
      _symbol = symbol;
      isSpecial = true;
      for (int i = 0; i < n; i++)
      {
        Discs.Add(new BoringDisc(symbol));
      }
    }
  }

  public class MagnetInventory : Inventory
  {
    public MagnetInventory(char symbol, int n)
    {
      _type = "magnet";
      _symbol = symbol;
      isSpecial = true;
      for (int i = 0; i < n; i++)
      {
        Discs.Add(new MagnetDisc(symbol));
      }
    }
  }

  public class ExplodingInventory : Inventory
  {
    public ExplodingInventory(char symbol, int n)
    {
      _type = "exploding";
      _symbol = symbol;
      isSpecial = true;
      for (int i = 0; i < n; i++)
      {
        Discs.Add(new ExplodingDisc(symbol));
      }
    }
  }
}
