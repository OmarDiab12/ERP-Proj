using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class PrivatePartnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 5, 14, 51, 13, 472, DateTimeKind.Utc).AddTicks(7986), "$2a$11$tdXasxqO4qZn8HQghHnoCeam9NRaYjEgGFbjKhuHHn72W3Igy.bE6", new DateTime(2025, 12, 5, 14, 51, 13, 472, DateTimeKind.Utc).AddTicks(7992) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 2, 0, 1, 32, 13, DateTimeKind.Utc).AddTicks(4784), "$2a$11$YCoxt1tuGTYn4vCZWbx.N.PZuoL2TMCPoZfFOyv206epuBGSt.hvi", new DateTime(2025, 12, 2, 0, 1, 32, 13, DateTimeKind.Utc).AddTicks(4789) });
        }
    }
}
