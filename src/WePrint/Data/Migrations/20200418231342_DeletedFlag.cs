using Microsoft.EntityFrameworkCore.Migrations;

namespace WePrint.Data.Migrations
{
    public partial class DeletedFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ProjectUpdates",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Projects",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Printers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Pledges",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Organizations",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ProjectUpdates");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Printers");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Pledges");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Organizations");
        }
    }
}
