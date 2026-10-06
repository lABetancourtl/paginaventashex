using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Orders.Application.Dtos;
using PaginaVentasNet.Api.Modules.Orders.Application.UseCases;

namespace PaginaVentasNet.Api.Modules.Orders.Presentation;

/// <summary>
/// Controller para la gestión de pedidos.
/// </summary>
[Authorize]
[Route("api/orders")]
public class OrderController : ApiController
{
    private readonly CreateOrderUseCase _createOrderUseCase;
    private readonly GetOrdersUseCase _getOrdersUseCase;
    private readonly CancelOrderUseCase _cancelOrderUseCase;
    private readonly UpdateOrderStatusUseCase _updateOrderStatusUseCase;

    public OrderController(
        CreateOrderUseCase createOrderUseCase,
        GetOrdersUseCase getOrdersUseCase,
        CancelOrderUseCase cancelOrderUseCase,
        UpdateOrderStatusUseCase updateOrderStatusUseCase)
    {
        _createOrderUseCase = createOrderUseCase;
        _getOrdersUseCase = getOrdersUseCase;
        _cancelOrderUseCase = cancelOrderUseCase;
        _updateOrderStatusUseCase = updateOrderStatusUseCase;
    }

    /// <summary>
    /// Crea un pedido desde el carrito del usuario autenticado.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<int>>> Create(CreateOrderDto dto)
    {
        try
        {
            var usuarioId = GetUsuarioId();
            var orderId = await _createOrderUseCase.ExecuteAsync(usuarioId, dto);
            return Success(orderId, 201);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<int>("ORDER_ERROR", ex.Message, 400);
        }
    }

    /// <summary>
    /// Obtiene todos los pedidos del usuario autenticado.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<OrderResponseDto>>>> GetMyOrders()
    {
        var usuarioId = GetUsuarioId();
        var orders = await _getOrdersUseCase.ExecuteAsync(usuarioId);
        return Success(orders);
    }

    /// <summary>
    /// Obtiene el detalle de un pedido del usuario autenticado.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> GetById(int id)
    {
        var usuarioId = GetUsuarioId();
        var order = await _getOrdersUseCase.ExecuteAsync(id, usuarioId);

        if (order is null)
            return Failure<OrderResponseDto>("ORDER_NOT_FOUND",
                $"No existe un pedido con Id '{id}'.", 404);

        return Success(order);
    }

    /// <summary>
    /// Cancela un pedido del usuario autenticado.
    /// Restaura el stock de los productos.
    /// </summary>
    [HttpPatch("{id}/cancel")]
    public async Task<ActionResult<ApiResponse<bool>>> Cancel(int id)
    {
        try
        {
            var usuarioId = GetUsuarioId();
            await _cancelOrderUseCase.ExecuteAsync(id, usuarioId);
            return Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<bool>("ORDER_ERROR", ex.Message, 400);
        }
    }

    /// <summary>
    /// Obtiene todos los pedidos de todos los usuarios. Solo Admin.
    /// </summary>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<List<OrderResponseDto>>>> GetAll()
    {
        var orders = await _getOrdersUseCase.ExecuteAllAsync();
        return Success(orders);
    }

    /// <summary>
    /// Actualiza el estado de un pedido. Solo Admin.
    /// </summary>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(int id, UpdateOrderStatusDto dto)
    {
        try
        {
            await _updateOrderStatusUseCase.ExecuteAsync(id, dto);
            return Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Failure<bool>("ORDER_ERROR", ex.Message, 400);
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