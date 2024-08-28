using System.Diagnostics.CodeAnalysis;

namespace DibariBot.Database.Models;

public enum FilterType
{
    Allow,
    Block
}

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

public class RegexFilter : DbModel
{
    public RegexFilter()
    {
    }

    [SetsRequiredMembers]
    public RegexFilter(uint id, ulong guildId, string filter, string template, FilterType filterType, ChannelFilterScope channelFilterScope, ICollection<RegexChannelEntry>? regexChannelEntries = null) : this()
    {
        Id = id;
        GuildId = guildId;
        Filter = filter;
        Template = template;
        FilterType = filterType;
        ChannelFilterScope = channelFilterScope;
        RegexChannelEntries = regexChannelEntries ?? new List<RegexChannelEntry>();
    }

    public required ulong GuildId { get; set; }

    public required string Filter { get; set; }
    public required string Template { get; set; }

    public required FilterType FilterType { get; set; }

    public required ChannelFilterScope ChannelFilterScope { get; set; }

    public ICollection<RegexChannelEntry> RegexChannelEntries { get; } = new List<RegexChannelEntry>();
}
