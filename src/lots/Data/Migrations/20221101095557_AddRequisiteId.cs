using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    public partial class AddRequisiteId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lots_PaymentsDetails_PaymentDetailsId",
                table: "Lots");

            migrationBuilder.DropTable(
                name: "PaymentsDetails");

            migrationBuilder.DropIndex(
                name: "IX_Lots_PaymentDetailsId",
                table: "Lots");

            migrationBuilder.RenameColumn(
                name: "PaymentDetailsId",
                table: "Lots",
                newName: "RequisiteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequisiteId",
                table: "Lots",
                newName: "PaymentDetailsId");

            migrationBuilder.CreateTable(
                name: "PaymentsDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    BankName = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreditCardRequisite = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaymentType = table.Column<int>(type: "integer", nullable: false),
                    PhoneRequisite = table.Column<string>(type: "text", nullable: true),
                    UserEmail = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentsDetails", x => x.Id);
                    table.CheckConstraint("CK_PaymentsDetails_PaymentType_Enum", "\"PaymentType\" IN (0, 1)");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lots_PaymentDetailsId",
                table: "Lots",
                column: "PaymentDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lots_PaymentsDetails_PaymentDetailsId",
                table: "Lots",
                column: "PaymentDetailsId",
                principalTable: "PaymentsDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
