using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Firstname = table.Column<string>(type: "text", nullable: false),
                    Lastname = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Hash = table.Column<byte[]>(type: "bytea", nullable: false),
                    Salt = table.Column<byte[]>(type: "bytea", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshExpires = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequisiteDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PhoneNumber = table.Column<string>(type: "character(12)", fixedLength: true, maxLength: 12, nullable: true),
                    CreditCardNumber = table.Column<string>(type: "character(16)", fixedLength: true, maxLength: 16, nullable: true),
                    BankName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisiteDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequisiteDetails_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Firstname", "Hash", "Lastname", "RefreshExpires", "RefreshToken", "Salt" },
                values: new object[] { new Guid("c9fc369d-d0ca-4709-9494-d792ecbc1cc6"), "admin@gmail.com", "admin", new byte[] { 37, 156, 226, 181, 6, 224, 205, 132, 131, 82, 229, 83, 214, 250, 77, 235, 220, 179, 91, 109, 131, 59, 197, 59, 36, 80, 74, 118, 23, 113, 90, 206 }, "admin", null, null, new byte[] { 104, 213, 252, 245, 152, 207, 147, 253, 225, 75, 150, 144, 129, 66, 53, 69, 196, 53, 214, 93, 15, 121, 99, 215, 134, 133, 122, 32, 13, 147, 23, 159, 4, 195, 10, 226, 157, 28, 191, 199, 68, 19, 48, 89, 36, 37, 237, 228, 232, 18, 4, 92, 90, 138, 19, 20, 157, 35, 239, 233, 254, 31, 25, 122 } });

            migrationBuilder.InsertData(
                table: "RequisiteDetails",
                columns: new[] { "Id", "BankName", "CreditCardNumber", "PhoneNumber", "UserId" },
                values: new object[] { 1L, "Kaspi", "1234567890123456", "+77761667060", new Guid("c9fc369d-d0ca-4709-9494-d792ecbc1cc6") });

            migrationBuilder.CreateIndex(
                name: "IX_RequisiteDetails_UserId",
                table: "RequisiteDetails",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequisiteDetails");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
