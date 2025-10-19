using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fcg.Data.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2b$12$qEZ5FkLS2D2DCOAuWJPemOUm8iWvU.SMTFRhJIcHPb.N5OQbnMi3W");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "User@1234");
        }
    }
}
