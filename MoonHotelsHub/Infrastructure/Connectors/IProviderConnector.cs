using MoonHotelsHub.Domain.Models;

namespace MoonHotelsHub.Infrastructure.Connectors
{
    public interface IProviderConnector
    {
        Task<HubSearchResponse> Search(HubSearchRequest request);
    }
}
