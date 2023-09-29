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

        if(useWordBoundary)
        {
            int lastSpaceIndex = subString.LastIndexOf(' ');
            if (lastSpaceIndex != -1)
            {
                subString = subString[..lastSpaceIndex].TrimEnd();
            }
        }

        return subString + '…';
    }

    public static IEnumerable<string> SplitToLines(this string input)
    {
        if (input == null)
        {
            yield break;
        }

        using StringReader reader = new(input);

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            yield return line;
        }
    }

    public static string StringOrDefault(this string potential, string def)
    {
        if (string.IsNullOrWhiteSpace(potential))
        {
            return def;
        }
        else
        {
            return potential;
        }
    }
}
