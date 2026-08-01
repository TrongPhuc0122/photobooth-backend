using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceBranchIdInVoucher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vouchers_Branches_BranchId",
                table: "Vouchers");

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "Vouchers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BranchCode",
                table: "Vouchers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Vouchers_Branches_BranchId",
                table: "Vouchers",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "BranchId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vouchers_Branches_BranchId",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "BranchCode",
                table: "Vouchers");

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "Vouchers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Vouchers_Branches_BranchId",
                table: "Vouchers",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "BranchId");
        }
    }
}
