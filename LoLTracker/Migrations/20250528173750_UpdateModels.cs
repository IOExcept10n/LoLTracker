using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoLTracker.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BaronKills",
                table: "Participants",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChampExperience",
                table: "Participants",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DamageSelfMitigated",
                table: "Participants",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DragonKills",
                table: "Participants",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimePlayed",
                table: "Participants",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalDamageDealtToChampions",
                table: "Participants",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalHeal",
                table: "Participants",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalMinionsKilled",
                table: "Participants",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TurretTakedowns",
                table: "Participants",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VisionScore",
                table: "Participants",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WardsKilled",
                table: "Participants",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WardsPlaced",
                table: "Participants",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaronKills",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "ChampExperience",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "DamageSelfMitigated",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "DragonKills",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "TimePlayed",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "TotalDamageDealtToChampions",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "TotalHeal",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "TotalMinionsKilled",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "TurretTakedowns",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "VisionScore",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "WardsKilled",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "WardsPlaced",
                table: "Participants");
        }
    }
}
