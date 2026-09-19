namespace PaginaVentasNet.Api.Modules.Pokemon.Application.Dtos;

public class PokemonResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Height { get; set; }
    public int Weight { get; set; }
}