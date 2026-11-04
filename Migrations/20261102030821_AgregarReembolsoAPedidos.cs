using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaginaVentasNet.Api.Migrations
{
    /// <inheritdoc />
    public partial class AgregarReembolsoAPedidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefundStatus",
                table: "Orders",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "NotRequired");

            migrationBuilder.AddColumn<DateTime>(
                name: "RefundedAtUtc",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            // Los pedidos ya cancelados que tienen un pago aprobado (y ninguna anulación)
            // quedan con la devolución pendiente. El resto queda en NotRequired.
            migrationBuilder.Sql("""
                UPDATE "Orders" o
                SET "RefundStatus" = 'Pending'
                WHERE o."Status" = 'Cancelado'
                  AND EXISTS (
                      SELECT 1 FROM "PaymentTransactions" pt
                      WHERE pt."OrderId" = o."Id" AND pt."Status" = 'APPROVED')
                  AND NOT EXISTS (
                      SELECT 1 FROM "PaymentTransactions" pt
                      WHERE pt."OrderId" = o."Id" AND pt."Status" = 'VOIDED');
                """);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_RefundStatus",
                table: "Orders",
                sql: "\"RefundStatus\" = 'NotRequired' OR \"Status\" = 'Cancelado'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_RefundStatus",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RefundStatus",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RefundedAtUtc",
                table: "Orders");
        }
    }
}
