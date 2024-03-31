using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomService.Migrations
{
    /// <inheritdoc />
    public partial class measureUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeasureDates_Measures_MeasureId",
                table: "MeasureDates");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasureDays_Measures_MeasureId",
                table: "MeasureDays");

            migrationBuilder.RenameColumn(
                name: "MeasureId",
                table: "MeasureDays",
                newName: "MeasureDivisionsInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_MeasureDays_MeasureId",
                table: "MeasureDays",
                newName: "IX_MeasureDays_MeasureDivisionsInfoId");

            migrationBuilder.RenameColumn(
                name: "MeasureId",
                table: "MeasureDates",
                newName: "MeasureDivisionsInfosId");

            migrationBuilder.RenameIndex(
                name: "IX_MeasureDates_MeasureId",
                table: "MeasureDates",
                newName: "IX_MeasureDates_MeasureDivisionsInfosId");

            migrationBuilder.AddColumn<bool>(
                name: "SameForAll",
                table: "MeasureDivisionsInfos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasureDates_MeasureDivisionsInfos_MeasureDivisionsInfosId",
                table: "MeasureDates",
                column: "MeasureDivisionsInfosId",
                principalTable: "MeasureDivisionsInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasureDays_MeasureDivisionsInfos_MeasureDivisionsInfoId",
                table: "MeasureDays",
                column: "MeasureDivisionsInfoId",
                principalTable: "MeasureDivisionsInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeasureDates_MeasureDivisionsInfos_MeasureDivisionsInfosId",
                table: "MeasureDates");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasureDays_MeasureDivisionsInfos_MeasureDivisionsInfoId",
                table: "MeasureDays");

            migrationBuilder.DropColumn(
                name: "SameForAll",
                table: "MeasureDivisionsInfos");

            migrationBuilder.RenameColumn(
                name: "MeasureDivisionsInfoId",
                table: "MeasureDays",
                newName: "MeasureId");

            migrationBuilder.RenameIndex(
                name: "IX_MeasureDays_MeasureDivisionsInfoId",
                table: "MeasureDays",
                newName: "IX_MeasureDays_MeasureId");

            migrationBuilder.RenameColumn(
                name: "MeasureDivisionsInfosId",
                table: "MeasureDates",
                newName: "MeasureId");

            migrationBuilder.RenameIndex(
                name: "IX_MeasureDates_MeasureDivisionsInfosId",
                table: "MeasureDates",
                newName: "IX_MeasureDates_MeasureId");

            migrationBuilder.AddForeignKey(
                name: "FK_MeasureDates_Measures_MeasureId",
                table: "MeasureDates",
                column: "MeasureId",
                principalTable: "Measures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasureDays_Measures_MeasureId",
                table: "MeasureDays",
                column: "MeasureId",
                principalTable: "Measures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
