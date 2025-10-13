/*
  Using an interface instead of abstract class to define a contract for parsers.
  Type parameter T allows for flexibility in return types.
*/
public interface IParser<T>
{
  static T Parse(string input) { throw new NotImplementedException(); }
}

/*
  A parser takes an input and returns a parsed value of type T.
  This follows the single responsibility principle, making it easier to maintain and extend.
*/
public class MoveParser : IParser<(string? symbol, int col)>
{
  public static (string? symbol, int col) Parse(string? input)
  {
    if (string.IsNullOrEmpty(input) || input.Length < 2)
    {
      throw new ArgumentException("Invalid input format. Expected format: <symbol><column>");
    }
    string? symbol = input[0].ToString();
    int col;
    if (!int.TryParse(input[1].ToString(), out col))
    {
      throw new ArgumentException("Invalid column number");
    }
    return (symbol, col);
  }
}

public class CommandParser : IParser<string[]>
{
  public static string[] Parse(string? input)
  {
    if (string.IsNullOrEmpty(input))
    {
      throw new ArgumentException("Input cannot be empty");
    }
    string[] parts = input.ToLower().Split(" ");
    return parts;
  }
}

public class IntParser : IParser<int>
{
  public static int Parse(string? input)
  {
    if (!int.TryParse(input, out int result))
    {
      throw new ArgumentException("Invalid integer input");
    }
    return result;
  }
}
