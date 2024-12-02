using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CameraRentalApp.Migrations
{
    public partial class AddRentalCodeToRentals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RentalCode",
                table: "Rentals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RentalCode",
                table: "Rentals");
        }
    }
}
