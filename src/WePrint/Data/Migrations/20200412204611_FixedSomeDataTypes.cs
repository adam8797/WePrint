using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WePrint.Data.Migrations
{
    public partial class FixedSomeDataTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Updates_Users_PostedById",
                table: "Updates");

            migrationBuilder.DropForeignKey(
                name: "FK_Updates_Projects_ProjectId",
                table: "Updates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Updates",
                table: "Updates");

            migrationBuilder.DropColumn(
                name: "DeliveryEstimate",
                table: "Pledge");

            migrationBuilder.RenameTable(
                name: "Updates",
                newName: "ProjectUpdates");

            migrationBuilder.RenameIndex(
                name: "IX_Updates_ProjectId",
                table: "ProjectUpdates",
                newName: "IX_ProjectUpdates_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Updates_PostedById",
                table: "ProjectUpdates",
                newName: "IX_ProjectUpdates_PostedById");

            migrationBuilder.AlterColumn<string>(
                name: "ShippingInstructions",
                table: "Projects",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PrintingInstructions",
                table: "Projects",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Projects",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeliveryDate",
                table: "Pledge",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<string>(
                name: "Body",
                table: "ProjectUpdates",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectUpdates",
                table: "ProjectUpdates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUpdates_Users_PostedById",
                table: "ProjectUpdates",
                column: "PostedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUpdates_Projects_ProjectId",
                table: "ProjectUpdates",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUpdates_Users_PostedById",
                table: "ProjectUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUpdates_Projects_ProjectId",
                table: "ProjectUpdates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectUpdates",
                table: "ProjectUpdates");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "Pledge");

            migrationBuilder.RenameTable(
                name: "ProjectUpdates",
                newName: "Updates");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectUpdates_ProjectId",
                table: "Updates",
                newName: "IX_Updates_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectUpdates_PostedById",
                table: "Updates",
                newName: "IX_Updates_PostedById");

            migrationBuilder.AlterColumn<string>(
                name: "ShippingInstructions",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PrintingInstructions",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 4000);

            migrationBuilder.AddColumn<long>(
                name: "DeliveryEstimate",
                table: "Pledge",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "Body",
                table: "Updates",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 4000);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Updates",
                table: "Updates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Updates_Users_PostedById",
                table: "Updates",
                column: "PostedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Updates_Projects_ProjectId",
                table: "Updates",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }
    }
}
