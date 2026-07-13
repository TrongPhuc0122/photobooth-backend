using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FrameAPI_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Frames_BranchId",
                table: "Frames",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Frames_Branches_BranchId",
                table: "Frames",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "BranchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Frames_Branches_BranchId",
                table: "Frames");

            migrationBuilder.DropIndex(
                name: "IX_Frames_BranchId",
                table: "Frames");
        }
    }
}
