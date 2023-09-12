using DibariBot.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace DibariBot.Database;

public abstract class BotContext : DbContext
{
    public DbSet<GuildConfig> GuildConfig { get; set; }
    public DbSet<ChannelConfig> ChannelConfig { get; set; }

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

        modelBuilder.Entity<ChannelConfig>()
            .HasIndex(x => x.ChannelId)
            .IsUnique();
    }
}
