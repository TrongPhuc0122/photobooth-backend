using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixVoucher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_BranchId",
                table: "Vouchers",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vouchers_Branches_BranchId",
                table: "Vouchers",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "BranchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vouchers_Branches_BranchId",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_BranchId",
                table: "Vouchers");
        }
    }
}
