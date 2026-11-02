using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaginaVentasNet.Api.Migrations
{
    /// <inheritdoc />
    public partial class AgregarVarianteYPrecioBaseAPedidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice",
                table: "OrderItems",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ColorName",
                table: "OrderItems",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiscountPercent",
                table: "OrderItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SizeName",
                table: "OrderItems",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sku",
                table: "OrderItems",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VariantId",
                table: "OrderItems",
                type: "integer",
                nullable: true);

            // Los items anteriores a los descuentos pagaron el precio base completo.
            migrationBuilder.Sql("""
                UPDATE "OrderItems" SET "BasePrice" = "ProductPrice";
                """);

            // El valor por defecto solo servía para crear la columna. Se quita para que
            // ninguna fila futura pueda quedar con precio base 0 sin que el dominio lo ponga.
            migrationBuilder.Sql("""
                ALTER TABLE "OrderItems" ALTER COLUMN "BasePrice" DROP DEFAULT;
                """);

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrderItems_DiscountPercent",
                table: "OrderItems",
                sql: "\"DiscountPercent\" IS NULL OR (\"DiscountPercent\" BETWEEN 1 AND 99)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_OrderItems_DiscountPercent",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "BasePrice",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ColorName",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "SizeName",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Sku",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "VariantId",
                table: "OrderItems");
        }
    }
}
