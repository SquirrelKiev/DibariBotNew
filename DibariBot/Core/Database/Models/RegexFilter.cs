namespace DibariBot.Core.Database.Models;

public enum FilterType
{
    Allow,
    Block
}

public class RegexFilter : DbModel
{
    public required ulong GuildId { get; set; }

    public required string Regex { get; set; }
    public required string Template { get; set; }

    public required FilterType FilterType { get; set; }

    public ICollection<RegexChannelEntry> RegexChannelEntries { get; } = new List<RegexChannelEntry>();
}
