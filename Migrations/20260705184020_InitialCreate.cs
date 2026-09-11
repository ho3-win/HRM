using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mhrm.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "HireDate",
                value: new DateTime(2026, 7, 5, 22, 10, 20, 167, DateTimeKind.Local).AddTicks(3223));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "HireDate",
                value: new DateTime(2026, 6, 21, 17, 52, 20, 355, DateTimeKind.Local).AddTicks(9391));
        }
    }
}
