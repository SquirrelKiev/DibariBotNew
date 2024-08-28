using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DibariBot.Database;

public class PostgresContext(
    string connectionString = "Host=127.0.0.1;Username=postgres;Password=;Database=botdb",
    ILoggerFactory? loggerFactory = null
) : BotDbContext(loggerFactory)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseNpgsql(
            connectionString,
            x => x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
        );
    }
}
