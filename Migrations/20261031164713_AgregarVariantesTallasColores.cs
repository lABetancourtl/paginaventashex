using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PaginaVentasNet.Api.Migrations
{
    /// <inheritdoc />
    public partial class AgregarVariantesTallasColores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    HexCode = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sizes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SizeGroup = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sizes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    SizeId = table.Column<int>(type: "integer", nullable: true),
                    ColorId = table.Column<int>(type: "integer", nullable: true),
                    Sku = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariants", x => x.Id);
                    table.CheckConstraint("CK_ProductVariants_Stock", "\"Stock\" >= 0");
                    table.ForeignKey(
                        name: "FK_ProductVariants_Colors_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Colors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Sizes_SizeId",
                        column: x => x.SizeId,
                        principalTable: "Sizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Colors_Name",
                table: "Colors",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ColorId",
                table: "ProductVariants",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ProductId_SizeId_ColorId",
                table: "ProductVariants",
                columns: new[] { "ProductId", "SizeId", "ColorId" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_SizeId",
                table: "ProductVariants",
                column: "SizeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_Sku",
                table: "ProductVariants",
                column: "Sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sizes_SizeGroup_DisplayOrder",
                table: "Sizes",
                columns: new[] { "SizeGroup", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Sizes_SizeGroup_Name",
                table: "Sizes",
                columns: new[] { "SizeGroup", "Name" },
                unique: true);

            
            // Tallas iniciales: 7 de bebé, 8 de niño y 5 de adulto. El admin las edita después.
            migrationBuilder.Sql("""
                INSERT INTO "Sizes" ("SizeGroup", "Name", "DisplayOrder", "IsActive") VALUES
                    ('Bebe', 'RN', 1, true),
                    ('Bebe', '0-3 meses', 2, true),
                    ('Bebe', '3-6 meses', 3, true),
                    ('Bebe', '6-9 meses', 4, true),
                    ('Bebe', '9-12 meses', 5, true),
                    ('Bebe', '12-18 meses', 6, true),
                    ('Bebe', '18-24 meses', 7, true),
                    ('Nino', '2', 1, true),
                    ('Nino', '3', 2, true),
                    ('Nino', '4', 3, true),
                    ('Nino', '5', 4, true),
                    ('Nino', '6', 5, true),
                    ('Nino', '8', 6, true),
                    ('Nino', '10', 7, true),
                    ('Nino', '12', 8, true),
                    ('Adulto', 'XS', 1, true),
                    ('Adulto', 'S', 2, true),
                    ('Adulto', 'M', 3, true),
                    ('Adulto', 'L', 4, true),
                    ('Adulto', 'XL', 5, true);
                """);

            // Una variante sin talla ni color por cada producto existente,
            // con el mismo SKU y el mismo stock que tiene hoy el producto.
            migrationBuilder.Sql("""
                INSERT INTO "ProductVariants" ("ProductId", "SizeId", "ColorId", "Sku", "Stock", "IsActive")
                SELECT "Id", NULL, NULL, "Sku", "Stock", true
                FROM "Products";
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductVariants");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropTable(
                name: "Sizes");
        }
    }
}
