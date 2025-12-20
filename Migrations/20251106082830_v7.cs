using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    dateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactPayments_ContractOfContracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "ContractOfContracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 6, 8, 28, 29, 215, DateTimeKind.Utc).AddTicks(5918), "$2a$11$laTUUgMGML8C3K2Pxz9HOe2AxfjGUPP/hj9w10BZnIfDn66Gg9g9e", new DateTime(2025, 11, 6, 8, 28, 29, 215, DateTimeKind.Utc).AddTicks(5925) });

            migrationBuilder.CreateIndex(
                name: "IX_ContactPayments_ContractId",
                table: "ContactPayments",
                column: "ContractId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactPayments");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 5, 10, 16, 15, 287, DateTimeKind.Utc).AddTicks(4474), "$2a$11$c16jdX1V07awyFfrSQC2VOBjnrDIp92WPlVG.MyNjbfMIZzhyIalG", new DateTime(2025, 11, 5, 10, 16, 15, 287, DateTimeKind.Utc).AddTicks(4480) });
        }
    }
}
