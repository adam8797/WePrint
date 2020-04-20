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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "Projects",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);
        }
    }
}
