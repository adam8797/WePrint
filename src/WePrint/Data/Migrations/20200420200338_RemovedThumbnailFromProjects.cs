using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WePrint.Data.Migrations
{
    public partial class RemovedThumbnailFromProjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "Projects");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("9b4261aa-6a0b-4664-b076-1bc3dd0476a3"), "97fe714d-b337-4044-8a83-3f92f444fe78", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("9b4261aa-6a0b-4664-b076-1bc3dd0476a3"));

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "Projects",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);
        }
    }
}
