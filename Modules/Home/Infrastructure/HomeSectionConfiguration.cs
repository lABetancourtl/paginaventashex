using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaginaVentasNet.Api.Modules.Home.Domain;

namespace PaginaVentasNet.Api.Modules.Home.Infrastructure;

public class HomeSectionConfiguration : IEntityTypeConfiguration<HomeSection>
{
    public void Configure(EntityTypeBuilder<HomeSection> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(s => s.Title)
            .HasMaxLength(HomeSection.MaxTitleLength);

        builder.Property(s => s.DisplayOrder)
            .IsRequired();

        builder.Property(s => s.IsActive)
            .IsRequired();

        builder.HasOne(s => s.Category)
            .WithMany()
            .HasForeignKey(s => s.CategoryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(s => s.Items)
            .WithOne()
            .HasForeignKey(i => i.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Products)
            .WithOne()
            .HasForeignKey(p => p.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        // El orden no es único: el intercambio se guarda en un solo SaveChanges,
        // igual que en las categorías y las tallas.
        builder.HasIndex(s => s.DisplayOrder);

        // Última barrera en la base: el título y el "Ver todo" son solo de los estantes de productos
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_HomeSections_BannerSinTitulo",
            "\"Type\" = 'Products' OR (\"Title\" IS NULL AND \"CategoryId\" IS NULL)"));
    }
}