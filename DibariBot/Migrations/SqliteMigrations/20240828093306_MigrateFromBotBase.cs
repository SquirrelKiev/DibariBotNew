using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DibariBot.Migrations.SqliteMigrations
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

            migrationBuilder.AlterColumn<ulong>(
                name: "GuildId",
                table: "GuildConfigs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "INTEGER");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GuildConfigs",
                table: "GuildConfigs",
                column: "GuildId");

            migrationBuilder.AlterColumn<string>(
                name: "Prefix",
                table: "GuildConfigs",
                type: "TEXT",
                maxLength: 8,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
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

            migrationBuilder.AddColumn<uint>(
                name: "Id",
                table: "GuildPrefixPreferences",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<ulong>(
                name: "GuildId",
                table: "GuildPrefixPreferences",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Prefix",
                table: "GuildPrefixPreferences",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
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