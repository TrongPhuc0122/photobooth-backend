using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTopic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TopicName",
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

            migrationBuilder.AddColumn<int>(
                name: "LayoutType",
                table: "Frames",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Topic",
                columns: table => new
                {
                    TopicId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TopicName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topic", x => x.TopicId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Frames_TopicId",
                table: "Frames",
                column: "TopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Frames_Topic_TopicId",
                table: "Frames",
                column: "TopicId",
                principalTable: "Topic",
                principalColumn: "TopicId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Frames_Topic_TopicId",
                table: "Frames");

            migrationBuilder.DropTable(
                name: "Topic");

            migrationBuilder.DropIndex(
                name: "IX_Frames_TopicId",
                table: "Frames");

            migrationBuilder.DropColumn(
                name: "LayoutType",
                table: "Frames");

            migrationBuilder.AlterColumn<int>(
                name: "TopicId",
                table: "Frames",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "TopicName",
                table: "Frames",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
