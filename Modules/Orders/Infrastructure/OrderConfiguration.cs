using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaginaVentasNet.Api.Modules.Orders.Domain;

namespace PaginaVentasNet.Api.Modules.Orders.Infrastructure;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(o => o.Total)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.DepartamentoNombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.MunicipioNombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.DireccionTexto)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.InformacionAdicional)
            .HasMaxLength(100);

        builder.Property(o => o.Barrio)
            .HasMaxLength(100);

        builder.Property(o => o.NombreQuienRecibe)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}