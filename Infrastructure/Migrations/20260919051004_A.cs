using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class A : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SettingHistories_Booths_BoothId",
                table: "SettingHistories");

            migrationBuilder.DropIndex(
                name: "IX_SettingHistories_BoothId",
                table: "SettingHistories");

            migrationBuilder.AddColumn<int>(
                name: "SettingHistoryId",
                table: "Booths",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Booths_SettingHistoryId",
                table: "Booths",
                column: "SettingHistoryId",
                unique: true,
                filter: "[SettingHistoryId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Booths_SettingHistories_SettingHistoryId",
                table: "Booths",
                column: "SettingHistoryId",
                principalTable: "SettingHistories",
                principalColumn: "SettingHistoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booths_SettingHistories_SettingHistoryId",
                table: "Booths");

            migrationBuilder.DropIndex(
                name: "IX_Booths_SettingHistoryId",
                table: "Booths");

            migrationBuilder.DropColumn(
                name: "SettingHistoryId",
                table: "Booths");

            migrationBuilder.CreateIndex(
                name: "IX_SettingHistories_BoothId",
                table: "SettingHistories",
                column: "BoothId");

            migrationBuilder.AddForeignKey(
                name: "FK_SettingHistories_Booths_BoothId",
                table: "SettingHistories",
                column: "BoothId",
                principalTable: "Booths",
                principalColumn: "BoothId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
