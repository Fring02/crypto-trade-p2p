using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    public partial class AddUserRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c9fc369d-d0ca-4709-9494-d792ecbc1cc6"));

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Firstname", "Hash", "Lastname", "RefreshExpires", "RefreshToken", "Role", "Salt" },
                values: new object[] { new Guid("bf9519e7-67c6-4ed6-96f6-17d93dd34a6b"), "admin@gmail.com", "admin", new byte[] { 218, 200, 113, 4, 156, 195, 71, 192, 230, 45, 96, 83, 135, 204, 199, 67, 118, 222, 105, 185, 255, 22, 39, 18, 29, 92, 108, 11, 112, 206, 95, 51 }, "admin", null, null, "admin", new byte[] { 203, 236, 166, 216, 206, 181, 98, 217, 58, 128, 238, 139, 251, 210, 32, 76, 198, 103, 26, 195, 215, 211, 24, 33, 160, 230, 164, 220, 109, 67, 173, 120, 102, 123, 55, 164, 20, 176, 147, 44, 153, 239, 66, 44, 45, 213, 36, 11, 139, 166, 182, 63, 237, 93, 49, 221, 88, 146, 58, 97, 45, 112, 239, 10 } });

            migrationBuilder.UpdateData(
                table: "RequisiteDetails",
                keyColumn: "Id",
                keyValue: 1L,
                column: "UserId",
                value: new Guid("bf9519e7-67c6-4ed6-96f6-17d93dd34a6b"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("bf9519e7-67c6-4ed6-96f6-17d93dd34a6b"));

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Firstname", "Hash", "Lastname", "RefreshExpires", "RefreshToken", "Salt" },
                values: new object[] { new Guid("c9fc369d-d0ca-4709-9494-d792ecbc1cc6"), "admin@gmail.com", "admin", new byte[] { 37, 156, 226, 181, 6, 224, 205, 132, 131, 82, 229, 83, 214, 250, 77, 235, 220, 179, 91, 109, 131, 59, 197, 59, 36, 80, 74, 118, 23, 113, 90, 206 }, "admin", null, null, new byte[] { 104, 213, 252, 245, 152, 207, 147, 253, 225, 75, 150, 144, 129, 66, 53, 69, 196, 53, 214, 93, 15, 121, 99, 215, 134, 133, 122, 32, 13, 147, 23, 159, 4, 195, 10, 226, 157, 28, 191, 199, 68, 19, 48, 89, 36, 37, 237, 228, 232, 18, 4, 92, 90, 138, 19, 20, 157, 35, 239, 233, 254, 31, 25, 122 } });

            migrationBuilder.UpdateData(
                table: "RequisiteDetails",
                keyColumn: "Id",
                keyValue: 1L,
                column: "UserId",
                value: new Guid("c9fc369d-d0ca-4709-9494-d792ecbc1cc6"));
        }
    }
}
