using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DibariBot.Migrations.SqliteMigrations
{
    /// <inheritdoc />
    public partial class SwitchToBotBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable("GuildConfig", newName: "GuildPrefixPreferences");

            //migrationBuilder.DropTable(
            //    name: "GuildConfig");

            //migrationBuilder.CreateTable(
            //    name: "GuildPrefixPreferences",
            //    columns: table => new
            //    {
            //        Id = table.Column<uint>(type: "INTEGER", nullable: false)
            //            .Annotation("Sqlite:Autoincrement", true),
            //        GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
            //        Prefix = table.Column<string>(type: "TEXT", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_GuildPrefixPreferences", x => x.Id);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_GuildPrefixPreferences_GuildId",
            //    table: "GuildPrefixPreferences",
            //    column: "GuildId",
            //    unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable("GuildPrefixPreferences", newName: "GuildConfig");

            //migrationBuilder.DropTable(
            //    name: "GuildPrefixPreferences");

            //migrationBuilder.CreateTable(
            //    name: "GuildConfig",
            //    columns: table => new
            //    {
            //        Id = table.Column<uint>(type: "INTEGER", nullable: false)
            //            .Annotation("Sqlite:Autoincrement", true),
            //        GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
            //        Prefix = table.Column<string>(type: "TEXT", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_GuildConfig", x => x.Id);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_GuildConfig_GuildId",
            //    table: "GuildConfig",
            //    column: "GuildId",
            //    unique: true);
        }
    }
}
