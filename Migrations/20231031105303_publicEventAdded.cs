using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomService.Migrations
{
    /// <inheritdoc />
    public partial class publicEventAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PublicEvent",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicEvent",
                table: "Events");
        }
    }
}
