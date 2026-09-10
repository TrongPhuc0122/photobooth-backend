using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSetting_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Settings",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "BoothId",
                table: "Settings");

            migrationBuilder.RenameColumn(
                name: "WB",
                table: "Settings",
                newName: "Camera_WB");

            migrationBuilder.RenameColumn(
                name: "Tv",
                table: "Settings",
                newName: "Camera_Tv");

            migrationBuilder.RenameColumn(
                name: "Quality",
                table: "Settings",
                newName: "Camera_Quality");

            migrationBuilder.RenameColumn(
                name: "PrinterDriverName",
                table: "Settings",
                newName: "Printer_PrinterDriverName");

            migrationBuilder.RenameColumn(
                name: "PictureStyle",
                table: "Settings",
                newName: "Camera_PictureStyle");

            migrationBuilder.RenameColumn(
                name: "Metering",
                table: "Settings",
                newName: "Camera_Metering");

            migrationBuilder.RenameColumn(
                name: "ISO",
                table: "Settings",
                newName: "Camera_ISO");

            migrationBuilder.RenameColumn(
                name: "HalfCut",
                table: "Settings",
                newName: "Printer_HalfCut");

            migrationBuilder.RenameColumn(
                name: "Flash",
                table: "Settings",
                newName: "Camera_Flash");

            migrationBuilder.RenameColumn(
                name: "Exposure",
                table: "Settings",
                newName: "Camera_Exposure");

            migrationBuilder.RenameColumn(
                name: "DriveMode",
                table: "Settings",
                newName: "Camera_DriveMode");

            migrationBuilder.RenameColumn(
                name: "ColorCorrection",
                table: "Settings",
                newName: "Printer_ColorCorrection");

            migrationBuilder.RenameColumn(
                name: "Borderless",
                table: "Settings",
                newName: "Printer_Borderless");

            migrationBuilder.RenameColumn(
                name: "Av",
                table: "Settings",
                newName: "Camera_Av");

            migrationBuilder.RenameColumn(
                name: "AutoRotate",
                table: "Settings",
                newName: "Printer_AutoRotate");

            migrationBuilder.RenameColumn(
                name: "AFMode",
                table: "Settings",
                newName: "Camera_AFMode");

            migrationBuilder.RenameColumn(
                name: "AE",
                table: "Settings",
                newName: "Camera_AE");

            migrationBuilder.AddColumn<int>(
                name: "SettingId",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Settings",
                table: "Settings",
                column: "SettingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Settings",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "SettingId",
                table: "Settings");

            migrationBuilder.RenameColumn(
                name: "Printer_PrinterDriverName",
                table: "Settings",
                newName: "PrinterDriverName");

            migrationBuilder.RenameColumn(
                name: "Printer_HalfCut",
                table: "Settings",
                newName: "HalfCut");

            migrationBuilder.RenameColumn(
                name: "Printer_ColorCorrection",
                table: "Settings",
                newName: "ColorCorrection");

            migrationBuilder.RenameColumn(
                name: "Printer_Borderless",
                table: "Settings",
                newName: "Borderless");

            migrationBuilder.RenameColumn(
                name: "Printer_AutoRotate",
                table: "Settings",
                newName: "AutoRotate");

            migrationBuilder.RenameColumn(
                name: "Camera_WB",
                table: "Settings",
                newName: "WB");

            migrationBuilder.RenameColumn(
                name: "Camera_Tv",
                table: "Settings",
                newName: "Tv");

            migrationBuilder.RenameColumn(
                name: "Camera_Quality",
                table: "Settings",
                newName: "Quality");

            migrationBuilder.RenameColumn(
                name: "Camera_PictureStyle",
                table: "Settings",
                newName: "PictureStyle");

            migrationBuilder.RenameColumn(
                name: "Camera_Metering",
                table: "Settings",
                newName: "Metering");

            migrationBuilder.RenameColumn(
                name: "Camera_ISO",
                table: "Settings",
                newName: "ISO");

            migrationBuilder.RenameColumn(
                name: "Camera_Flash",
                table: "Settings",
                newName: "Flash");

            migrationBuilder.RenameColumn(
                name: "Camera_Exposure",
                table: "Settings",
                newName: "Exposure");

            migrationBuilder.RenameColumn(
                name: "Camera_DriveMode",
                table: "Settings",
                newName: "DriveMode");

            migrationBuilder.RenameColumn(
                name: "Camera_Av",
                table: "Settings",
                newName: "Av");

            migrationBuilder.RenameColumn(
                name: "Camera_AFMode",
                table: "Settings",
                newName: "AFMode");

            migrationBuilder.RenameColumn(
                name: "Camera_AE",
                table: "Settings",
                newName: "AE");

            migrationBuilder.AddColumn<Guid>(
                name: "BoothId",
                table: "Settings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Settings",
                table: "Settings",
                column: "BoothId");
        }
    }
}
