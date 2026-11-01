using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaginaVentasNet.Api.Migrations
{
    /// <inheritdoc />
    public partial class AgregarVarianteAlCarrito : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CartItems_CartId_ProductId",
                table: "CartItems");

            migrationBuilder.AddColumn<int>(
                name: "VariantId",
                table: "CartItems",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "CartItems" ci
                SET "VariantId" = (
                    SELECT MIN(v."Id")
                    FROM "ProductVariants" v
                    WHERE v."ProductId" = ci."ProductId"
                );
                """);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId_VariantId",
                table: "CartItems",
                columns: new[] { "CartId", "VariantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_VariantId",
                table: "CartItems",
                column: "VariantId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_ProductVariants_VariantId",
                table: "CartItems",
                column: "VariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_ProductVariants_VariantId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_CartId_VariantId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_VariantId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "VariantId",
                table: "CartItems");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId_ProductId",
                table: "CartItems",
                columns: new[] { "CartId", "ProductId" },
                unique: true);
        }
    }
}
