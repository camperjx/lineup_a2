/*
  Parser interface and concrete parsers for different data types.
  Can be used in parsing moves and menu selections.
*/
public interface IParser<T>
{
  static bool TryParse(string input, out T? result)
  {
    throw new NotImplementedException();
  }
}

public class CharParser : IParser<char?>
{
  public static bool TryParse(string input, out char? result)
  {
    if (string.IsNullOrEmpty(input))
    {
      result = null;
      return false;
    }
    if (int.TryParse(input, out int _))
    {
      result = null;
      return false;
    }
    if (char.TryParse(input, out char value))
    {
      result = value;
      return true;
    }
    result = null;
    return false;
  }
}

public class IntParser : IParser<int?>
{
  public static bool TryParse(string input, out int? result)
  {
    if (string.IsNullOrEmpty(input))
    {
      result = null;
      return false;
    }
    if (int.TryParse(input, out int value))
    {
      result = value;
      return true;
    }
    result = null;
    return false;
  }
}
