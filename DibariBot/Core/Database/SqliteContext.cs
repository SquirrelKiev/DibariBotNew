using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DibariBot.Database;

public class SqliteContext(string connectionString = "Data Source=data/botDb.db", ILoggerFactory? loggerFactory = null) : BotDbContext(loggerFactory)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var builder = new SqliteConnectionStringBuilder(connectionString);
        builder.DataSource = Path.Combine(AppContext.BaseDirectory, builder.DataSource);
        optionsBuilder.UseSqlite(builder.ToString(), x => x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
    }
}
