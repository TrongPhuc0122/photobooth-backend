using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTopic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LayoutType",
                table: "Frames");

            migrationBuilder.RenameColumn(
                name: "SubjectImageUrl",
                table: "Frames",
                newName: "Subject");

            migrationBuilder.RenameColumn(
                name: "OverlayUrl",
                table: "Frames",
                newName: "Overlay");

            migrationBuilder.RenameColumn(
                name: "BackgroundUrl",
                table: "Frames",
                newName: "Background");

            migrationBuilder.AddColumn<string>(
                name: "BranchCode",
                table: "Topics",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "Topics",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "layoutType",
                table: "Topics",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchCode",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "layoutType",
                table: "Topics");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "Frames",
                newName: "SubjectImageUrl");

            migrationBuilder.RenameColumn(
                name: "Overlay",
                table: "Frames",
                newName: "OverlayUrl");

            migrationBuilder.RenameColumn(
                name: "Background",
                table: "Frames",
                newName: "BackgroundUrl");

            migrationBuilder.AddColumn<int>(
                name: "LayoutType",
                table: "Frames",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
