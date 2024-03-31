using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomService.Migrations
{
    /// <inheritdoc />
    public partial class MeasureIconAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Icon",
                table: "Measures",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DivisionDirector",
                table: "ApplicationData",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Measures");

            migrationBuilder.DropColumn(
                name: "DivisionDirector",
                table: "ApplicationData");
        }
    }
}
