using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DibariBot.Migrations.SqliteMigrations
{
    /// <inheritdoc />
    public partial class SqliteSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DefaultMangas",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Manga = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultMangas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuildConfig",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Prefix = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegexFilters",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Filter = table.Column<string>(type: "TEXT", nullable: false),
                    Template = table.Column<string>(type: "TEXT", nullable: false),
                    FilterType = table.Column<int>(type: "INTEGER", nullable: false),
                    ChannelFilterScope = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegexFilters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegexChannelEntries",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    RegexFilterId = table.Column<uint>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegexChannelEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegexChannelEntries_RegexFilters_RegexFilterId",
                        column: x => x.RegexFilterId,
                        principalTable: "RegexFilters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultMangas_GuildId_ChannelId",
                table: "DefaultMangas",
                columns: new[] { "GuildId", "ChannelId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GuildConfig_GuildId",
                table: "GuildConfig",
                column: "GuildId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegexChannelEntries_ChannelId_RegexFilterId",
                table: "RegexChannelEntries",
                columns: new[] { "ChannelId", "RegexFilterId" });

            migrationBuilder.CreateIndex(
                name: "IX_RegexChannelEntries_RegexFilterId",
                table: "RegexChannelEntries",
                column: "RegexFilterId");

            migrationBuilder.CreateIndex(
                name: "IX_RegexFilters_GuildId",
                table: "RegexFilters",
                column: "GuildId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DefaultMangas");

            migrationBuilder.DropTable(
                name: "GuildConfig");

            migrationBuilder.DropTable(
                name: "RegexChannelEntries");

            migrationBuilder.DropTable(
                name: "RegexFilters");
        }
    }
}
