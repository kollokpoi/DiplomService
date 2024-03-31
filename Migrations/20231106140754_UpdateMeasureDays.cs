using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMeasureDays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayName",
                table: "MeasureDays");

            migrationBuilder.AddColumn<bool>(
                name: "OneTime",
                table: "Measures",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WeekDays",
                table: "Measures",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DayNumber",
                table: "MeasureDays",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OneTime",
                table: "Measures");

            migrationBuilder.DropColumn(
                name: "WeekDays",
                table: "Measures");

            migrationBuilder.DropColumn(
                name: "DayNumber",
                table: "MeasureDays");

            migrationBuilder.AddColumn<string>(
                name: "DayName",
                table: "MeasureDays",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
