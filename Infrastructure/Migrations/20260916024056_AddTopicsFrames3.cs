using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTopicsFrames3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Frames_Topics_TopicId",
                table: "Frames");

            migrationBuilder.DropIndex(
                name: "IX_Frames_TopicId",
                table: "Frames");

            migrationBuilder.DropColumn(
                name: "TopicId",
                table: "Frames");

            migrationBuilder.CreateTable(
                name: "TopicsFrames",
                columns: table => new
                {
                    TopicId = table.Column<int>(type: "int", nullable: false),
                    FrameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicsFrames", x => new { x.TopicId, x.FrameId });
                    table.ForeignKey(
                        name: "FK_TopicsFrames_Frames_FrameId",
                        column: x => x.FrameId,
                        principalTable: "Frames",
                        principalColumn: "FrameId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TopicsFrames_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "TopicId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TopicsFrames_FrameId",
                table: "TopicsFrames",
                column: "FrameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TopicsFrames");

            migrationBuilder.AddColumn<int>(
                name: "TopicId",
                table: "Frames",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Frames_TopicId",
                table: "Frames",
                column: "TopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Frames_Topics_TopicId",
                table: "Frames",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "TopicId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
