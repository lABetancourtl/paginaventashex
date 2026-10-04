using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaginaVentasNet.Api.Migrations
{
    /// <inheritdoc />
    public partial class CambiarRolAEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Rol",
                table: "Usuarios",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_Direcciones_DepartamentoCodigo",
                table: "Direcciones",
                column: "DepartamentoCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_Direcciones_MunicipioCodigo",
                table: "Direcciones",
                column: "MunicipioCodigo");

            migrationBuilder.AddForeignKey(
                name: "FK_Direcciones_Departamentos_DepartamentoCodigo",
                table: "Direcciones",
                column: "DepartamentoCodigo",
                principalTable: "Departamentos",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Direcciones_Municipios_MunicipioCodigo",
                table: "Direcciones",
                column: "MunicipioCodigo",
                principalTable: "Municipios",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Direcciones_Departamentos_DepartamentoCodigo",
                table: "Direcciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Direcciones_Municipios_MunicipioCodigo",
                table: "Direcciones");

            migrationBuilder.DropIndex(
                name: "IX_Direcciones_DepartamentoCodigo",
                table: "Direcciones");

            migrationBuilder.DropIndex(
                name: "IX_Direcciones_MunicipioCodigo",
                table: "Direcciones");

            migrationBuilder.AlterColumn<string>(
                name: "Rol",
                table: "Usuarios",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);
        }
    }
}
