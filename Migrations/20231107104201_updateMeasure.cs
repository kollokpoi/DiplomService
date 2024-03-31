using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomService.Migrations
{
    /// <inheritdoc />
    public partial class updateMeasure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "MeasureDays",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Place",
                table: "MeasureDays",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "MeasureDates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Place",
                table: "MeasureDates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MeasureDays_DivisionId",
                table: "MeasureDays",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasureDates_DivisionId",
                table: "MeasureDates",
                column: "DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_MeasureDates_Divisions_DivisionId",
                table: "MeasureDates",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MeasureDays_Divisions_DivisionId",
                table: "MeasureDays",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeasureDates_Divisions_DivisionId",
                table: "MeasureDates");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasureDays_Divisions_DivisionId",
                table: "MeasureDays");

            migrationBuilder.DropIndex(
                name: "IX_MeasureDays_DivisionId",
                table: "MeasureDays");

            migrationBuilder.DropIndex(
                name: "IX_MeasureDates_DivisionId",
                table: "MeasureDates");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "MeasureDays");

            migrationBuilder.DropColumn(
                name: "Place",
                table: "MeasureDays");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "MeasureDates");

            migrationBuilder.DropColumn(
                name: "Place",
                table: "MeasureDates");
        }
    }
}
