using Microsoft.EntityFrameworkCore;

namespace DibariBot.Database;

public class PostgresqlContext : BotDbContext
{
    public PostgresqlContext(string connStr = "Host=127.0.0.1;Username=postgres;Password=;Database=botdb") : base(connStr)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(connectionString);
    }
}