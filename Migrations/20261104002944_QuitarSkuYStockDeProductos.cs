using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaginaVentasNet.Api.Migrations
{
    /// <inheritdoc />
    public partial class QuitarSkuYStockDeProductos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            // Antes de borrar las columnas, todo producto debe tener su variante: ahí viven
            // ahora el SKU y el stock. Hoy ningún producto la necesita, pero así la migración
            // es reproducible sobre cualquier base y no se pierde el stock de un producto sin variante.
            migrationBuilder.Sql("""
                INSERT INTO "ProductVariants" ("ProductId", "SizeId", "ColorId", "Sku", "Stock", "IsActive")
                SELECT p."Id", NULL, NULL, p."Sku", p."Stock", true
                FROM "Products" p
                WHERE NOT EXISTS (SELECT 1 FROM "ProductVariants" v WHERE v."ProductId" = p."Id");
                """);

            migrationBuilder.DropIndex(
                name: "IX_Products_Sku",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Sku",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sku",
                table: "Products",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Un SKU provisional único para todo producto. Así, si alguno no tuviera variante,
            // el índice único no choca por SKU vacíos.
            migrationBuilder.Sql("""
                UPDATE "Products" p SET "Sku" = 'PRODUCTO-' || p."Id";
                """);

            // El SKU real: el de la primera variante de cada producto.
            migrationBuilder.Sql("""
                UPDATE "Products" p
                SET "Sku" = v."Sku"
                FROM (
                    SELECT DISTINCT ON ("ProductId") "ProductId", "Sku"
                    FROM "ProductVariants"
                    ORDER BY "ProductId", "Id"
                ) v
                WHERE v."ProductId" = p."Id";
                """);

            // El stock: la suma de las variantes activas.
            migrationBuilder.Sql("""
                UPDATE "Products" p
                SET "Stock" = COALESCE((
                    SELECT SUM(v."Stock") FROM "ProductVariants" v
                    WHERE v."ProductId" = p."Id" AND v."IsActive"), 0);
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Sku",
                table: "Products",
                column: "Sku",
                unique: true);
        }
    }
}
