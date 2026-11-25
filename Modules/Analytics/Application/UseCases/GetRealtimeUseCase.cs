using PaginaVentasNet.Api.Modules.Analytics.Application.Dtos;
using PaginaVentasNet.Api.Modules.Analytics.Application.Ports;

namespace PaginaVentasNet.Api.Modules.Analytics.Application.UseCases;

/// <summary>Cuántas personas hay en la tienda en este momento.</summary>
public class GetRealtimeUseCase
{
    private readonly IAnalyticsProvider _provider;

    public GetRealtimeUseCase(IAnalyticsProvider provider)
    {
        _provider = provider;
    }

    public async Task<RealtimeDto> ExecuteAsync()
    {
        return new RealtimeDto { ActiveVisitors = await _provider.GetActiveVisitorsAsync() };
    }
}
