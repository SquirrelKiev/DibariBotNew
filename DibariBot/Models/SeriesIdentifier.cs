using System.Diagnostics.CodeAnalysis;

namespace DibariBot;

public struct SeriesIdentifier
{
    public string? platform = null;
    public string? series = null;

    public SeriesIdentifier() { }

    public SeriesIdentifier(string? platform, string? series) : this()
    {
        this.platform = platform;
        this.series = series;
    }

    [MemberNotNull(nameof(platform), nameof(series))]
    public readonly void ThrowIfInvalid()
    {
        ArgumentNullException.ThrowIfNull(platform, nameof(platform));
        ArgumentNullException.ThrowIfNull(series, nameof(series));
    }

    public readonly override string ToString()
    {
        return $"{platform}/{series}";
    }
}
