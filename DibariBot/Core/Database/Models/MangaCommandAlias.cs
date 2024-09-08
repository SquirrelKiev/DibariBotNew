using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DibariBot.Database.Models;

public class MangaCommandAlias : DbModel
{
    public const int MaxSlashCommandNameLength = 32;

    public ulong GuildId { get; set; }

    [MaxLength(MaxSlashCommandNameLength)]
    public required string SlashCommandName { get; set; }

    [MaxLength(DefaultManga.MaxMangaLength)]
    public required string Manga { get; set; }
}
