namespace DibariBot.Core.Database.Models;

public enum FilterType
{
    AllowList,
    BlockList
}

public class RegexFilter : DbModel
{
    public required ulong GuildId { get; set; }

    public required string RegexString { get; set; }

    public ICollection<RegexChannelEntry> Channels { get; } = new List<RegexChannelEntry>();

    public required FilterType FilterType { get; set; }
}
