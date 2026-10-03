using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaginaVentasNet.Api.Modules.Geography.Domain;

namespace PaginaVentasNet.Api.Modules.Geography.Infrastructure;

public class MunicipioConfiguration : IEntityTypeConfiguration<Municipio>
{
    public void Configure(EntityTypeBuilder<Municipio> builder)
    {
        builder.HasKey(m => m.Codigo);

        builder.Property(m => m.Codigo)
            .ValueGeneratedNever();

        builder.Property(m => m.Nombre)
            .IsRequired()
            .HasMaxLength(100);
    }
}