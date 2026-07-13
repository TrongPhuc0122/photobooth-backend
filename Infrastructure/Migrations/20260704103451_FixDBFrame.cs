using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixDBFrame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Frames");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Frames",
                newName: "SubjectImageUrl");

            migrationBuilder.AddColumn<string>(
                name: "BackgroundUrl",
                table: "Frames",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OverlayUrl",
                table: "Frames",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundUrl",
                table: "Frames");

            migrationBuilder.DropColumn(
                name: "OverlayUrl",
                table: "Frames");

            migrationBuilder.RenameColumn(
                name: "SubjectImageUrl",
                table: "Frames",
                newName: "ImageUrl");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Frames",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
