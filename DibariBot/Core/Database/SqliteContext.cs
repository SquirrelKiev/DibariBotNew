using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DibariBot.Database;

public class SqliteContext : BotContext
{
    public SqliteContext(string connStr = "Data Source=data/DibariBot.db") : base(connStr)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = new SqliteConnectionStringBuilder(connectionString);
        builder.DataSource = Path.Combine(AppContext.BaseDirectory, builder.DataSource);
        optionsBuilder.UseSqlite(builder.ToString());
    }
}
