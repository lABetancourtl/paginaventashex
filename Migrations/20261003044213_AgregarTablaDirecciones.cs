using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PaginaVentasNet.Api.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaDirecciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Direcciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    DepartamentoCodigo = table.Column<int>(type: "integer", nullable: false),
                    MunicipioCodigo = table.Column<int>(type: "integer", nullable: false),
                    DireccionTexto = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    InformacionAdicional = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Barrio = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NombreQuienRecibe = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EsPrincipal = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreadoEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Direcciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Direcciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Direcciones_UsuarioId",
                table: "Direcciones",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Direcciones");
        }
    }
}
