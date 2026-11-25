using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaginaVentasNet.Api.Migrations
{
    /// <inheritdoc />
    public partial class AgregarImagenCelularABanner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MobilePublicId",
                table: "HomeSectionItems",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileUrl",
                table: "HomeSectionItems",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MobilePublicId",
                table: "HomeSectionItems");

            migrationBuilder.DropColumn(
                name: "MobileUrl",
                table: "HomeSectionItems");
        }
    }
}
