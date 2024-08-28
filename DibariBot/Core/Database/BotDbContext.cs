using DibariBot.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DibariBot.Database
{
    public class BotDbContext(ILoggerFactory? loggerFactory) : DbContext
    {
        public DbSet<DefaultManga> DefaultMangas { get; set; }
        public DbSet<RegexFilter> RegexFilters { get; set; }
        public DbSet<RegexChannelEntry> RegexChannelEntries { get; set; }
        
        public DbSet<GuildConfig> GuildConfigs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(loggerFactory);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
