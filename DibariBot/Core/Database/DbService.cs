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

        if (migrationEnabled) 
        {
            await GetDbContext().Database.MigrateAsync();
        }
    }

    public async Task ResetDatabase()
    {
        using var dbContext = GetDbContext();

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
            default:
                throw new NotSupportedException(botConfig.Database.ToString());
        }

        return context;
    }
}
