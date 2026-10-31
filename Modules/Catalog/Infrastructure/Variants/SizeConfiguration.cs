using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Catalog.Infrastructure.Variants;

public class SizeConfiguration : IEntityTypeConfiguration<Size>
{
    public void Configure(EntityTypeBuilder<Size> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.SizeGroup)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.DisplayOrder)
            .IsRequired();

        builder.Property(s => s.IsActive)
            .IsRequired();

        // No se repite el nombre de una talla dentro de su grupo
        builder.HasIndex(s => new { s.SizeGroup, s.Name })
            .IsUnique();

        // Acelera el desplegable del admin: tallas de un grupo, en orden
        builder.HasIndex(s => new { s.SizeGroup, s.DisplayOrder });
    }
}