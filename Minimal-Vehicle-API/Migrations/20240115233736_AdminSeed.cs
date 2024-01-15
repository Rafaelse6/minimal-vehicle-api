using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minimal_Vehicle_API.Migrations
{
    /// <inheritdoc />
    public partial class AdminSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "Id", "Email", "Password", "Profile" },
                values: new object[] { 1, "admin@teste.com", "123456", "Adm" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
