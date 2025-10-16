/*
  Using an immutable struct here to ensure that menu items cannot be changed after creation.
  Another reason for using a struct instead of an enum is that MenuItem is more complex than a simple value.
*/
public readonly struct MenuItem
{
  public string Key { get; }
  public string Description { get; }

  public MenuItem(string key, string description)
  {
    Key = key;
    Description = description;
  }
}

/*
  Menu only manages a list of MenuItems.
  This follows the single responsibility principle, making it easier to maintain and extend.
*/
public abstract class Menu
{
  public List<MenuItem> Items = new List<MenuItem>();
}

public class GameTypeMenu : Menu
{
  public GameTypeMenu()
  {
    Items.Add(new MenuItem("1", "LineUp Classic (with special discs)"));
    Items.Add(new MenuItem("2", "LineUp Basic (ordinary only)"));
    Items.Add(new MenuItem("3", "LineUp Spin (ordinary only + board rotates every 5 turns)"));
    Items.Add(new MenuItem("4", "Test Mode (run scripted sequence)"));
    Items.Add(new MenuItem("h", "Help"));
    Items.Add(new MenuItem("x", "Exit"));
  }
}

public class HelpMenu : Menu
{
  public HelpMenu()
  {
    Items.Add(new MenuItem("", "====== Help Menu ======"));
    Items.Add(new MenuItem("", ""));
    Items.Add(new MenuItem("", "Game Types:"));
    Items.Add(new MenuItem("1", "Classic: Full game with special discs"));
    Items.Add(new MenuItem("2", "Basic: Ordinary discs only"));
    Items.Add(new MenuItem("3", "Spin: Basic + board rotates every 5 turns"));
    Items.Add(new MenuItem("4", "Test: Run scripted sequence (e.g., O4,M5,B2)"));
    Items.Add(new MenuItem("", ""));
    Items.Add(new MenuItem("", "Disc Types:"));
    Items.Add(new MenuItem("o", "Ordinary Disc"));
    Items.Add(new MenuItem("b", "Boring Disc"));
    Items.Add(new MenuItem("m", "Magnet Disc"));
    Items.Add(new MenuItem("e", "Exploding Disc"));
    Items.Add(new MenuItem("", ""));
    Items.Add(new MenuItem("", "In-game commands:"));
    Items.Add(new MenuItem($"{"Default move (ordinary disc)",-32}", "<col> (e.g., 2)"));
    Items.Add(new MenuItem($"{"Standard move",-32}", "o/b/m/e<col> (e.g., b2)"));
    Items.Add(new MenuItem($"{"Save game",-32}", "save <filename>"));
    Items.Add(new MenuItem($"{"Load game",-32}", "load <filename>"));
    Items.Add(new MenuItem($"{"Undo",-32}", "undo"));
    Items.Add(new MenuItem($"{"Redo",-32}", "redo"));
    Items.Add(new MenuItem($"{"Help",-32}", "help"));
    Items.Add(new MenuItem($"{"Exit",-32}", "exit"));
  }
}
