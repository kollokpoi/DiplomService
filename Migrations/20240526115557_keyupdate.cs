using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomService.Migrations
{
    /// <inheritdoc />
    public partial class keyupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationData_Divisions_DivisionId",
                table: "ApplicationData");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationData_Divisions_DivisionId",
                table: "ApplicationData",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationData_Divisions_DivisionId",
                table: "ApplicationData");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationData_Divisions_DivisionId",
                table: "ApplicationData",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id");
        }
    }
}
