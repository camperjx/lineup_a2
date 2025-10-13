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
  public bool isMultipleSelection = false;
}

public class GameInitMenu : Menu
{
  public GameInitMenu()
  {
    Items.Add(new MenuItem("1", "Open a saved game"));
    Items.Add(new MenuItem("2", "Start a new game"));
    Items.Add(new MenuItem("3", "Quit"));
  }
}

public class GameModeMenu : Menu
{
  public GameModeMenu()
  {
    Items.Add(new MenuItem("1", "Human vs Human"));
    Items.Add(new MenuItem("2", "Human vs AI"));
    Items.Add(new MenuItem("3", "Back to previous menu"));
    Items.Add(new MenuItem("4", "Quit"));
  }
}

public class GameTypeMenu : Menu
{
  public GameTypeMenu()
  {
    Items.Add(new MenuItem("1", "Standard (with special discs)"));
    Items.Add(new MenuItem("2", "Basic (no special discs)"));
    Items.Add(new MenuItem("3", "Back to previous menu"));
    Items.Add(new MenuItem("4", "Quit"));
  }
}

public class DiscMenu : Menu
{
  public DiscMenu()
  {
    isMultipleSelection = true;
    Items.Add(new MenuItem("1", "Boring Disc"));
    Items.Add(new MenuItem("2", "Magnetic Disc"));
    Items.Add(new MenuItem("3", "Exploding Disc"));
    Items.Add(new MenuItem("4", "Skip (no special discs)"));
    Items.Add(new MenuItem("5", "Back to previous menu"));
    Items.Add(new MenuItem("6", "Quit"));
  }
}

public class PlayMenu : Menu
{
  public PlayMenu()
  {
    Items.Add(new MenuItem("1", "Play a move"));
    Items.Add(new MenuItem("2", "Undo last move"));
    Items.Add(new MenuItem("3", "Redo last move"));
    Items.Add(new MenuItem("4", "Save game and continue"));
    Items.Add(new MenuItem("5", "Save game and quit"));
    Items.Add(new MenuItem("6", "Quit game without saving"));
  }
}
