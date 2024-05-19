using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomService.Migrations
{
    /// <inheritdoc />
    public partial class eventAppUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DivisionDirector",
                table: "ApplicationData");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ApplicationData",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationData_UserId",
                table: "ApplicationData",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationData_WebUsers_UserId",
                table: "ApplicationData",
                column: "UserId",
                principalTable: "WebUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationData_WebUsers_UserId",
                table: "ApplicationData");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationData_UserId",
                table: "ApplicationData");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ApplicationData");

            migrationBuilder.AddColumn<bool>(
                name: "DivisionDirector",
                table: "ApplicationData",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
