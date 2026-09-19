namespace PaginaVentasNet.Api.Modules.Pokemon.Domain;

public class Pokemon
{
    public int Id { get; private set; }

    public string Name { get; private set; }

    public int Height { get; private set; }

    public int Weight { get; private set; }

    public Pokemon(int id, string name, int height, int weight)
    {
        Id = id;
        Name = name;
        Height = height;
        Weight = weight;
    }
}