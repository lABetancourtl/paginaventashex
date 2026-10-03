using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaginaVentasNet.Api.Modules.Geography.Domain;

namespace PaginaVentasNet.Api.Modules.Geography.Infrastructure;

public class DepartamentoConfiguration : IEntityTypeConfiguration<Departamento>
{
    public void Configure(EntityTypeBuilder<Departamento> builder)
    {
        builder.HasKey(d => d.Codigo);

        builder.Property(d => d.Codigo)
            .ValueGeneratedNever();

        builder.Property(d => d.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(d => d.Municipios)
            .WithOne(m => m.Departamento)
            .HasForeignKey(m => m.DepartamentoCodigo)
            .OnDelete(DeleteBehavior.Restrict);
    }
}