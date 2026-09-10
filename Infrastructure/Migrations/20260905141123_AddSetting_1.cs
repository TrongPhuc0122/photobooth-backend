using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSetting_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    BoothId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Av = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Exposure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Metering = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Flash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AFMode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PictureStyle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DriveMode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ColorCorrection = table.Column<bool>(type: "bit", nullable: false),
                    AutoRotate = table.Column<bool>(type: "bit", nullable: false),
                    Borderless = table.Column<bool>(type: "bit", nullable: false),
                    HalfCut = table.Column<bool>(type: "bit", nullable: false),
                    PrinterDriverName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.BoothId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
