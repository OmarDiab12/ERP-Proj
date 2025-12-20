using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class PrivatePartnershipTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 5, 15, 3, 53, 906, DateTimeKind.Utc).AddTicks(1538), "$2a$11$zCdcY/FpYTZICGCZ2FKr/Ov3BsYe9fwZOMxRMz82EwRsbHoG.FK/u", new DateTime(2025, 12, 5, 15, 3, 53, 906, DateTimeKind.Utc).AddTicks(1544) });
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
