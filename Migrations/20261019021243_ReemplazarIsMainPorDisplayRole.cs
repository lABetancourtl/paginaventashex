using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaginaVentasNet.Api.Migrations
{
    public partial class ReemplazarIsMainPorDisplayRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "ProductMedia");

            migrationBuilder.AddColumn<string>(
                name: "DisplayRole",
                table: "ProductMedia",
                type: "text",
                nullable: false,
                defaultValue: "None");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayRole",
                table: "ProductMedia");

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "ProductMedia",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
