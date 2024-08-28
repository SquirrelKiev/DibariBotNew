using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DibariBot.Migrations.PostgresMigrations
{
    /// <inheritdoc />
    public partial class MigrateFromBotBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "GuildPrefixPreferences",
                newName: "GuildConfigs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GuildPrefixPreferences",
                table: "GuildConfigs");

            migrationBuilder.DropIndex(
                name: "IX_GuildPrefixPreferences_GuildId",
                table: "GuildConfigs");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "GuildConfigs");

            migrationBuilder.AlterColumn<decimal>(
                name: "GuildId",
                table: "GuildConfigs",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GuildConfigs",
                table: "GuildConfigs",
                column: "GuildId");

            migrationBuilder.AlterColumn<string>(
                name: "Prefix",
                table: "GuildConfigs",
                type: "character varying(8)",
                maxLength: 8,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "GuildConfigs",
                newName: "GuildPrefixPreferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GuildConfigs",
                table: "GuildPrefixPreferences");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "GuildPrefixPreferences",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<decimal>(
                name: "GuildId",
                table: "GuildPrefixPreferences",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");

            migrationBuilder.AlterColumn<string>(
                name: "Prefix",
                table: "GuildPrefixPreferences",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(8)",
                oldMaxLength: 8,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GuildPrefixPreferences",
                table: "GuildPrefixPreferences",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GuildPrefixPreferences_GuildId",
                table: "GuildPrefixPreferences",
                column: "GuildId",
                unique: true);
        }
    }
}