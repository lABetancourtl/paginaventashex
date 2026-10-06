using Microsoft.EntityFrameworkCore;
using PaginaVentasNet.Api.Data;
using PaginaVentasNet.Api.Modules.Cart.Application.Dtos;
using PaginaVentasNet.Api.Modules.Cart.Application.Ports;
using PaginaVentasNet.Api.Modules.Cart.Domain;

namespace PaginaVentasNet.Api.Modules.Cart.Infrastructure;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ShoppingCart?> GetByUsuarioIdAsync(int usuarioId)
    {
        return await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
    }

    public async Task<CartResponseDto?> GetCartDtoByUsuarioIdAsync(int usuarioId)
    {
        return await _context.Carts
            .Where(c => c.UsuarioId == usuarioId)
            .Select(c => new CartResponseDto
            {
                Id = c.Id,
                Items = c.Items.Select(i => new CartItemResponseDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product!.Name,
                    ProductImageUrl = i.Product.Media
                        .Where(m => m.IsMain)
                        .Select(m => m.Url)
                        .FirstOrDefault(),
                    ProductPrice = i.Product.Price,
                    Quantity = i.Quantity
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CartItem?> GetItemByIdAsync(int itemId)
    {
        return await _context.CartItems.FindAsync(itemId);
    }

    public async Task<CartItem?> GetItemByProductIdAsync(int cartId, int productId)
    {
        return await _context.CartItems
            .FirstOrDefaultAsync(i => i.CartId == cartId && i.ProductId == productId);
    }

    public async Task AddAsync(ShoppingCart cart)
    {
        await _context.Carts.AddAsync(cart);
    }

    public async Task AddItemAsync(CartItem item)
    {
        await _context.CartItems.AddAsync(item);
    }

    public Task RemoveItemAsync(CartItem item)
    {
        _context.CartItems.Remove(item);
        return Task.CompletedTask;
    }

    public async Task ClearItemsAsync(int cartId)
    {
        var items = await _context.CartItems
            .Where(i => i.CartId == cartId)
            .ToListAsync();

        _context.CartItems.RemoveRange(items);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}