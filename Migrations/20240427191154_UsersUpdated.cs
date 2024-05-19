using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomService.Migrations
{
    /// <inheritdoc />
    public partial class UsersUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMembers_MobileUsers_UserId",
                table: "ChatMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_DivisionUsers_MobileUsers_UserId",
                table: "DivisionUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDeviceTokens_MobileUsers_UserId",
                table: "UserDeviceTokens");

            migrationBuilder.AlterColumn<int>(
                name: "Course",
                table: "MobileUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Birthday",
                table: "MobileUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMembers_AspNetUsers_UserId",
                table: "ChatMembers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DivisionUsers_AspNetUsers_UserId",
                table: "DivisionUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDeviceTokens_AspNetUsers_UserId",
                table: "UserDeviceTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMembers_AspNetUsers_UserId",
                table: "ChatMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_DivisionUsers_AspNetUsers_UserId",
                table: "DivisionUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDeviceTokens_AspNetUsers_UserId",
                table: "UserDeviceTokens");

            migrationBuilder.AlterColumn<int>(
                name: "Course",
                table: "MobileUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Birthday",
                table: "MobileUsers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMembers_MobileUsers_UserId",
                table: "ChatMembers",
                column: "UserId",
                principalTable: "MobileUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DivisionUsers_MobileUsers_UserId",
                table: "DivisionUsers",
                column: "UserId",
                principalTable: "MobileUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDeviceTokens_MobileUsers_UserId",
                table: "UserDeviceTokens",
                column: "UserId",
                principalTable: "MobileUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
