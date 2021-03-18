using Microsoft.EntityFrameworkCore.Migrations;

namespace DeliveryCompany.DataLayer.Migrations
{
    public partial class ModyficationTablePackage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ModeWaybill",
                table: "Packages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModeWaybill",
                table: "Packages");
        }
    }
}
