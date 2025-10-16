namespace LineUpGame
{
  /*
  Renderer interface and implementations for rendering various objects to the console.
  Using generic type to allow flexibility in rendering different types.
*/
  public interface IRenderer<T>
  {
    static abstract void Render(T obj);
  }

  public class MenuRenderer : IRenderer<Menu>
  {
    public static void Render(Menu menu)
    {
      foreach (var item in menu.Items)
      {
        if (item.Key == "" && item.Description == "")
        {
          Console.WriteLine();
          continue;
        }
        if (item.Key == "")
        {
          Console.WriteLine(item.Description);
          continue;
        }
        Console.WriteLine($"{item.Key}: {item.Description}");
      }
    }
  }

  public class BoardRenderer : IRenderer<Board>
  {
    public static void Render(Board board)
    {
      Console.WriteLine();
      for (int r = 0; r < board.Rows; r++)
      {
        Console.Write("|");
        for (int c = 0; c < board.Columns; c++)
        {
          char ch = board.GetCell(r, c);
          Console.Write($" {(ch == ' ' ? ' ' : ch)} |");
        }
        Console.WriteLine();
      }
      Console.Write(" ");
      for (int c = 0; c < board.Columns; c++) Console.Write($" {c}  ");
      Console.WriteLine("\n");
    }
  }
}
