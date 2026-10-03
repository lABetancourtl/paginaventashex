using Microsoft.EntityFrameworkCore;
using PaginaVentasNet.Api.Modules.Catalog.Domain;
using PaginaVentasNet.Api.Modules.Catalog.Infrastructure.Categories;
using PaginaVentasNet.Api.Modules.Catalog.Infrastructure.Products;
using PaginaVentasNet.Api.Modules.Geography.Domain;
using PaginaVentasNet.Api.Modules.Geography.Infrastructure;
using PaginaVentasNet.Api.Modules.Identity.Domain;
using PaginaVentasNet.Api.Modules.Identity.Infrastructure;
using PaginaVentasNet.Api.Modules.Identity.Infrastructure.Otp;

namespace PaginaVentasNet.Api.Data;

/// <summary>
/// Contexto de base de datos para la aplicación.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<ProductMedia> ProductMedia { get; set; }
    public DbSet<OtpCode> OtpCodes { get; set; }
    public DbSet<Departamento> Departamentos { get; set; }
    public DbSet<Municipio> Municipios { get; set; }
    public DbSet<Direccion> Direcciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new ProductMediaConfiguration());
        modelBuilder.ApplyConfiguration(new UsuarioConfiguration());
        modelBuilder.ApplyConfiguration(new OtpConfiguration());
        modelBuilder.ApplyConfiguration(new DepartamentoConfiguration());
        modelBuilder.ApplyConfiguration(new MunicipioConfiguration());
        modelBuilder.ApplyConfiguration(new DireccionConfiguration());
    }
}