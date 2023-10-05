using Microsoft.EntityFrameworkCore;

namespace DibariBot.Database;

public class DbService
{
    private readonly BotConfig botConfig;

    public DbService(BotConfig botConfig)
    {
        this.botConfig = botConfig;
    }

    public async Task Initialize()
    {
        var args = Environment.GetCommandLineArgs();
        var migrationEnabled = !(args.Contains("nomigrate") || args.Contains("nukedb"));

        Log.Debug("Database migration: {migrationStatus}", migrationEnabled);

        var context = GetDbContext();

        if (context is SqliteContext)
        {
            await context.Database.ExecuteSqlRawAsync("PRAGMA journal_mode=WAL");
        }

        if (migrationEnabled)
        {
            await context.Database.MigrateAsync();
        }
    }

    public async Task ResetDatabase()
    {
        await using var dbContext = GetDbContext();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public BotContext GetDbContext()
    {
        BotContext context;

        switch (botConfig.Database)
        {
            case BotConfig.DatabaseType.Postgresql:
                context = new PostgresqlContext(botConfig.DatabaseConnectionString);
                break;
            case BotConfig.DatabaseType.Sqlite:
                context = new SqliteContext(botConfig.DatabaseConnectionString);
                break;
            default:
                throw new NotSupportedException(botConfig.Database.ToString());
        }

        return context;
    }
}
