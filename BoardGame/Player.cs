/*
  Abstract Player class implements the Observer pattern to observe Disc placements on the Board.
*/
public abstract class Player : IObserver<Disc>
{
  // Each player has multiple inventories for different disc types
  public List<DiscInventory> Inventories { get; set; }
  public string Name { get; set; } = "";

  // ====== Observer pattern implementation begins =======
  protected IDisposable? _cancellation;
  public virtual void Subscribe(Board board)
  {
    _cancellation = board.Subscribe(this);
  }
  public virtual void Unsubscribe()
  {
    _cancellation?.Dispose();
  }
  public abstract void OnNext(Disc value);
  public abstract void OnError(Exception error);
  public abstract void OnCompleted();
  // ====== Observer pattern implementation ends =======

  public virtual DiscInventory? GetInventoryBySymbol(string symbol)
  {
    foreach (var inventory in Inventories)
    {
      if (inventory.Symbol == symbol)
      {
        return inventory;
      }
    }
    return null;
  }
  public virtual DiscInventory? GetOrdinaryInventory()
  {
    foreach (var inventory in Inventories)
    {
      if (inventory is OrdinaryDiscInventory)
      {
        return inventory;
      }
    }
    return null;
  }
  public virtual DiscInventory? GetInventoryByType(DiscType type)
  {
    foreach (var inventory in Inventories)
    {
      if (inventory.Type == type)
      {
        return inventory;
      }
    }
    return null;
  }
  public virtual List<DiscInventory> GetSpecialInventories()
  {
    List<DiscInventory> specialInventories = new List<DiscInventory>();
    foreach (var inventory in Inventories)
    {
      if (inventory is not OrdinaryDiscInventory)
      {
        specialInventories.Add(inventory);
      }
    }
    return specialInventories;
  }
}

public class HumanPlayer : Player
{
  public HumanPlayer(List<DiscInventory> inventories)
  {
    Inventories = inventories;
  }
  public override void OnNext(Disc disc)
  {
    var inventory = GetInventoryBySymbol(disc.Symbol);
    // If no matching inventory, skip
    if (inventory == null)
    {
      return;
    }
    // If disc is placed on board, remove from inventory
    if (disc.Position != null)
    {
      inventory.Remove();
    }
    // If returned from board, add to ORDINARY inventory
    else
    {
      var ordinaryInventory = GetOrdinaryInventory();
      ordinaryInventory?.Add();
    }
  }
  public override void OnError(Exception error)
  {
    // Skip for now
  }
  public override void OnCompleted()
  {
    // Skip for now
  }
}

public class AIPlayer : Player
{
  public AIPlayer(List<DiscInventory> inventories)
  {
    Name = "AI";
    Inventories = inventories;
  }
  public override void OnNext(Disc disc)
  {
    foreach (var inventory in Inventories)
    {
      if (inventory.Symbol == disc.Symbol && disc.Position != null)
      {
        inventory.Remove();
        break;
      }
      else
      {
        inventory.Add();
        break;
      }
    }
  }
  public override void OnError(Exception error)
  {
    // Skip for now
  }
  public override void OnCompleted()
  {
    // Skip for now
  }
}
