using System;
using ERP.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ERPDBContext))]
    [Migration("20251205001000_PrivatePartnershipsModule", "8.0.19")]
    public partial class PrivatePartnershipsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrivatePartnershipProjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivatePartnershipProjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrivatePartnershipPartnerShares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    PartnerName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ContributionPercentage = table.Column<double>(type: "float(5)", precision: 5, scale: 2, nullable: false),
                    ContributionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivatePartnershipPartnerShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivatePartnershipPartnerShares_PrivatePartnershipProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "PrivatePartnershipProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrivatePartnershipTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    PartnerShareId = table.Column<int>(type: "int", nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivatePartnershipTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivatePartnershipTransactions_PrivatePartnershipPartnerShares_PartnerShareId",
                        column: x => x.PartnerShareId,
                        principalTable: "PrivatePartnershipPartnerShares",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PrivatePartnershipTransactions_PrivatePartnershipProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "PrivatePartnershipProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrivatePartnershipPartnerShares_ProjectId",
                table: "PrivatePartnershipPartnerShares",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivatePartnershipTransactions_PartnerShareId",
                table: "PrivatePartnershipTransactions",
                column: "PartnerShareId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivatePartnershipTransactions_ProjectId",
                table: "PrivatePartnershipTransactions",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrivatePartnershipTransactions");

            migrationBuilder.DropTable(
                name: "PrivatePartnershipPartnerShares");

            migrationBuilder.DropTable(
                name: "PrivatePartnershipProjects");
        }
    }
}
