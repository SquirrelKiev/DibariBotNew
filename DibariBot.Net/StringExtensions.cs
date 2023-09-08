using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DibariBot;

public static class StringExtensions
{
    public static string Truncate(this string str, int limit, bool useWordBoundary)
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

        using StringReader reader = new StringReader(input);

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            yield return line;
        }
    }
}
