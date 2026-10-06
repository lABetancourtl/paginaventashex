using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PaginaVentasNet.Api.Modules.Cart.Infrastructure;

public class CartConfiguration : IEntityTypeConfiguration<Domain.ShoppingCart>
{
    public void Configure(EntityTypeBuilder<Domain.ShoppingCart> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.UsuarioId)
            .IsUnique();

        builder.HasMany(c => c.Items)
            .WithOne()
            .HasForeignKey(i => i.CartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}