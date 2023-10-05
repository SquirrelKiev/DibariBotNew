namespace DibariBot;

public static class StringExtensions
{
    // replace with humanizer truncate?
    public static string Truncate(this string str, int limit, bool useWordBoundary = true)
    {
        if (str.Length <= limit)
        {
            return str;
        }

        var subString = str[..limit].Trim();

        if (!useWordBoundary) return subString + '…';

        int lastSpaceIndex = subString.LastIndexOf(' ');
        if (lastSpaceIndex != -1)
        {
            subString = subString[..lastSpaceIndex].TrimEnd();
        }

        return subString + '…';
    }

    public static IEnumerable<string> SplitToLines(this string? input)
    {
        if (input == null)
        {
            yield break;
        }

        using StringReader reader = new(input);

        while (reader.ReadLine() is { } line)
        {
            yield return line;
        }
    }

    public static string StringOrDefault(this string? potential, string def)
    {
        return string.IsNullOrWhiteSpace(potential) ? def : potential;
    }
}
