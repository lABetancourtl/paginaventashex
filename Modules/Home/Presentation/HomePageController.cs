using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Home.Application.Dtos;
using PaginaVentasNet.Api.Modules.Home.Application.UseCases;

namespace PaginaVentasNet.Api.Modules.Home.Presentation;

/// <summary>
/// Página de inicio pública de la tienda.
/// </summary>
[AllowAnonymous]
[Route("api/home")]
public class HomePageController : ApiController
{
    private readonly GetHomePageUseCase _getHomePageUseCase;

    public HomePageController(GetHomePageUseCase getHomePageUseCase)
    {
        _getHomePageUseCase = getHomePageUseCase;
    }

    /// <summary>
    /// Devuelve las secciones activas de la página de inicio, en orden. Cada banner trae sus medios
    /// y cada estante sus productos visibles. Una sección sin nada que mostrar no sale.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<HomePageResponseDto>>> Get()
    {
        var page = await _getHomePageUseCase.ExecuteAsync();
        return Success(page);
    }
}