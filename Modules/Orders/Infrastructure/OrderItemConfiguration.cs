using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaginaVentasNet.Api.Modules.Orders.Domain;

namespace PaginaVentasNet.Api.Modules.Orders.Infrastructure;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.ProductPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Property(i => i.SizeName)
            .HasMaxLength(50);

        builder.Property(i => i.ColorName)
            .HasMaxLength(50);

        builder.Property(i => i.Sku)
            .HasMaxLength(50);

        builder.Property(i => i.BasePrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.DiscountPercent);

        builder.Ignore(i => i.Subtotal);

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_OrderItems_DiscountPercent",
            "\"DiscountPercent\" IS NULL OR (\"DiscountPercent\" BETWEEN 1 AND 99)"));
    }
}