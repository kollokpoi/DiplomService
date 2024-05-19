using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomService.Migrations
{
    /// <inheritdoc />
    public partial class eventAppChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationData_WebUsers_UserId",
                table: "ApplicationData");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationData_MobileUsers_UserId",
                table: "ApplicationData",
                column: "UserId",
                principalTable: "MobileUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationData_MobileUsers_UserId",
                table: "ApplicationData");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationData_WebUsers_UserId",
                table: "ApplicationData",
                column: "UserId",
                principalTable: "WebUsers",
                principalColumn: "Id");
        }
    }
}
