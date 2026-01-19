using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchupCompanion.API.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseItemNameLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Items",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.UpdateData(
                table: "Champions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 19, 18, 16, 7, 546, DateTimeKind.Utc).AddTicks(3970), new DateTime(2026, 1, 19, 18, 16, 7, 546, DateTimeKind.Utc).AddTicks(3971) });

            migrationBuilder.UpdateData(
                table: "Champions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 19, 18, 16, 7, 546, DateTimeKind.Utc).AddTicks(3974), new DateTime(2026, 1, 19, 18, 16, 7, 546, DateTimeKind.Utc).AddTicks(3975) });

            migrationBuilder.UpdateData(
                table: "Champions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 19, 18, 16, 7, 546, DateTimeKind.Utc).AddTicks(3977), new DateTime(2026, 1, 19, 18, 16, 7, 546, DateTimeKind.Utc).AddTicks(3978) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Items",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

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
        }
    }
}
