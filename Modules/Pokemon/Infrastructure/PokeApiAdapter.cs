namespace PaginaVentasNet.Api.Modules.Pokemon.Infrastructure;

using System.Net.Http;
using PaginaVentasNet.Api.Modules.Pokemon.Application.Dtos;
using PaginaVentasNet.Api.Modules.Pokemon.Application.Ports;

public class PokeApiAdapter : IPokemonProvider
{

   private readonly HttpClient _httpCliente;

   public PokeApiAdapter(HttpClient httpCliente)
    {
        _httpCliente = httpCliente;
    }


    public async Task<PokemonResponseDto> GetByNameAsync(string name)
    {

        var response = await _httpCliente.GetAsync(
            $"https://pokeapi.co/api/v2/pokemon/{name}"
        );
        response.EnsureSuccessStatusCode();

        var pokemon = await response.Content
            .ReadFromJsonAsync<PokemonResponseDto>();

        return pokemon!;
    }

    public async Task<PokemonResponseDto> GetPokemonByIdAsync(int id)
    {

        var response = await _httpCliente.GetAsync(
            $"https://pokeapi.co/api/v2/pokemon/{id}"
        );
        response.EnsureSuccessStatusCode();

        var pokemon = await response.Content
            .ReadFromJsonAsync<PokemonResponseDto>();

        return pokemon!;
    }


}