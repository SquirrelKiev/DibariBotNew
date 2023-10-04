using DibariBot.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace DibariBot.Database;

public abstract class BotContext : DbContext
{
    public DbSet<GuildConfig> GuildConfig { get; set; }
    public DbSet<DefaultManga> DefaultMangas { get; set; }
    public DbSet<RegexFilter> RegexFilters { get; set; }
    public DbSet<RegexChannelEntry> RegexChannelEntries { get; set; }

    protected readonly string connectionString;

    public BotContext(string connStr)
    {
        connectionString = connStr;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GuildConfig>()
            .HasIndex(x => x.GuildId)
            .IsUnique();

        modelBuilder.Entity<DefaultManga>()
            .HasIndex(x => new { x.GuildId, x.ChannelId })
            .IsUnique();

        modelBuilder.Entity<RegexFilter>()
            .HasIndex(x => x.GuildId);

        modelBuilder.Entity<RegexChannelEntry>()
            .HasIndex(x => new {x.ChannelId, x.RegexFilterId});
    }
}
