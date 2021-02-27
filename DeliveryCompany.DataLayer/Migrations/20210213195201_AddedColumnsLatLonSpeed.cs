using Microsoft.EntityFrameworkCore.Migrations;

namespace DeliveryCompany.DataLayer.Migrations
{
    public partial class AddedColumnsLatLonSpeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AverageSpeed",
                table: "Vehicles",
                type: "int",
                nullable: false,
                defaultValue: 70);

            migrationBuilder.AddColumn<double>(
                name: "lat",
                table: "Users",
                type: "float",
                nullable: false,
                defaultValue: 999.0);

            migrationBuilder.AddColumn<double>(
                name: "lon",
                table: "Users",
                type: "float",
                nullable: false,
                defaultValue: 999.0);

            migrationBuilder.AddColumn<double>(
                name: "RecipientLat",
                table: "Packages",
                type: "float",
                nullable: false,
                defaultValue: 999.0);

            migrationBuilder.AddColumn<double>(
                name: "RecipientLon",
                table: "Packages",
                type: "float",
                nullable: false,
                defaultValue: 999.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageSpeed",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "lat",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "lon",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RecipientLat",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "RecipientLon",
                table: "Packages");
        }
    }
}
