namespace DibariBot.Core.Database.Models;

public class RegexChannelEntry : DbModel
{
    public required uint RegexFilterId { get; set; }

    public required RegexFilter RegexFilter { get; set; } = null!;
}
