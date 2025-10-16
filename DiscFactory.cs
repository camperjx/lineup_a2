namespace LineUpGame
{
  /*
    A centralized factory to create Disc instances based on type and symbol.
  */
  public class DiscFactory
  {
    public static Disc CreateDisc(string type, char ownerOrdinary)
    {
      return type.ToLower() switch
      {
        "ordinary" => new OrdinaryDisc(ownerOrdinary),
        "boring" => new BoringDisc(ownerOrdinary),
        "magnet" => new MagnetDisc(ownerOrdinary),
        "exploding" => new ExplodingDisc(ownerOrdinary),
        _ => throw new ArgumentException($"Unknown disc type: {type}"),
      };
    }
  }
}
