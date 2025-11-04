using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fcg.Data.Migrations
{
    /// <inheritdoc />
    public partial class CorrigeMudancasPendentes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 1,
                column: "ReleaseDate",
                value: new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Promotions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2025, 10, 13, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 1,
                column: "ReleaseDate",
                value: new DateTime(2025, 10, 10, 3, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Promotions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2025, 10, 13, 3, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 10, 10, 3, 0, 0, 0, DateTimeKind.Utc) });
        }
    }
}
