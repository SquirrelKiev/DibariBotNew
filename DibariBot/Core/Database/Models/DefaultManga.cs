using BotBase.Database;

namespace DibariBot.Database.Models;

#nullable disable
public class DefaultManga : DbModel
{
    public required ulong GuildId { get; set; }
    public required ulong ChannelId { get; set; } = 0;
    public string Manga { get; set; } // this should really be set as a value but i don't want efcore db gen to assume this is a not required field on the database side
}
