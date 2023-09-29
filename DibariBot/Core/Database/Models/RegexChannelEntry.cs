namespace DibariBot.Core.Database.Models;

// TODO: better name
public enum ChannelFilterScope
{
    /// <summary>
    /// Channels specified will be subject to this regex. (regex will only apply to these channels)
    /// </summary>
    Include,
    /// <summary>
    /// Channels specified will not be subject to this regex. (regex will apply to every channel except these channels)
    /// </summary>
    Exclude
}

public class RegexChannelEntry : DbModel
{
    public required ChannelFilterScope ChannelFilterScope { get; set; }
    
    public required ulong ChannelId { get; set; }

    public required uint RegexFilterId { get; set; }

    public required RegexFilter RegexFilter { get; set; } = null!;
}
