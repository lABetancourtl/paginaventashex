using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaginaVentasNet.Api.Modules.Identity.Domain;

namespace PaginaVentasNet.Api.Modules.Identity.Infrastructure;

public class DireccionConfiguration : IEntityTypeConfiguration<Direccion>
{
    public void Configure(EntityTypeBuilder<Direccion> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.DireccionTexto)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.NombreQuienRecibe)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.InformacionAdicional)
            .HasMaxLength(100);

        builder.Property(d => d.Barrio)
            .HasMaxLength(100);

        builder.Property(d => d.EsPrincipal)
            .HasDefaultValue(false);

        builder.HasOne<Usuario>()
            .WithMany(u => u.Direcciones)
            .HasForeignKey(d => d.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Departamento)
            .WithMany()
            .HasForeignKey(d => d.DepartamentoCodigo)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Municipio)
            .WithMany()
            .HasForeignKey(d => d.MunicipioCodigo)
            .OnDelete(DeleteBehavior.Restrict);           
    }
}