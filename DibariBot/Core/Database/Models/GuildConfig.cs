using System.ComponentModel.DataAnnotations;

namespace DibariBot.Database;

public class GuildConfig
{
    public const int MaxPrefixLength = 8;
    public const string DefaultPrefix = "m.";

    [Key]
    public ulong GuildId { get; set; }

    [MaxLength(MaxPrefixLength)]
    public string Prefix { get; set; } = DefaultPrefix;

    public int? EmbedColor { get; set; }
}
