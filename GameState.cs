public class GameState
{
  public string BoardData { get; set; } = "";
  public int Rows { get; set; }
  public int Cols { get; set; }
  public Dictionary<string, int> P1Inventory { get; set; } = new();
  public Dictionary<string, int> P2Inventory { get; set; } = new();
  public string CurrentPlayer { get; set; } = "P1";
  public int TurnCount { get; set; } = 0;
}
