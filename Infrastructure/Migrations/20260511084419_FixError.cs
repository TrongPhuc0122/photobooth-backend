using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixError : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Message",
                table: "BoothError",
                newName: "Solution");

            migrationBuilder.AddColumn<string>(
                name: "Cause",
                table: "BoothError",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResolvedBy",
                table: "BoothError",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cause",
                table: "BoothError");

            migrationBuilder.DropColumn(
                name: "ResolvedBy",
                table: "BoothError");

            migrationBuilder.RenameColumn(
                name: "Solution",
                table: "BoothError",
                newName: "Message");
        }
    }
}
