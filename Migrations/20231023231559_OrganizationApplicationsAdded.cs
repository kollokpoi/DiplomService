using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomService.Migrations
{
    /// <inheritdoc />
    public partial class OrganizationApplicationsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReadyToShow",
                table: "Organizations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "OrganizationApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    OrganizationEmail = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ApplicationApproved = table.Column<bool>(type: "bit", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationApplications", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationApplications");

            migrationBuilder.DropColumn(
                name: "ReadyToShow",
                table: "Organizations");
        }
    }
}
