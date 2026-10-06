using PaginaVentasNet.Api.Modules.Cart.Application.Dtos;
using PaginaVentasNet.Api.Modules.Cart.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Cart.Application.UseCases;

/// <summary>
/// Caso de uso para obtener el carrito del usuario autenticado.
/// Si no tiene carrito, retorna uno vacío.
/// </summary>
public class GetCartUseCase
{
    private readonly ICartRepository _repository;

    public GetCartUseCase(ICartRepository repository)
    {
        _repository = repository;
    }

    public async Task<CartResponseDto> ExecuteAsync(int usuarioId)
    {
        var cart = await _repository.GetCartDtoByUsuarioIdAsync(usuarioId);

        return cart ?? new CartResponseDto();
    }
}