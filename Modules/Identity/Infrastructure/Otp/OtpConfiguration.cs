using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaginaVentasNet.Api.Modules.Identity.Domain;

namespace PaginaVentasNet.Api.Modules.Identity.Infrastructure.Otp;

public class OtpConfiguration : IEntityTypeConfiguration<OtpCode>
{
    public void Configure(EntityTypeBuilder<OtpCode> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Code)
            .IsRequired()
            .HasMaxLength(6);

        builder.Property(o => o.ExpiresAtUtc)
            .IsRequired();

        builder.Property(o => o.IsUsed)
            .HasDefaultValue(false);

        builder.HasIndex(o => new { o.Email, o.Code });
    }
}