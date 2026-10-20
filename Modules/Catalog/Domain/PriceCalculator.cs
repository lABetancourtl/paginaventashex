namespace PaginaVentasNet.Api.Modules.Catalog.Domain;

/// <summary>
/// Única fuente de verdad para calcular el precio final de un producto.
/// El resultado se redondea a pesos enteros (compatible con Wompi, que usa centavos enteros).
/// </summary>
public static class PriceCalculator
{
    public static decimal Final(decimal price, int? discountPercent)
    {
        if (discountPercent is null)
            return price;

        var discounted = price - (price * discountPercent.Value / 100m);
        return Math.Round(discounted, 0, MidpointRounding.AwayFromZero);
    }
}