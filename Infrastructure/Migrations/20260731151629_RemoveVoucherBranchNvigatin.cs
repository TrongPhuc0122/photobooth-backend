using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVoucherBranchNvigatin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vouchers_Branches_BranchId",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_BranchId",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Vouchers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Vouchers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_BranchId",
                table: "Vouchers",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vouchers_Branches_BranchId",
                table: "Vouchers",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "BranchId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
