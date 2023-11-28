using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiTutorial.Migrations
{
    /// <inheritdoc />
    public partial class ChangeVoteOption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Votes",
                table: "PollOption");

            migrationBuilder.AddColumn<int>(
                name: "PollOptionID",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PollOptionID",
                table: "Users",
                column: "PollOptionID");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_PollOption_PollOptionID",
                table: "Users",
                column: "PollOptionID",
                principalTable: "PollOption",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_PollOption_PollOptionID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PollOptionID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PollOptionID",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "Votes",
                table: "PollOption",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
