using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MatchupCompanion.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSummonerSpellsAndMatchupEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AbilityOrder",
                table: "Matchups",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullBuildItems",
                table: "Matchups",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SummonerSpell1Id",
                table: "Matchups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SummonerSpell2Id",
                table: "Matchups",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SummonerSpells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RiotSpellId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Cooldown = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummonerSpells", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Champions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 16, 18, 19, 903, DateTimeKind.Utc).AddTicks(2099), new DateTime(2026, 1, 21, 16, 18, 19, 903, DateTimeKind.Utc).AddTicks(2102) });

            migrationBuilder.UpdateData(
                table: "Champions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 16, 18, 19, 903, DateTimeKind.Utc).AddTicks(2110), new DateTime(2026, 1, 21, 16, 18, 19, 903, DateTimeKind.Utc).AddTicks(2110) });

            migrationBuilder.UpdateData(
                table: "Champions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 21, 16, 18, 19, 903, DateTimeKind.Utc).AddTicks(2113), new DateTime(2026, 1, 21, 16, 18, 19, 903, DateTimeKind.Utc).AddTicks(2114) });

            migrationBuilder.InsertData(
                table: "SummonerSpells",
                columns: new[] { "Id", "Cooldown", "Description", "ImageUrl", "Name", "RiotSpellId" },
                values: new object[,]
                {
                    { 1, 300, "Teleports your champion a short distance toward your cursor's location.", "https://ddragon.leagueoflegends.com/cdn/14.1.1/img/spell/SummonerFlash.png", "Flash", 4 },
                    { 2, 180, "Ignites target enemy champion, dealing true damage over 5 seconds.", "https://ddragon.leagueoflegends.com/cdn/14.1.1/img/spell/SummonerDot.png", "Ignite", 14 },
                    { 3, 360, "After channeling for 4 seconds, teleports your champion to target allied structure or minion.", "https://ddragon.leagueoflegends.com/cdn/14.1.1/img/spell/SummonerTeleport.png", "Teleport", 12 },
                    { 4, 90, "Deals true damage to target monster or enemy minion.", "https://ddragon.leagueoflegends.com/cdn/14.1.1/img/spell/SummonerSmite.png", "Smite", 11 },
                    { 5, 210, "Exhausts target enemy champion, reducing their Movement Speed and damage dealt.", "https://ddragon.leagueoflegends.com/cdn/14.1.1/img/spell/SummonerExhaust.png", "Exhaust", 3 },
                    { 6, 240, "Restores Health to your champion and target allied champion and grants Movement Speed.", "https://ddragon.leagueoflegends.com/cdn/14.1.1/img/spell/SummonerHeal.png", "Heal", 7 },
                    { 7, 210, "Your champion gains increased Movement Speed and can move through units.", "https://ddragon.leagueoflegends.com/cdn/14.1.1/img/spell/SummonerHaste.png", "Ghost", 6 },
                    { 8, 180, "Shields your champion from damage.", "https://ddragon.leagueoflegends.com/cdn/14.1.1/img/spell/SummonerBarrier.png", "Barrier", 21 },
                    { 9, 210, "Removes all disables and summoner spell debuffs affecting your champion.", "https://ddragon.leagueoflegends.com/cdn/14.1.1/img/spell/SummonerBoost.png", "Cleanse", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SummonerSpells_Name",
                table: "SummonerSpells",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SummonerSpells_RiotSpellId",
                table: "SummonerSpells",
                column: "RiotSpellId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SummonerSpells");

            migrationBuilder.DropColumn(
                name: "AbilityOrder",
                table: "Matchups");

            migrationBuilder.DropColumn(
                name: "FullBuildItems",
                table: "Matchups");

            migrationBuilder.DropColumn(
                name: "SummonerSpell1Id",
                table: "Matchups");

            migrationBuilder.DropColumn(
                name: "SummonerSpell2Id",
                table: "Matchups");

            migrationBuilder.UpdateData(
                table: "Champions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 15, 23, 56, 643, DateTimeKind.Utc).AddTicks(5595), new DateTime(2026, 1, 20, 15, 23, 56, 643, DateTimeKind.Utc).AddTicks(5597) });

            migrationBuilder.UpdateData(
                table: "Champions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 15, 23, 56, 643, DateTimeKind.Utc).AddTicks(5601), new DateTime(2026, 1, 20, 15, 23, 56, 643, DateTimeKind.Utc).AddTicks(5601) });

            migrationBuilder.UpdateData(
                table: "Champions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 15, 23, 56, 643, DateTimeKind.Utc).AddTicks(5604), new DateTime(2026, 1, 20, 15, 23, 56, 643, DateTimeKind.Utc).AddTicks(5605) });
        }
    }
}
