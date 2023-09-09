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
    /// <exception cref="ArgumentNullException">if platform or series are null</exception>
    public readonly void ThrowIfInvalid()
    {
        ArgumentNullException.ThrowIfNull(platform, nameof(platform));
        ArgumentNullException.ThrowIfNull(series, nameof(series));
    }
}
