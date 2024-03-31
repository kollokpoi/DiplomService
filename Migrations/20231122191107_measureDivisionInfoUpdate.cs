using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomService.Migrations
{
    /// <inheritdoc />
    public partial class measureDivisionInfoUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeasureDates_Divisions_DivisionId",
                table: "MeasureDates");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasureDays_Divisions_DivisionId",
                table: "MeasureDays");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasureDivisionsInfos_Measures_MeasureId",
                table: "MeasureDivisionsInfos");

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
                name: "DivisionId",
                table: "MeasureDates");

            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "MeasureDivisionsInfos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeasureDivisionsInfos_DivisionId",
                table: "MeasureDivisionsInfos",
                column: "DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_MeasureDivisionsInfos_Divisions_DivisionId",
                table: "MeasureDivisionsInfos",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MeasureDivisionsInfos_Measures_MeasureId",
                table: "MeasureDivisionsInfos",
                column: "MeasureId",
                principalTable: "Measures",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeasureDivisionsInfos_Divisions_DivisionId",
                table: "MeasureDivisionsInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasureDivisionsInfos_Measures_MeasureId",
                table: "MeasureDivisionsInfos");

            migrationBuilder.DropIndex(
                name: "IX_MeasureDivisionsInfos_DivisionId",
                table: "MeasureDivisionsInfos");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "MeasureDivisionsInfos");

            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "MeasureDays",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DivisionId",
                table: "MeasureDates",
                type: "int",
                nullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_MeasureDivisionsInfos_Measures_MeasureId",
                table: "MeasureDivisionsInfos",
                column: "MeasureId",
                principalTable: "Measures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
