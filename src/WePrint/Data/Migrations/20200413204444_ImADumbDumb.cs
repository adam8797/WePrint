using Microsoft.EntityFrameworkCore.Migrations;

namespace WePrint.Data.Migrations
{
    public partial class ImADumbDumb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address_AddressLine4",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Address_AddressLine4",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Address_AddressLine4",
                table: "Jobs");

            migrationBuilder.AddColumn<string>(
                name: "Address_AddressLine1",
                table: "Projects",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_AddressLine1",
                table: "Organizations",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_AddressLine1",
                table: "Jobs",
                maxLength: 150,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address_AddressLine1",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Address_AddressLine1",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Address_AddressLine1",
                table: "Jobs");

            migrationBuilder.AddColumn<string>(
                name: "Address_AddressLine4",
                table: "Projects",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_AddressLine4",
                table: "Organizations",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_AddressLine4",
                table: "Jobs",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }
    }
}
