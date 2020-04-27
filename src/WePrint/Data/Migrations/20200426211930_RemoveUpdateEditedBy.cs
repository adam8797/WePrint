using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WePrint.Data.Migrations
{
    public partial class RemoveUpdateEditedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUpdates_Users_EditedById",
                table: "ProjectUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUpdates_Users_Id",
                table: "ProjectUpdates");

            migrationBuilder.DropIndex(
                name: "IX_ProjectUpdates_EditedById",
                table: "ProjectUpdates");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("9b4261aa-6a0b-4664-b076-1bc3dd0476a3"));

            migrationBuilder.DropColumn(
                name: "EditedById",
                table: "ProjectUpdates");

            migrationBuilder.AddColumn<Guid>(
                name: "PostedById",
                table: "ProjectUpdates",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUpdates_PostedById",
                table: "ProjectUpdates",
                column: "PostedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUpdates_Users_PostedById",
                table: "ProjectUpdates",
                column: "PostedById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUpdates_Users_PostedById",
                table: "ProjectUpdates");

            migrationBuilder.DropIndex(
                name: "IX_ProjectUpdates_PostedById",
                table: "ProjectUpdates");

            migrationBuilder.DropColumn(
                name: "PostedById",
                table: "ProjectUpdates");

            migrationBuilder.AddColumn<Guid>(
                name: "EditedById",
                table: "ProjectUpdates",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("9b4261aa-6a0b-4664-b076-1bc3dd0476a3"), "97fe714d-b337-4044-8a83-3f92f444fe78", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUpdates_EditedById",
                table: "ProjectUpdates",
                column: "EditedById",
                unique: true,
                filter: "[EditedById] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUpdates_Users_EditedById",
                table: "ProjectUpdates",
                column: "EditedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUpdates_Users_Id",
                table: "ProjectUpdates",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
