using System.Diagnostics.CodeAnalysis;

namespace DibariBot.Database.Models;

#nullable disable
public class GuildConfig : DbModel
{
    public ulong GuildId { get; set; }
    public string Prefix { get; set; }
}