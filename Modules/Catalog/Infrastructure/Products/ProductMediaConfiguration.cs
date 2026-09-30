using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Catalog.Infrastructure.Products;

public class ProductMediaConfiguration : IEntityTypeConfiguration<ProductMedia>
{
    public void Configure(EntityTypeBuilder<ProductMedia> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(m => m.PublicId)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.MediaType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(m => m.DisplayOrder)
            .IsRequired();

        builder.Property(m => m.IsMain)
            .HasDefaultValue(false);

        builder.HasOne<Product>()
            .WithMany(p => p.Media)
            .HasForeignKey(m => m.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}