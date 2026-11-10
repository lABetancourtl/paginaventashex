using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaginaVentasNet.Api.Modules.Home.Domain;

namespace PaginaVentasNet.Api.Modules.Home.Infrastructure;

public class HomeSectionProductConfiguration : IEntityTypeConfiguration<HomeSectionProduct>
{
    public void Configure(EntityTypeBuilder<HomeSectionProduct> builder)
    {
        // La clave compuesta impide repetir un producto en el mismo estante
        builder.HasKey(p => new { p.SectionId, p.ProductId });

        builder.Property(p => p.DisplayOrder)
            .IsRequired();

        builder.HasOne(p => p.Product)
            .WithMany()
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => new { p.SectionId, p.DisplayOrder });
    }
}