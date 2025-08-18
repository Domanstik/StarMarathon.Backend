using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarMarathon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Idexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Employees_TgId",
                table: "Employees",
                column: "TgId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_TgId",
                table: "Employees");
        }
    }
}
