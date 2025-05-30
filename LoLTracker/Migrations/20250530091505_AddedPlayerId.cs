using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoLTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddedPlayerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChampionId",
                table: "Participants",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GameVersion",
                table: "Matches",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChampionId",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "GameVersion",
                table: "Matches");
        }
    }
}
