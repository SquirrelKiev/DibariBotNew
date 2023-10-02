namespace DibariBot.Core.Database.Models;

public class RegexChannelEntry : DbModel
{
    public required ulong ChannelId { get; set; }

    public uint RegexFilterId { get; set; }

    public RegexFilter RegexFilter { get; set; } = null!;
}
