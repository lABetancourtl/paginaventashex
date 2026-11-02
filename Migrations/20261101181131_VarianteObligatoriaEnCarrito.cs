using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaginaVentasNet.Api.Migrations
{
    /// <inheritdoc />
    public partial class VarianteObligatoriaEnCarrito : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "VariantId",
                table: "CartItems",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "VariantId",
                table: "CartItems",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
