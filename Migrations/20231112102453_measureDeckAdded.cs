using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomService.Migrations
{
    /// <inheritdoc />
    public partial class measureDeckAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Information",
                table: "Measures",
                newName: "Descrition");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descrition",
                table: "Measures",
                newName: "Information");
        }
    }
}
