using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchupCompanion.API.Migrations
{
    /// <inheritdoc />
    public partial class AddRunesItemsAndMatchupFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoreItems",
                table: "Matchups",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KeystoneId",
                table: "Matchups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrimaryRune1Id",
                table: "Matchups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrimaryRune2Id",
                table: "Matchups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrimaryRune3Id",
                table: "Matchups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrimaryTreeId",
                table: "Matchups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SecondaryRune1Id",
                table: "Matchups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SecondaryRune2Id",
                table: "Matchups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SecondaryTreeId",
                table: "Matchups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SituationalItems",
                table: "Matchups",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartingItems",
                table: "Matchups",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatShards",
                table: "Matchups",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Strategy",
                table: "Matchups",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RiotItemId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IconPath = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TotalGold = table.Column<int>(type: "int", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsPurchasable = table.Column<bool>(type: "bit", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    BuildsFrom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BuildsInto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Runes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RiotRuneId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IconPath = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TreeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TreeId = table.Column<int>(type: "int", nullable: false),
                    SlotIndex = table.Column<int>(type: "int", nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Runes", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Champions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 16, 19, 51, 1, 103, DateTimeKind.Utc).AddTicks(9015), new DateTime(2026, 1, 16, 19, 51, 1, 103, DateTimeKind.Utc).AddTicks(9016) });

            migrationBuilder.UpdateData(
                table: "Champions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 16, 19, 51, 1, 103, DateTimeKind.Utc).AddTicks(9019), new DateTime(2026, 1, 16, 19, 51, 1, 103, DateTimeKind.Utc).AddTicks(9020) });

            migrationBuilder.UpdateData(
                table: "Champions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 16, 19, 51, 1, 103, DateTimeKind.Utc).AddTicks(9023), new DateTime(2026, 1, 16, 19, 51, 1, 103, DateTimeKind.Utc).AddTicks(9023) });

            migrationBuilder.CreateIndex(
                name: "IX_Items_IsCompleted",
                table: "Items",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_Items_IsPurchasable",
                table: "Items",
                column: "IsPurchasable");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Name",
                table: "Items",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Items_RiotItemId",
                table: "Items",
                column: "RiotItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Runes_RiotRuneId",
                table: "Runes",
                column: "RiotRuneId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Runes_SlotIndex",
                table: "Runes",
                column: "SlotIndex");

            migrationBuilder.CreateIndex(
                name: "IX_Runes_TreeId",
                table: "Runes",
                column: "TreeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Runes");

            migrationBuilder.DropColumn(
                name: "CoreItems",
                table: "Matchups");

            migrationBuilder.DropColumn(
                name: "KeystoneId",
                table: "Matchups");

            migrationBuilder.DropColumn(
                name: "PrimaryRune1Id",
                table: "Matchups");

            migrationBuilder.DropColumn(
                name: "PrimaryRune2Id",
                table: "Matchups");

            migrationBuilder.DropColumn(
                name: "PrimaryRune3Id",
                table: "Matchups");

            migrationBuilder.DropColumn(
                name: "PrimaryTreeId",
                table: "Matchups");

            migrationBuilder.DropColumn(
                name: "SecondaryRune1Id",
                table: "Matchups");

            migrationBuilder.DropColumn(
                name: "SecondaryRune2Id",
                table: "Matchups");

            migrationBuilder.DropColumn(
                name: "SecondaryTreeId",
                table: "Matchups");

            migrationBuilder.DropColumn(
                name: "SituationalItems",
                table: "Matchups");

            migrationBuilder.DropColumn(
                name: "StartingItems",
                table: "Matchups");

            migrationBuilder.DropColumn(
                name: "StatShards",
                table: "Matchups");

            migrationBuilder.DropColumn(
                name: "Strategy",
                table: "Matchups");

            migrationBuilder.UpdateData(
                table: "Champions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 15, 22, 22, 58, 698, DateTimeKind.Utc).AddTicks(9594), new DateTime(2026, 1, 15, 22, 22, 58, 698, DateTimeKind.Utc).AddTicks(9597) });

            migrationBuilder.UpdateData(
                table: "Champions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 15, 22, 22, 58, 698, DateTimeKind.Utc).AddTicks(9601), new DateTime(2026, 1, 15, 22, 22, 58, 698, DateTimeKind.Utc).AddTicks(9602) });

            migrationBuilder.UpdateData(
                table: "Champions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 15, 22, 22, 58, 698, DateTimeKind.Utc).AddTicks(9604), new DateTime(2026, 1, 15, 22, 22, 58, 698, DateTimeKind.Utc).AddTicks(9605) });
        }
    }
}
