using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DibariBot.Database
{
    public class DbService(BotConfig botConfig, ILoggerFactory loggerFactory, ILogger<DbService> logger)
    {
        public async Task Initialize(bool migrationEnabled)
        {
            logger.LogDebug("Database migration: {migrationStatus}", migrationEnabled);

            var context = GetDbContext();

            if (migrationEnabled)
            {
                await context.Database.MigrateAsync();
            }
        }

        public BotDbContext GetDbContext()
        {
            BotDbContext context = botConfig.Database switch
            {
                BotConfig.DatabaseType.Postgresql => new PostgresContext(botConfig.DatabaseConnectionString, loggerFactory),
                BotConfig.DatabaseType.Sqlite => new SqliteContext(botConfig.DatabaseConnectionString, loggerFactory),
                _ => throw new NotSupportedException(botConfig.Database.ToString())
            };

            return context;
        }

        public async Task ResetDatabase()
        {
            await using var dbContext = GetDbContext();

            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
        }
    }
}
