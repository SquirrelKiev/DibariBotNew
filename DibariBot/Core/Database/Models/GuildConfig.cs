﻿namespace DibariBot.Database.Models;

public class GuildConfig : DbModel
{
    public ulong GuildId { get; set; }
    public string DefaultManga { get; set; }
}
