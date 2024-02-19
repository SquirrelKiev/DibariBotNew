using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DibariBot.Migrations.PostgresMigrations
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
            //        Id = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
            //        Prefix = table.Column<string>(type: "text", nullable: true)
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
            //        Id = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
            //        Prefix = table.Column<string>(type: "text", nullable: true)
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
