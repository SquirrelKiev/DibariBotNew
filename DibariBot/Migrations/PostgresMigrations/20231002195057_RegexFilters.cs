#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DibariBot.Migrations.PostgresMigrations
{
    /// <inheritdoc />
    public partial class RegexFilters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegexFilters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Filter = table.Column<string>(type: "text", nullable: false),
                    Template = table.Column<string>(type: "text", nullable: false),
                    FilterType = table.Column<int>(type: "integer", nullable: false),
                    ChannelFilterScope = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegexFilters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegexChannelEntries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChannelId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    RegexFilterId = table.Column<long>(type: "bigint", nullable: false)
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
                name: "RegexChannelEntries");

            migrationBuilder.DropTable(
                name: "RegexFilters");
        }
    }
}
