public interface IRenderer<T>
{
  static abstract void Render(T obj);
}

public class MenuRenderer : IRenderer<Menu>
{
  public static void Render(Menu menu)
  {
    Console.WriteLine("Please select an option:");
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
