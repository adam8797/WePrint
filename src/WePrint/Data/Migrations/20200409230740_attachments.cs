using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WePrint.Data.Migrations
{
    public partial class attachments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Printer_AspNetUsers_OwnerId",
                table: "Printer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Printer",
                table: "Printer");

            migrationBuilder.RenameTable(
                name: "Printer",
                newName: "Printers");

            migrationBuilder.RenameIndex(
                name: "IX_Printer_OwnerId",
                table: "Printers",
                newName: "IX_Printers_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Printers",
                table: "Printers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "JobAttachment",
                columns: table => new
                {
                    OwnerId = table.Column<Guid>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmittedById = table.Column<Guid>(nullable: false),
                    URL = table.Column<string>(maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobAttachment", x => new { x.OwnerId, x.Id });
                    table.ForeignKey(
                        name: "FK_JobAttachment_Jobs_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobAttachment_AspNetUsers_SubmittedById",
                        column: x => x.SubmittedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectAttachment",
                columns: table => new
                {
                    OwnerId = table.Column<Guid>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmittedById = table.Column<Guid>(nullable: false),
                    URL = table.Column<string>(maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAttachment", x => new { x.OwnerId, x.Id });
                    table.ForeignKey(
                        name: "FK_ProjectAttachment_Projects_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectAttachment_AspNetUsers_SubmittedById",
                        column: x => x.SubmittedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobAttachment_SubmittedById",
                table: "JobAttachment",
                column: "SubmittedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAttachment_SubmittedById",
                table: "ProjectAttachment",
                column: "SubmittedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Printers_AspNetUsers_OwnerId",
                table: "Printers",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Printers_AspNetUsers_OwnerId",
                table: "Printers");

            migrationBuilder.DropTable(
                name: "JobAttachment");

            migrationBuilder.DropTable(
                name: "ProjectAttachment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Printers",
                table: "Printers");

            migrationBuilder.RenameTable(
                name: "Printers",
                newName: "Printer");

            migrationBuilder.RenameIndex(
                name: "IX_Printers_OwnerId",
                table: "Printer",
                newName: "IX_Printer_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Printer",
                table: "Printer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Printer_AspNetUsers_OwnerId",
                table: "Printer",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
