using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MatchupCompanion.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Champions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RiotChampionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PrimaryRoleId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Champions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Champions_Roles_PrimaryRoleId",
                        column: x => x.PrimaryRoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Matchups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerChampionId = table.Column<int>(type: "int", nullable: false),
                    EnemyChampionId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    Difficulty = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GeneralAdvice = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matchups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matchups_Champions_EnemyChampionId",
                        column: x => x.EnemyChampionId,
                        principalTable: "Champions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matchups_Champions_PlayerChampionId",
                        column: x => x.PlayerChampionId,
                        principalTable: "Champions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matchups_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatchupTips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchupId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuthorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchupTips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchupTips_Matchups_MatchupId",
                        column: x => x.MatchupId,
                        principalTable: "Matchups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Línea superior", "Top" },
                    { 2, "Jungla", "Jungle" },
                    { 3, "Línea media", "Mid" },
                    { 4, "Tirador (Bot Lane)", "ADC" },
                    { 5, "Soporte (Bot Lane)", "Support" }
                });

            migrationBuilder.InsertData(
                table: "Champions",
                columns: new[] { "Id", "CreatedAt", "Description", "ImageUrl", "Name", "PrimaryRoleId", "RiotChampionId", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 15, 22, 22, 58, 698, DateTimeKind.Utc).AddTicks(9594), null, null, "Aatrox", 1, "266", "the Darkin Blade", new DateTime(2026, 1, 15, 22, 22, 58, 698, DateTimeKind.Utc).AddTicks(9597) },
                    { 2, new DateTime(2026, 1, 15, 22, 22, 58, 698, DateTimeKind.Utc).AddTicks(9601), null, null, "Ahri", 3, "103", "the Nine-Tailed Fox", new DateTime(2026, 1, 15, 22, 22, 58, 698, DateTimeKind.Utc).AddTicks(9602) },
                    { 3, new DateTime(2026, 1, 15, 22, 22, 58, 698, DateTimeKind.Utc).AddTicks(9604), null, null, "Zed", 3, "238", "the Master of Shadows", new DateTime(2026, 1, 15, 22, 22, 58, 698, DateTimeKind.Utc).AddTicks(9605) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Champions_Name",
                table: "Champions",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Champions_PrimaryRoleId",
                table: "Champions",
                column: "PrimaryRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Champions_RiotChampionId",
                table: "Champions",
                column: "RiotChampionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matchups_EnemyChampionId",
                table: "Matchups",
                column: "EnemyChampionId");

            migrationBuilder.CreateIndex(
                name: "IX_Matchups_PlayerChampionId_EnemyChampionId_RoleId",
                table: "Matchups",
                columns: new[] { "PlayerChampionId", "EnemyChampionId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matchups_RoleId",
                table: "Matchups",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchupTips_Category",
                table: "MatchupTips",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_MatchupTips_MatchupId",
                table: "MatchupTips",
                column: "MatchupId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchupTips_Priority",
                table: "MatchupTips",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchupTips");

            migrationBuilder.DropTable(
                name: "Matchups");

            migrationBuilder.DropTable(
                name: "Champions");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
