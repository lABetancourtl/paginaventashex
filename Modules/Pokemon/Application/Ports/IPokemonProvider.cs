namespace PaginaVentasNet.Api.Modules.Pokemon.Application.Ports;

using PaginaVentasNet.Api.Modules.Pokemon.Application.Dtos;

public interface IPokemonProvider
{
    Task<PokemonResponseDto> GetPokemonByIdAsync(int id);

    Task<PokemonResponseDto> GetByNameAsync(string name);
}