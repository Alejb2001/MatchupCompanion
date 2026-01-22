using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchupCompanion.API.Migrations
{
    /// <inheritdoc />
    public partial class AddChampionAbilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ESpellIcon",
                table: "Champions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ESpellId",
                table: "Champions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ESpellName",
                table: "Champions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QSpellIcon",
                table: "Champions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QSpellId",
                table: "Champions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QSpellName",
                table: "Champions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RSpellIcon",
                table: "Champions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RSpellId",
                table: "Champions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RSpellName",
                table: "Champions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WSpellIcon",
                table: "Champions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WSpellId",
                table: "Champions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WSpellName",
                table: "Champions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Champions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "ESpellIcon", "ESpellId", "ESpellName", "QSpellIcon", "QSpellId", "QSpellName", "RSpellIcon", "RSpellId", "RSpellName", "UpdatedAt", "WSpellIcon", "WSpellId", "WSpellName" },
                values: new object[] { new DateTime(2026, 1, 21, 22, 10, 56, 309, DateTimeKind.Utc).AddTicks(1625), null, null, null, null, null, null, null, null, null, new DateTime(2026, 1, 21, 22, 10, 56, 309, DateTimeKind.Utc).AddTicks(1627), null, null, null });

            migrationBuilder.UpdateData(
                table: "Champions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "ESpellIcon", "ESpellId", "ESpellName", "QSpellIcon", "QSpellId", "QSpellName", "RSpellIcon", "RSpellId", "RSpellName", "UpdatedAt", "WSpellIcon", "WSpellId", "WSpellName" },
                values: new object[] { new DateTime(2026, 1, 21, 22, 10, 56, 309, DateTimeKind.Utc).AddTicks(1631), null, null, null, null, null, null, null, null, null, new DateTime(2026, 1, 21, 22, 10, 56, 309, DateTimeKind.Utc).AddTicks(1631), null, null, null });

            migrationBuilder.UpdateData(
                table: "Champions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "ESpellIcon", "ESpellId", "ESpellName", "QSpellIcon", "QSpellId", "QSpellName", "RSpellIcon", "RSpellId", "RSpellName", "UpdatedAt", "WSpellIcon", "WSpellId", "WSpellName" },
                values: new object[] { new DateTime(2026, 1, 21, 22, 10, 56, 309, DateTimeKind.Utc).AddTicks(1635), null, null, null, null, null, null, null, null, null, new DateTime(2026, 1, 21, 22, 10, 56, 309, DateTimeKind.Utc).AddTicks(1636), null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ESpellIcon",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "ESpellId",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "ESpellName",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "QSpellIcon",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "QSpellId",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "QSpellName",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "RSpellIcon",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "RSpellId",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "RSpellName",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "WSpellIcon",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "WSpellId",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "WSpellName",
                table: "Champions");

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
        }
    }
}
