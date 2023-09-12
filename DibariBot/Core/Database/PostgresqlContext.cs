using Microsoft.EntityFrameworkCore;

namespace DibariBot.Database;

public class PostgresqlContext : BotContext
{
    public PostgresqlContext(string connStr = "Host=127.0.0.1;Username=postgres;Password=;Database=dibari") : base(connStr)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(connectionString);
    }
}
