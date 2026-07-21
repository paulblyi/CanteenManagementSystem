using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CanteenApi.Migrations
{
    /// <inheritdoc />
    public partial class FixDateTimeKind : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 7, 13, 34, 27, 783, DateTimeKind.Utc).AddTicks(9398));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 7, 1, 9, 22, 18, 667, DateTimeKind.Utc).AddTicks(2107));
        }
    }
}
