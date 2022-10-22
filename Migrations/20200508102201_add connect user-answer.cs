using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication2.Migrations
{
    public partial class addconnectuseranswer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "answers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_answers_UserId",
                table: "answers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_answers_AspNetUsers_UserId",
                table: "answers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_answers_AspNetUsers_UserId",
                table: "answers");

            migrationBuilder.DropIndex(
                name: "IX_answers_UserId",
                table: "answers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "answers");
        }
    }
}
