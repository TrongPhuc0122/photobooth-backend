using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SettingHistoryId",
                table: "Booths");

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    SettingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SettingStatus = table.Column<bool>(type: "bit", nullable: false),
                    SettingTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Camera = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Printer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    System = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.SettingId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Booths_SettingId",
                table: "Booths",
                column: "SettingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Booths_Settings_SettingId",
                table: "Booths",
                column: "SettingId",
                principalTable: "Settings",
                principalColumn: "SettingId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booths_Settings_SettingId",
                table: "Booths");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropIndex(
                name: "IX_Booths_SettingId",
                table: "Booths");

            migrationBuilder.AddColumn<int>(
                name: "SettingHistoryId",
                table: "Booths",
                type: "int",
                nullable: true);
        }
    }
}
