using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fcg.Data.Migrations
{
    /// <inheritdoc />
    public partial class v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_Games_GameId",
                table: "Promotions");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$12$4v7rJsBJeZZ.Zivh/iTab.SGuxcNFWOOzKqZ34scDghOlmw3ImV3S");

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_Games_GameId",
                table: "Promotions",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_Games_GameId",
                table: "Promotions");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2b$12$qEZ5FkLS2D2DCOAuWJPemOUm8iWvU.SMTFRhJIcHPb.N5OQbnMi3W");

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_Games_GameId",
                table: "Promotions",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");
        }
    }
}
