namespace LineUpGame
{
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
