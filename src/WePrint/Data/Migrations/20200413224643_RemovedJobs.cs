using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WePrint.Data.Migrations
{
    public partial class RemovedJobs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bid_Jobs_JobId",
                table: "Bid");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Bid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Edited = table.Column<bool>(type: "bit", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_Comment_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Comment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comment_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcceptedBidId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BidClose = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaterialColor = table.Column<int>(type: "int", nullable: true),
                    MaterialType = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrinterType = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Address_AddressLine1 = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Address_AddressLine2 = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Address_AddressLine3 = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Address_Attention = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Address_City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address_State = table.Column<string>(type: "nchar(2)", maxLength: 2, nullable: true),
                    Address_ZipCode = table.Column<string>(type: "nchar(5)", maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jobs_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Bid",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Accepted = table.Column<bool>(type: "bit", nullable: false),
                    BidderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FillPercentage = table.Column<decimal>(type: "decimal(6,3)", nullable: false),
                    Finishing = table.Column<int>(type: "int", nullable: false),
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LayerHeight = table.Column<decimal>(type: "decimal(6,3)", nullable: false),
                    MaterialColor = table.Column<int>(type: "int", nullable: false),
                    MaterialType = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Price = table.Column<decimal>(type: "smallmoney", nullable: false),
                    ShellThickness = table.Column<decimal>(type: "decimal(6,3)", nullable: false),
                    WorkTime = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bid", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bid_Users_BidderId",
                        column: x => x.BidderId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Bid_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    ReviewedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Review_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Review_Users_ReviewedUserId",
                        column: x => x.ReviewedUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Review_Users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bid_BidderId",
                table: "Bid",
                column: "BidderId");

            migrationBuilder.CreateIndex(
                name: "IX_Bid_JobId",
                table: "Bid",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ParentId",
                table: "Comment",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_UserId",
                table: "Comment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_AcceptedBidId",
                table: "Jobs",
                column: "AcceptedBidId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_CustomerId",
                table: "Jobs",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_JobId",
                table: "Review",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_ReviewedUserId",
                table: "Review",
                column: "ReviewedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_ReviewerId",
                table: "Review",
                column: "ReviewerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Bid_AcceptedBidId",
                table: "Jobs",
                column: "AcceptedBidId",
                principalTable: "Bid",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
