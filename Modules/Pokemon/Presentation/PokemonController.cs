using Microsoft.AspNetCore.Mvc;
using PaginaVentasNet.Api.Common.Responses;
using PaginaVentasNet.Api.Controllers;
using PaginaVentasNet.Api.Modules.Pokemon.Application.Dtos;
using PaginaVentasNet.Api.Modules.Pokemon.Application.UseCases;

namespace PaginaVentasNet.Api.Modules.Pokemon.Presentation;

[Route("api/pokemon")]
public class PokemonController : ApiController
{
    private readonly GetPokemonUseCase _getPokemonUseCase;

    public PokemonController(GetPokemonUseCase getPokemonUseCase)
    {
        _getPokemonUseCase = getPokemonUseCase;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PokemonResponseDto>>> GetByName([FromQuery] string name)
    {
        var pokemon = await _getPokemonUseCase.ExecuteAsync(name);
        return Success(pokemon);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<PokemonResponseDto>>> GetById(
    [FromRoute] int id)
    {
        var pokemon = await _getPokemonUseCase.ExecuteAsync(id);
        return Success(pokemon);
    }

   
}