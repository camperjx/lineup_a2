namespace LineUpGame
{
  /*
    A centralized factory for creating Inventory instances based on type, symbol, and count.
    It creates a single type of inventory at a time.
    This allows for easy extension if new inventory types are added in the future.
  */
  public class InventoryFactory
  {
    public static Inventory CreateInventory(string type, char symbol, int n)
    {
      return type.ToLower() switch
      {
        "ordinary" => new OrdinaryInventory(symbol, n),
        "boring" => new BoringInventory(symbol, n),
        "magnet" => new MagnetInventory(symbol, n),
        "exploding" => new ExplodingInventory(symbol, n),
        _ => throw new ArgumentException($"Unknown inventory type: {type}"),
      };
    }
  }
}
