using Microsoft.EntityFrameworkCore.Migrations;

namespace ShoppingWarehouse.Data.Migrations
{
    public partial class AddRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "8dd572b6-45df-42fa-8e02-0b1c210795f8", "081752f7-4a42-40ca-bcd8-cdef39d2dd82", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "7fe65fa3-45a6-4fb1-9285-02f69b8f9e7f", "37e5910f-7d68-4f42-9df7-8dbc01eb7f8f", "User", "USER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7fe65fa3-45a6-4fb1-9285-02f69b8f9e7f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8dd572b6-45df-42fa-8e02-0b1c210795f8");
        }
    }
}
