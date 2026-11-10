using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaginaVentasNet.Api.Modules.Home.Domain;

namespace PaginaVentasNet.Api.Modules.Home.Infrastructure;

public class HomeSectionItemConfiguration : IEntityTypeConfiguration<HomeSectionItem>
{
    public void Configure(EntityTypeBuilder<HomeSectionItem> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(i => i.PublicId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(i => i.MediaType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(i => i.DisplayOrder)
            .IsRequired();

        builder.HasOne(i => i.Category)
            .WithMany()
            .HasForeignKey(i => i.CategoryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(i => new { i.SectionId, i.DisplayOrder });
    }
}