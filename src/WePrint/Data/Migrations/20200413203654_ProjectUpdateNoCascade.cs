using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WePrint.Data.Migrations
{
    public partial class ProjectUpdateNoCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pledge_Users_MakerId",
                table: "Pledge");

            migrationBuilder.DropForeignKey(
                name: "FK_Pledge_Projects_ProjectId",
                table: "Pledge");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUpdates_Users_PostedById",
                table: "ProjectUpdates");

            migrationBuilder.DropIndex(
                name: "IX_ProjectUpdates_PostedById",
                table: "ProjectUpdates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pledge",
                table: "Pledge");

            migrationBuilder.DropColumn(
                name: "PostedById",
                table: "ProjectUpdates");

            migrationBuilder.DropColumn(
                name: "Address_AddressLine1",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Address_AddressLine1",
                table: "Jobs");

            migrationBuilder.RenameTable(
                name: "Pledge",
                newName: "Pledges");

            migrationBuilder.RenameIndex(
                name: "IX_Pledge_ProjectId",
                table: "Pledges",
                newName: "IX_Pledges_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Pledge_MakerId",
                table: "Pledges",
                newName: "IX_Pledges_MakerId");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EditTimestamp",
                table: "ProjectUpdates",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "EditedById",
                table: "ProjectUpdates",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Projects",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_AddressLine4",
                table: "Projects",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Attention",
                table: "Projects",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Organizations",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<string>(
                name: "Address_AddressLine2",
                table: "Organizations",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_AddressLine3",
                table: "Organizations",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_AddressLine4",
                table: "Organizations",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Attention",
                table: "Organizations",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                table: "Organizations",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_State",
                table: "Organizations",
                type: "nchar(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_ZipCode",
                table: "Organizations",
                type: "nchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_AddressLine4",
                table: "Jobs",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Attention",
                table: "Jobs",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pledges",
                table: "Pledges",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUpdates_EditedById",
                table: "ProjectUpdates",
                column: "EditedById",
                unique: true,
                filter: "[EditedById] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Pledges_Users_MakerId",
                table: "Pledges",
                column: "MakerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pledges_Projects_ProjectId",
                table: "Pledges",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pledges_Users_MakerId",
                table: "Pledges");

            migrationBuilder.DropForeignKey(
                name: "FK_Pledges_Projects_ProjectId",
                table: "Pledges");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUpdates_Users_EditedById",
                table: "ProjectUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUpdates_Users_Id",
                table: "ProjectUpdates");

            migrationBuilder.DropIndex(
                name: "IX_ProjectUpdates_EditedById",
                table: "ProjectUpdates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pledges",
                table: "Pledges");

            migrationBuilder.DropColumn(
                name: "EditTimestamp",
                table: "ProjectUpdates");

            migrationBuilder.DropColumn(
                name: "EditedById",
                table: "ProjectUpdates");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Address_AddressLine4",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Address_Attention",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Address_AddressLine2",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Address_AddressLine3",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Address_AddressLine4",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Address_Attention",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Address_City",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Address_State",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Address_ZipCode",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Address_AddressLine4",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Address_Attention",
                table: "Jobs");

            migrationBuilder.RenameTable(
                name: "Pledges",
                newName: "Pledge");

            migrationBuilder.RenameIndex(
                name: "IX_Pledges_ProjectId",
                table: "Pledge",
                newName: "IX_Pledge_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Pledges_MakerId",
                table: "Pledge",
                newName: "IX_Pledge_MakerId");

            migrationBuilder.AddColumn<Guid>(
                name: "PostedById",
                table: "ProjectUpdates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Address_AddressLine1",
                table: "Projects",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Organizations",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 4000);

            migrationBuilder.AddColumn<string>(
                name: "Address_AddressLine1",
                table: "Jobs",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pledge",
                table: "Pledge",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUpdates_PostedById",
                table: "ProjectUpdates",
                column: "PostedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Pledge_Users_MakerId",
                table: "Pledge",
                column: "MakerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pledge_Projects_ProjectId",
                table: "Pledge",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUpdates_Users_PostedById",
                table: "ProjectUpdates",
                column: "PostedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
