using System.ComponentModel.DataAnnotations;

namespace DibariBot.Database.Models;

// TODO: remove this nullable disable
#nullable disable
public class DefaultManga : DbModel
{
    public const int MaxMangaLength = 256;

    public required ulong GuildId { get; set; }
    public required ulong ChannelId { get; set; } = 0;
    [MaxLength(MaxMangaLength)]
    public string Manga { get; set; }
}
