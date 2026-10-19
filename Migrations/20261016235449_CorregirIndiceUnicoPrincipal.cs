using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaginaVentasNet.Api.Migrations
{
    public partial class CorregirIndiceUnicoPrincipal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Direcciones_UsuarioId",
                table: "Direcciones");

            migrationBuilder.Sql(
                @"CREATE UNIQUE INDEX ""IX_Direcciones_UsuarioId_Principal"" 
                  ON ""Direcciones"" (""UsuarioId"") 
                  WHERE ""EsPrincipal"" = true");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"DROP INDEX ""IX_Direcciones_UsuarioId_Principal""");

            migrationBuilder.CreateIndex(
                name: "IX_Direcciones_UsuarioId",
                table: "Direcciones",
                column: "UsuarioId",
                unique: true);
        }
    }
}
