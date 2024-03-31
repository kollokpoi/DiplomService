using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomService.Migrations
{
    /// <inheritdoc />
    public partial class MessageUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_AspNetUsers_SenderId1",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_SenderId1",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "SenderId1",
                table: "Message");

            migrationBuilder.AlterColumn<string>(
                name: "SenderId",
                table: "Message",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Message_SenderId",
                table: "Message",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_AspNetUsers_SenderId",
                table: "Message",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_AspNetUsers_SenderId",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_SenderId",
                table: "Message");

            migrationBuilder.AlterColumn<int>(
                name: "SenderId",
                table: "Message",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "SenderId1",
                table: "Message",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Message_SenderId1",
                table: "Message",
                column: "SenderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_AspNetUsers_SenderId1",
                table: "Message",
                column: "SenderId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
