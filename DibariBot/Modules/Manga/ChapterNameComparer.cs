namespace DibariBot.Modules.Manga;

public class ChapterNameComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        switch (x)
        {
            // null go to the start
            case null when y == null:
                return 0;
            case null:
                return -1;
        }

        if (y == null) return 1;

        var isXNumeric = float.TryParse(x, out var xValue);
        var isYNumeric = float.TryParse(y, out var yValue);

        return isXNumeric switch
        {
            // x and y numeric
            true when isYNumeric => xValue.CompareTo(yValue),
            true =>
                // x is a number, y is not
                -1 // x comes before y
            ,
            false => isYNumeric
                ?
                // y is a number, x is not
                1
                : // y comes before x
                string.Compare(x, y, StringComparison.Ordinal)
        };
    }
}
