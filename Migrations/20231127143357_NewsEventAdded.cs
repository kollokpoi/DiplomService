using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomService.Migrations
{
    /// <inheritdoc />
    public partial class NewsEventAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "News",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_News_EventId",
                table: "News",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_News_Events_EventId",
                table: "News",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_Events_EventId",
                table: "News");

            migrationBuilder.DropIndex(
                name: "IX_News_EventId",
                table: "News");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "News");
        }
    }
}
