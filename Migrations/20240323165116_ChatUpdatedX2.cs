using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomService.Migrations
{
    /// <inheritdoc />
    public partial class ChatUpdatedX2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_AspNetUsers_FirstUserId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_AspNetUsers_SecondUserId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_FirstUserId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_SecondUserId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "FirstUserId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "SecondUserId",
                table: "Chats");

            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "Chats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ChatMembers",
                columns: table => new
                {
                    ChatId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMembers", x => new { x.ChatId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ChatMembers_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMembers_MobileUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "MobileUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chats_DivisionId",
                table: "Chats",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMembers_UserId",
                table: "ChatMembers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Divisions_DivisionId",
                table: "Chats",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Divisions_DivisionId",
                table: "Chats");

            migrationBuilder.DropTable(
                name: "ChatMembers");

            migrationBuilder.DropIndex(
                name: "IX_Chats_DivisionId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "Chats");

            migrationBuilder.AddColumn<string>(
                name: "FirstUserId",
                table: "Chats",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecondUserId",
                table: "Chats",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_FirstUserId",
                table: "Chats",
                column: "FirstUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_SecondUserId",
                table: "Chats",
                column: "SecondUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_AspNetUsers_FirstUserId",
                table: "Chats",
                column: "FirstUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_AspNetUsers_SecondUserId",
                table: "Chats",
                column: "SecondUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
