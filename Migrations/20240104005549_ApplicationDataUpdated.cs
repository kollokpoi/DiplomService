using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomService.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationDataUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventApplications_OrganizationUsers_ApplicationSenderId",
                table: "EventApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_EventApplications_WebUsers_WebUserId",
                table: "EventApplications");

            migrationBuilder.DropIndex(
                name: "IX_EventApplications_WebUserId",
                table: "EventApplications");

            migrationBuilder.DropColumn(
                name: "WebUserId",
                table: "EventApplications");

            migrationBuilder.DropColumn(
                name: "Institution",
                table: "ApplicationData");

            migrationBuilder.AddColumn<bool>(
                name: "Accepted",
                table: "EventApplications",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Institution",
                table: "EventApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ApplicationData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Course",
                table: "ApplicationData",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "ApplicationData",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationData_DivisionId",
                table: "ApplicationData",
                column: "DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationData_Divisions_DivisionId",
                table: "ApplicationData",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventApplications_WebUsers_ApplicationSenderId",
                table: "EventApplications",
                column: "ApplicationSenderId",
                principalTable: "WebUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationData_Divisions_DivisionId",
                table: "ApplicationData");

            migrationBuilder.DropForeignKey(
                name: "FK_EventApplications_WebUsers_ApplicationSenderId",
                table: "EventApplications");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationData_DivisionId",
                table: "ApplicationData");

            migrationBuilder.DropColumn(
                name: "Accepted",
                table: "EventApplications");

            migrationBuilder.DropColumn(
                name: "Institution",
                table: "EventApplications");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "ApplicationData");

            migrationBuilder.AddColumn<string>(
                name: "WebUserId",
                table: "EventApplications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ApplicationData",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Course",
                table: "ApplicationData",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Institution",
                table: "ApplicationData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventApplications_WebUserId",
                table: "EventApplications",
                column: "WebUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventApplications_OrganizationUsers_ApplicationSenderId",
                table: "EventApplications",
                column: "ApplicationSenderId",
                principalTable: "OrganizationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventApplications_WebUsers_WebUserId",
                table: "EventApplications",
                column: "WebUserId",
                principalTable: "WebUsers",
                principalColumn: "Id");
        }
    }
}
