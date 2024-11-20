using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CameraRentalApp.Migrations
{
    public partial class AddIsDeletedToCameras : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Cameras",
            nullable: false,
            defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Cameras");
        }
    }
}
