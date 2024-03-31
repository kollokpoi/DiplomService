using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomService.Migrations
{
    /// <inheritdoc />
    public partial class AddMeasureDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeasureDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Datetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MeasureId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasureDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeasureDates_Measures_MeasureId",
                        column: x => x.MeasureId,
                        principalTable: "Measures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeasureDates_MeasureId",
                table: "MeasureDates",
                column: "MeasureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeasureDates");
        }
    }
}
