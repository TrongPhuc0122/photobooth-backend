using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSetting_001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SettingStatus",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "SettingTime",
                table: "Settings");

            migrationBuilder.CreateTable(
                name: "SettingLog",
                columns: table => new
                {
                    SettingLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoothId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettingId = table.Column<int>(type: "int", nullable: false),
                    SettingTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SettingStatus = table.Column<bool>(type: "bit", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingLog", x => x.SettingLogId);
                    table.ForeignKey(
                        name: "FK_SettingLog_Booths_BoothId",
                        column: x => x.BoothId,
                        principalTable: "Booths",
                        principalColumn: "BoothId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SettingLog_Settings_SettingId",
                        column: x => x.SettingId,
                        principalTable: "Settings",
                        principalColumn: "SettingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SettingLog_BoothId",
                table: "SettingLog",
                column: "BoothId");

            migrationBuilder.CreateIndex(
                name: "IX_SettingLog_SettingId",
                table: "SettingLog",
                column: "SettingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SettingLog");

            migrationBuilder.AddColumn<bool>(
                name: "SettingStatus",
                table: "Settings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SettingTime",
                table: "Settings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
