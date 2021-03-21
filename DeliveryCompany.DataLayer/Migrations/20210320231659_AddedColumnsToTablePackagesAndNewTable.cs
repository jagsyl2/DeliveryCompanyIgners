using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DeliveryCompany.DataLayer.Migrations
{
    public partial class AddedColumnsToTablePackagesAndNewTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourierRating",
                table: "Packages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "Packages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ModeWaybill",
                table: "Packages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NameOfWaybill",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CouriersRating = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropColumn(
                name: "CourierRating",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "ModeWaybill",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "NameOfWaybill",
                table: "Packages");
        }
    }
}
