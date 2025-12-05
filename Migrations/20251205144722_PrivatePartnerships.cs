using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class PrivatePartnerships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 5, 14, 47, 19, 293, DateTimeKind.Utc).AddTicks(9114), "$2a$11$fzQ.T0/xaQue0h5UmcaPteMXtM69T3x0YiAR7rwqC.vsQj1G9SO/m", new DateTime(2025, 12, 5, 14, 47, 19, 293, DateTimeKind.Utc).AddTicks(9122) });
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
