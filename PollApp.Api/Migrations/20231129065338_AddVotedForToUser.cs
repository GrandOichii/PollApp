using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PollApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddVotedForToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "PollOptionUser",
                columns: table => new
                {
                    VotedForID = table.Column<int>(type: "int", nullable: false),
                    VotedUsersUsername = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollOptionUser", x => new { x.VotedForID, x.VotedUsersUsername });
                    table.ForeignKey(
                        name: "FK_PollOptionUser_PollOption_VotedForID",
                        column: x => x.VotedForID,
                        principalTable: "PollOption",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollOptionUser_Users_VotedUsersUsername",
                        column: x => x.VotedUsersUsername,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PollOptionUser_VotedUsersUsername",
                table: "PollOptionUser",
                column: "VotedUsersUsername");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PollOptionUser");

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
    }
}
