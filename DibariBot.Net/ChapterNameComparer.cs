namespace DibariBot;

public class ChapterNameComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == null && y == null) return 0;

        // null to the start
        if (x == null) return -1;
        if (y == null) return 1;

        bool isXNumeric = float.TryParse(x, out float xValue);
        bool isYNumeric = float.TryParse(y, out float yValue);

        if (isXNumeric && isYNumeric)
        {
            return xValue.CompareTo(yValue);
        }
        else if (isXNumeric)
        {
            // x is a number, y is not
            return -1; // x comes before y
        }
        else if (isYNumeric)
        {
            // y is a number, x is not
            return 1; // y comes before x
        }
        else
        {
            return x.CompareTo(y);
        }
    }
}
