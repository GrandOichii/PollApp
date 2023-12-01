using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PollApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class MigrateToPostgreSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Polls",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Polls", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "PollOption",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "text", nullable: false),
                    PollID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollOption", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PollOption_Polls_PollID",
                        column: x => x.PollID,
                        principalTable: "Polls",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PollOptionUser",
                columns: table => new
                {
                    VotedForID = table.Column<int>(type: "integer", nullable: false),
                    VotedUsersUsername = table.Column<string>(type: "text", nullable: false)
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
                name: "IX_PollOption_PollID",
                table: "PollOption",
                column: "PollID");

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

            migrationBuilder.DropTable(
                name: "PollOption");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Polls");
        }
    }
}
