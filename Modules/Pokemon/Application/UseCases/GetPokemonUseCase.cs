using PaginaVentasNet.Api.Modules.Pokemon.Application.Dtos;
using PaginaVentasNet.Api.Modules.Pokemon.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Pokemon.Application.UseCases;

public class GetPokemonUseCase
{
    private readonly IPokemonProvider _pokemonProvider;

    public GetPokemonUseCase(IPokemonProvider pokemonProvider)
    {
        _pokemonProvider = pokemonProvider;
    }

    public async Task<PokemonResponseDto> ExecuteAsync(string name)
    {
        return await _pokemonProvider.GetByNameAsync(name);
    }

    public async Task<PokemonResponseDto> ExecuteAsync(int id)
    {
        return await _pokemonProvider.GetPokemonByIdAsync(id);
    }

}