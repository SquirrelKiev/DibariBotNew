namespace DibariBot.Database.Models;

public class ChannelConfig : DbModel
{
    public ulong ChannelId { get; set; }
    public string DefaultManga { get; set; }
}

