using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SettingStatus",
                table: "Settings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SettingTime",
                table: "Settings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "System_Audio_BackgroundMusic",
                table: "Settings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "System_Audio_PrintBackgroundMusic",
                table: "Settings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "System_Audio_VoiceGuide",
                table: "Settings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "System_Audio_Volume",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "System_Capture_AutoCaptureCountdown",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "System_Capture_EvfGcIntervalSeconds",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "System_Capture_EvfOffMs",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "System_Capture_MaxThumbnails",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "System_Capture_ShotsFor4Cut",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "System_Capture_ShotsFor8Cut",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "System_Capture_TotalShots",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "System_Default_Country",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "System_Default_Language",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "System_Default_RunMode",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "System_Default_Server",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "System_Default_ServerAddress",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "System_Timer_ErrorScreenSeconds",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "System_Timer_PaymentMethodSeconds",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "System_Timer_PaymentWaitSeconds",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "System_Timer_PreviewSeconds",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "System_Timer_PrintWaitSeconds",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "System_Timer_SelectFilterSeconds",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "System_Timer_SelectPhotoSeconds",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "System_Timer_SelectScreenSeconds",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "System_Timer_SelectVoucherSeconds",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "System_Timer_UnitPricePaymentSeconds",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SettingId",
                table: "Booths",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SettingHistories",
                columns: table => new
                {
                    SettingHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoothId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Camera_AE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Camera_ISO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Camera_Tv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Camera_Av = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Camera_Exposure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Camera_WB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Camera_Metering = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Camera_Quality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Camera_Flash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Camera_AFMode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Camera_PictureStyle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Camera_DriveMode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Printer_ColorCorrection = table.Column<bool>(type: "bit", nullable: false),
                    Printer_AutoRotate = table.Column<bool>(type: "bit", nullable: false),
                    Printer_Borderless = table.Column<bool>(type: "bit", nullable: false),
                    Printer_HalfCut = table.Column<bool>(type: "bit", nullable: false),
                    Printer_PrinterDriverName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    System_Default_Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    System_Default_Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    System_Default_RunMode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    System_Default_Server = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    System_Default_ServerAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    System_Timer_PreviewSeconds = table.Column<int>(type: "int", nullable: false),
                    System_Timer_SelectScreenSeconds = table.Column<int>(type: "int", nullable: false),
                    System_Timer_SelectPhotoSeconds = table.Column<int>(type: "int", nullable: false),
                    System_Timer_PaymentWaitSeconds = table.Column<int>(type: "int", nullable: false),
                    System_Timer_ErrorScreenSeconds = table.Column<int>(type: "int", nullable: false),
                    System_Timer_SelectFilterSeconds = table.Column<int>(type: "int", nullable: false),
                    System_Timer_PaymentMethodSeconds = table.Column<int>(type: "int", nullable: false),
                    System_Timer_UnitPricePaymentSeconds = table.Column<int>(type: "int", nullable: false),
                    System_Timer_SelectVoucherSeconds = table.Column<int>(type: "int", nullable: false),
                    System_Timer_PrintWaitSeconds = table.Column<int>(type: "int", nullable: false),
                    System_Capture_TotalShots = table.Column<int>(type: "int", nullable: false),
                    System_Capture_AutoCaptureCountdown = table.Column<int>(type: "int", nullable: false),
                    System_Capture_ShotsFor4Cut = table.Column<int>(type: "int", nullable: false),
                    System_Capture_ShotsFor8Cut = table.Column<int>(type: "int", nullable: false),
                    System_Capture_MaxThumbnails = table.Column<int>(type: "int", nullable: false),
                    System_Capture_EvfGcIntervalSeconds = table.Column<int>(type: "int", nullable: false),
                    System_Capture_EvfOffMs = table.Column<int>(type: "int", nullable: false),
                    System_Audio_VoiceGuide = table.Column<bool>(type: "bit", nullable: false),
                    System_Audio_BackgroundMusic = table.Column<bool>(type: "bit", nullable: false),
                    System_Audio_Volume = table.Column<int>(type: "int", nullable: false),
                    System_Audio_PrintBackgroundMusic = table.Column<bool>(type: "bit", nullable: false),
                    CalledAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingHistories", x => x.SettingHistoryId);
                    table.ForeignKey(
                        name: "FK_SettingHistories_Booths_BoothId",
                        column: x => x.BoothId,
                        principalTable: "Booths",
                        principalColumn: "BoothId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Booths_SettingId",
                table: "Booths",
                column: "SettingId");

            migrationBuilder.CreateIndex(
                name: "IX_SettingHistories_BoothId",
                table: "SettingHistories",
                column: "BoothId");

            migrationBuilder.AddForeignKey(
                name: "FK_Booths_Settings_SettingId",
                table: "Booths",
                column: "SettingId",
                principalTable: "Settings",
                principalColumn: "SettingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booths_Settings_SettingId",
                table: "Booths");

            migrationBuilder.DropTable(
                name: "SettingHistories");

            migrationBuilder.DropIndex(
                name: "IX_Booths_SettingId",
                table: "Booths");

            migrationBuilder.DropColumn(
                name: "SettingStatus",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "SettingTime",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Audio_BackgroundMusic",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Audio_PrintBackgroundMusic",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Audio_VoiceGuide",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Audio_Volume",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Capture_AutoCaptureCountdown",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Capture_EvfGcIntervalSeconds",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Capture_EvfOffMs",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Capture_MaxThumbnails",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Capture_ShotsFor4Cut",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Capture_ShotsFor8Cut",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Capture_TotalShots",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Default_Country",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Default_Language",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Default_RunMode",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Default_Server",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Default_ServerAddress",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Timer_ErrorScreenSeconds",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Timer_PaymentMethodSeconds",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Timer_PaymentWaitSeconds",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Timer_PreviewSeconds",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Timer_PrintWaitSeconds",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Timer_SelectFilterSeconds",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Timer_SelectPhotoSeconds",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Timer_SelectScreenSeconds",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Timer_SelectVoucherSeconds",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "System_Timer_UnitPricePaymentSeconds",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "SettingId",
                table: "Booths");
        }
    }
}
