using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomService.Migrations
{
    /// <inheritdoc />
    public partial class mapAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DivisionUsers_MobileUsers_UserId",
                table: "DivisionUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_AspNetUsers_SenderId",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_SenderId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "MainPlace",
                table: "Divisions");

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

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "DivisionUsers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Divisions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Divisions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "PlaceName",
                table: "Divisions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Message_SenderId1",
                table: "Message",
                column: "SenderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DivisionUsers_MobileUsers_UserId",
                table: "DivisionUsers",
                column: "UserId",
                principalTable: "MobileUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_AspNetUsers_SenderId1",
                table: "Message",
                column: "SenderId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DivisionUsers_MobileUsers_UserId",
                table: "DivisionUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_AspNetUsers_SenderId1",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_SenderId1",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "SenderId1",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Divisions");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Divisions");

            migrationBuilder.DropColumn(
                name: "PlaceName",
                table: "Divisions");

            migrationBuilder.AlterColumn<string>(
                name: "SenderId",
                table: "Message",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "DivisionUsers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "MainPlace",
                table: "Divisions",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Message_SenderId",
                table: "Message",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_DivisionUsers_MobileUsers_UserId",
                table: "DivisionUsers",
                column: "UserId",
                principalTable: "MobileUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_AspNetUsers_SenderId",
                table: "Message",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
