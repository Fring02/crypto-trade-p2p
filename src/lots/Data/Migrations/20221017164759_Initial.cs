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
                name: "PaymentsDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    UserEmail = table.Column<string>(type: "text", nullable: false),
                    BankName = table.Column<string>(type: "text", nullable: false),
                    CreditCardRequisite = table.Column<string>(type: "text", nullable: true),
                    PhoneRequisite = table.Column<string>(type: "text", nullable: true),
                    PaymentType = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentsDetails", x => x.Id);
                    table.CheckConstraint("CK_PaymentsDetails_PaymentType_Enum", "\"PaymentType\" IN (0, 1)");
                });

            migrationBuilder.CreateTable(
                name: "Lots",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    OwnerEmail = table.Column<string>(type: "text", nullable: false),
                    OwnerWallet = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    Supply = table.Column<double>(type: "double precision", nullable: false),
                    MinLimit = table.Column<double>(type: "double precision", nullable: false),
                    MaxLimit = table.Column<double>(type: "double precision", nullable: false),
                    FiatType = table.Column<int>(type: "integer", nullable: false),
                    CryptoType = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    PaymentDetailsId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lots", x => x.Id);
                    table.CheckConstraint("CK_Lots_CryptoType_Enum", "\"CryptoType\" IN (0, 1)");
                    table.CheckConstraint("CK_Lots_FiatType_Enum", "\"FiatType\" IN (0, 1, 2)");
                    table.CheckConstraint("CK_Lots_Type_Enum", "\"Type\" IN (0, 1)");
                    table.ForeignKey(
                        name: "FK_Lots_PaymentsDetails_PaymentDetailsId",
                        column: x => x.PaymentDetailsId,
                        principalTable: "PaymentsDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lots_PaymentDetailsId",
                table: "Lots",
                column: "PaymentDetailsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lots");

            migrationBuilder.DropTable(
                name: "PaymentsDetails");
        }
    }
}
