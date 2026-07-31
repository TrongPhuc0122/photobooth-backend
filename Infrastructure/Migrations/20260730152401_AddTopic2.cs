using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTopic2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Frames_Topic_TopicId",
                table: "Frames");

            migrationBuilder.AlterColumn<int>(
                name: "TopicId",
                table: "Frames",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Frames_Topic_TopicId",
                table: "Frames",
                column: "TopicId",
                principalTable: "Topic",
                principalColumn: "TopicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Frames_Topic_TopicId",
                table: "Frames");

            migrationBuilder.AlterColumn<int>(
                name: "TopicId",
                table: "Frames",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Frames_Topic_TopicId",
                table: "Frames",
                column: "TopicId",
                principalTable: "Topic",
                principalColumn: "TopicId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
