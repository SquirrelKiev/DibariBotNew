using DibariBot.Database;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DibariBot.Migrations.PostgresMigrations
{
    /// <inheritdoc />
    public partial class EmbedColorConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Prefix",
                table: "GuildConfigs",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: GuildConfig.DefaultPrefix,
                oldClrType: typeof(string),
                oldType: "character varying(8)",
                oldMaxLength: 8,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmbedColor",
                table: "GuildConfigs",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmbedColor",
                table: "GuildConfigs");

            migrationBuilder.AlterColumn<string>(
                name: "Prefix",
                table: "GuildConfigs",
                type: "character varying(8)",
                maxLength: 8,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(8)",
                oldMaxLength: 8);
        }
    }
}
