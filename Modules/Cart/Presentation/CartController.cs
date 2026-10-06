using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Cart.Application.Dtos;
using PaginaVentasNet.Api.Modules.Cart.Application.UseCases;

namespace PaginaVentasNet.Api.Modules.Cart.Presentation;

/// <summary>
/// Controller para la gestión del carrito de compras del usuario autenticado.
/// </summary>
[Authorize]
[Route("api/cart")]
public class CartController : ApiController
{
    private readonly GetCartUseCase _getCartUseCase;
    private readonly AddCartItemUseCase _addCartItemUseCase;
    private readonly UpdateCartItemUseCase _updateCartItemUseCase;
    private readonly RemoveCartItemUseCase _removeCartItemUseCase;
    private readonly ClearCartUseCase _clearCartUseCase;

    public CartController(
        GetCartUseCase getCartUseCase,
        AddCartItemUseCase addCartItemUseCase,
        UpdateCartItemUseCase updateCartItemUseCase,
        RemoveCartItemUseCase removeCartItemUseCase,
        ClearCartUseCase clearCartUseCase)
    {
        _getCartUseCase = getCartUseCase;
        _addCartItemUseCase = addCartItemUseCase;
        _updateCartItemUseCase = updateCartItemUseCase;
        _removeCartItemUseCase = removeCartItemUseCase;
        _clearCartUseCase = clearCartUseCase;
    }

    /// <summary>
    /// Obtiene el carrito del usuario autenticado con todos sus items y el total.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<CartResponseDto>>> GetCart()
    {
        var usuarioId = GetUsuarioId();
        var cart = await _getCartUseCase.ExecuteAsync(usuarioId);
        return Success(cart);
    }

    /// <summary>
    /// Agrega un producto al carrito.
    /// Si el producto ya existe, actualiza la cantidad.
    /// Si no tiene carrito, lo crea automáticamente.
    /// </summary>
    [HttpPost("items")]
    public async Task<ActionResult<ApiResponse<bool>>> AddItem(AddCartItemDto dto)
    {
        try
        {
            var usuarioId = GetUsuarioId();
            await _addCartItemUseCase.ExecuteAsync(usuarioId, dto);
            return Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<bool>("CART_ERROR", ex.Message, 400);
        }
    }

    /// <summary>
    /// Actualiza la cantidad de un item del carrito.
    /// </summary>
    [HttpPut("items/{itemId}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateItem(int itemId, UpdateCartItemDto dto)
    {
        try
        {
            var usuarioId = GetUsuarioId();
            await _updateCartItemUseCase.ExecuteAsync(usuarioId, itemId, dto);
            return Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<bool>("CART_ERROR", ex.Message, 400);
        }
    }

    /// <summary>
    /// Elimina un item del carrito.
    /// </summary>
    [HttpDelete("items/{itemId}")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveItem(int itemId)
    {
        try
        {
            var usuarioId = GetUsuarioId();
            await _removeCartItemUseCase.ExecuteAsync(usuarioId, itemId);
            return Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<bool>("CART_ERROR", ex.Message, 404);
        }
    }

    /// <summary>
    /// Vacía el carrito completo del usuario.
    /// </summary>
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<bool>>> ClearCart()
    {
        try
        {
            var usuarioId = GetUsuarioId();
            await _clearCartUseCase.ExecuteAsync(usuarioId);
            return Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<bool>("CART_ERROR", ex.Message, 400);
        }
    }

    private int GetUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                    ?? User.FindFirst("sub");

        if (claim is null)
            throw new InvalidOperationException("No se pudo obtener el Id del usuario del token.");

        return int.Parse(claim.Value);
    }
}