using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaginaVentasNet.Api.Modules.Payments.Domain;

namespace PaginaVentasNet.Api.Modules.Payments.Infrastructure;

public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.WompiTransactionId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Reference)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Currency)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(t => t.PaymentMethodType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.CardBrand)
            .HasMaxLength(50);

        builder.Property(t => t.CardLastFour)
            .HasMaxLength(4);

        builder.Property(t => t.CardHolder)
            .HasMaxLength(100);

        builder.Property(t => t.CustomerEmail)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.CustomerPhone)
            .HasMaxLength(20);

        builder.Property(t => t.CustomerName)
            .HasMaxLength(100);

        builder.Property(t => t.WompiEvent)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.RawPayload)
            .IsRequired();

        builder.HasIndex(t => t.WompiTransactionId);
        builder.HasIndex(t => t.Reference);
        builder.HasIndex(t => t.Status);
    }
}