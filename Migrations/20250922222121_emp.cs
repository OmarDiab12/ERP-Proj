using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class emp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 22, 22, 21, 21, 19, DateTimeKind.Utc).AddTicks(8944), "$2a$11$bE2WLGoS2TRw10R/EW8uQuIcB5bG8SGpcqU2qxti4BGEctjMGAUMO", new DateTime(2025, 9, 22, 22, 21, 21, 19, DateTimeKind.Utc).AddTicks(8949) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 9, 22, 22, 17, 16, 106, DateTimeKind.Utc).AddTicks(621), "$2a$11$G.rwAqxxnXixOgBTrblj3OuEIVfz9jUCPhG8NhgKDYNnfEKtGKznW", new DateTime(2025, 9, 22, 22, 17, 16, 106, DateTimeKind.Utc).AddTicks(625) });
        }
    }
}
