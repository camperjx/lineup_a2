namespace LineUpGame
{
  public static class GameRegistry
  {
    public static Player? P1;
    public static Player? P2;

    public static void ReturnToOwner(char discChar)
    {
      if (P1 == null || P2 == null) return;
      if (discChar == '@' || discChar == 'B' || discChar == 'M' || discChar == 'E')
        P1.ReturnFromChar(discChar);
      else if (discChar == '#' || discChar == 'b' || discChar == 'm' || discChar == 'e')
        P2.ReturnFromChar(discChar);
    }
  }
}
