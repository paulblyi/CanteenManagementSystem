using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanteenApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Email" },
                values: new object[] { new DateTime(2026, 7, 27, 15, 39, 18, 594, DateTimeKind.Utc).AddTicks(4832), "admin@masimba.co.zw" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Email" },
                values: new object[] { new DateTime(2026, 7, 7, 13, 34, 27, 783, DateTimeKind.Utc).AddTicks(9398), "admin@canteen.com" });
        }
    }
}
