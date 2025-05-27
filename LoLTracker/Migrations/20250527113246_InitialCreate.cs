using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoLTracker.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Puuid = table.Column<string>(type: "TEXT", nullable: false),
                    RiotId = table.Column<string>(type: "TEXT", nullable: false),
                    LeagueAccountId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Puuid);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    GameId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameMode = table.Column<string>(type: "TEXT", nullable: false),
                    GameResult = table.Column<string>(type: "TEXT", nullable: false),
                    GameDuration = table.Column<long>(type: "INTEGER", nullable: false),
                    GameStartTimestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GameEndTimestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.GameId);
                });

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MatchId = table.Column<long>(type: "INTEGER", nullable: false),
                    ChampionName = table.Column<string>(type: "TEXT", nullable: false),
                    Puuid = table.Column<string>(type: "TEXT", nullable: false),
                    Kills = table.Column<int>(type: "INTEGER", nullable: false),
                    Assists = table.Column<int>(type: "INTEGER", nullable: false),
                    Deaths = table.Column<int>(type: "INTEGER", nullable: false),
                    GoldEarned = table.Column<int>(type: "INTEGER", nullable: false),
                    DamageDealtToBuildings = table.Column<int>(type: "INTEGER", nullable: false),
                    DamageDealtToChampions = table.Column<int>(type: "INTEGER", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    DetectorWardsPlaced = table.Column<int>(type: "INTEGER", nullable: false),
                    StealthWardsPlaced = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalWardsPlaced = table.Column<int>(type: "INTEGER", nullable: false),
                    MagicDamageDealt = table.Column<int>(type: "INTEGER", nullable: false),
                    PhysicalDamageDealt = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalDamageDealt = table.Column<int>(type: "INTEGER", nullable: false),
                    RiotIdGameName = table.Column<string>(type: "TEXT", nullable: false),
                    RiotIdTagLine = table.Column<string>(type: "TEXT", nullable: false),
                    TeamPosition = table.Column<int>(type: "INTEGER", nullable: true),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Participants_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MatchId = table.Column<long>(type: "INTEGER", nullable: false),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    Win = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TeamId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChampionId = table.Column<int>(type: "INTEGER", nullable: false),
                    PickTurn = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bans_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_RiotId",
                table: "Accounts",
                column: "RiotId");

            migrationBuilder.CreateIndex(
                name: "IX_Bans_TeamId",
                table: "Bans",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_MatchId",
                table: "Participants",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_MatchId",
                table: "Teams",
                column: "MatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Bans");

            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Matches");
        }
    }
}
