using BotBase.Database;
using DibariBot.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace DibariBot.Database
{
    public class BotDbContext : BotDbContextBase
    {
        public DbSet<DefaultManga> DefaultMangas { get; set; }
        public DbSet<RegexFilter> RegexFilters { get; set; }
        public DbSet<RegexChannelEntry> RegexChannelEntries { get; set; }

        public BotDbContext(string connectionString) : base(connectionString)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GuildPrefixPreference>()
                .HasIndex(x => x.GuildId)
                .IsUnique();

            modelBuilder.Entity<DefaultManga>()
                .HasIndex(x => new { x.GuildId, x.ChannelId })
                .IsUnique();

            modelBuilder.Entity<RegexFilter>()
                .HasIndex(x => x.GuildId);

            modelBuilder.Entity<RegexChannelEntry>()
                .HasIndex(x => new { x.ChannelId, x.RegexFilterId });
        }
    }
}
