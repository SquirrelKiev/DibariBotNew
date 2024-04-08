using BotBase;
using BotBase.Database;

namespace DibariBot.Database
{
    public class DbService : DbServiceBase<BotDbContext>
    {
        public DbService(BotConfigBase botConfig) : base(botConfig)
        {
        }

        public override BotDbContext GetDbContext()
        {
            BotDbContext context;

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
}
