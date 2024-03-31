using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomService.Migrations
{
    /// <inheritdoc />
    public partial class EventApplicationUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_Events_EventId",
                table: "News");

            migrationBuilder.AlterColumn<int>(
                name: "EventId",
                table: "News",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Events",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationSenderId",
                table: "EventApplications",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_EventApplications_ApplicationSenderId",
                table: "EventApplications",
                column: "ApplicationSenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventApplications_OrganizationUsers_ApplicationSenderId",
                table: "EventApplications",
                column: "ApplicationSenderId",
                principalTable: "OrganizationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_News_Events_EventId",
                table: "News",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventApplications_OrganizationUsers_ApplicationSenderId",
                table: "EventApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_News_Events_EventId",
                table: "News");

            migrationBuilder.DropIndex(
                name: "IX_EventApplications_ApplicationSenderId",
                table: "EventApplications");

            migrationBuilder.DropColumn(
                name: "ApplicationSenderId",
                table: "EventApplications");

            migrationBuilder.AlterColumn<int>(
                name: "EventId",
                table: "News",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Events",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AddForeignKey(
                name: "FK_News_Events_EventId",
                table: "News",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
