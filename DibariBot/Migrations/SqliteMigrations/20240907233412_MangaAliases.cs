using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DibariBot.Migrations.SqliteMigrations
{
    /// <inheritdoc />
    public partial class MangaAliases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MangaCommandAliases",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    SlashCommandName = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Manga = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MangaCommandAliases", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MangaCommandAliases_GuildId",
                table: "MangaCommandAliases",
                column: "GuildId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MangaCommandAliases");
        }
    }
}
