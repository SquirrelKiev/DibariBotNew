using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DibariBot.Migrations
{
    /// <inheritdoc />
    public partial class PrefixCommands : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Prefix",
                table: "GuildConfig",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegexChannelEntries_ChannelId_RegexFilterId",
                table: "RegexChannelEntries",
                columns: new[] { "ChannelId", "RegexFilterId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RegexChannelEntries_ChannelId_RegexFilterId",
                table: "RegexChannelEntries");

            migrationBuilder.DropColumn(
                name: "Prefix",
                table: "GuildConfig");
        }
    }
}
