using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class emps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 22, 22, 22, 13, 429, DateTimeKind.Utc).AddTicks(6990), "$2a$11$3uvCMA.eJoaGG.SDTSsknuW3201tmtMYEfu9bwucUeokCRV0dN9RS", new DateTime(2025, 9, 22, 22, 22, 13, 429, DateTimeKind.Utc).AddTicks(6993) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 22, 22, 21, 21, 19, DateTimeKind.Utc).AddTicks(8944), "$2a$11$bE2WLGoS2TRw10R/EW8uQuIcB5bG8SGpcqU2qxti4BGEctjMGAUMO", new DateTime(2025, 9, 22, 22, 21, 21, 19, DateTimeKind.Utc).AddTicks(8949) });
        }
    }
}
