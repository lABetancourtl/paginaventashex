using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaginaVentasNet.Api.Modules.Catalog.Domain;

namespace PaginaVentasNet.Api.Modules.Catalog.Infrastructure.Variants;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Sku)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(v => v.Sku)
            .IsUnique();

        builder.Property(v => v.Stock)
            .IsRequired();

        builder.Property(v => v.IsActive)
            .IsRequired();

        builder.HasOne<Product>()
            .WithMany(p => p.Variants)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.Size)
            .WithMany()
            .HasForeignKey(v => v.SizeId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.Color)
            .WithMany()
            .HasForeignKey(v => v.ColorId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // Una combinación de talla y color por producto. AreNullsDistinct(false) genera
        // NULLS NOT DISTINCT, así el biberón no puede tener dos variantes con ambos en null.
        builder.HasIndex(v => new { v.ProductId, v.SizeId, v.ColorId })
            .IsUnique()
            .AreNullsDistinct(false);

        // Última barrera en la base, igual que con el descuento
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_ProductVariants_Stock",
            "\"Stock\" >= 0"));
    }
}