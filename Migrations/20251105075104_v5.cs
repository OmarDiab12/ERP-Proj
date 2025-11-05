using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "location",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "fileName",
                table: "OperationalExpenses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "filePath",
                table: "OperationalExpenses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ContractOfContracts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "ContractOfContracts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "ContractOfContracts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 5, 7, 51, 3, 236, DateTimeKind.Utc).AddTicks(5497), "$2a$11$mI3FGIH5bIMxcta5XEfc8epln564jcfZLnoSTDk7UqZCdixhRCEK.", new DateTime(2025, 11, 5, 7, 51, 3, 236, DateTimeKind.Utc).AddTicks(5506) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "location",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "fileName",
                table: "OperationalExpenses");

            migrationBuilder.DropColumn(
                name: "filePath",
                table: "OperationalExpenses");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ContractOfContracts");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "ContractOfContracts");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "ContractOfContracts");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 31, 11, 41, 7, 704, DateTimeKind.Utc).AddTicks(2194), "$2a$11$cbH57kcGzeWUoEJKcXRmeOog5uJv0uirKJc2mokKyxhf/dmoc1Oxy", new DateTime(2025, 10, 31, 11, 41, 7, 704, DateTimeKind.Utc).AddTicks(2198) });
        }
    }
}
