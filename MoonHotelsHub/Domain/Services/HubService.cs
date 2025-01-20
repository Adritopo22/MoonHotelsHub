using MoonHotelsHub.Domain.Models;
using MoonHotelsHub.Infrastructure.Connectors;

namespace MoonHotelsHub.Domain.Services
{
    public class HubService
    {
        private readonly List<IProviderConnector> _providers = [];
        private readonly ILogger<HubService> _logger;

        public HubService(IEnumerable<IProviderConnector> providers, ILogger<HubService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (providers == null || !providers.Any())
            {
                throw new ArgumentException("The list of providers cannot be null or empty.", nameof(providers));
            }

            _providers.AddRange(providers);
        }

        public async Task<HubSearchResponse> Search(HubSearchRequest request)
        {
            var providerResponses = await GetProvidersResponsesAsync(request);

            return AggregateResponses(providerResponses);
        }

        private async Task<List<HubSearchResponse>> GetProvidersResponsesAsync(HubSearchRequest request)
        {
            var tasks = _providers.Select(async p =>
            {
                try
                {
                    return await p.Search(request);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while searching with provider.");
                    return null;
                }
            });

            var providerResponses = (await Task.WhenAll(tasks))
                .Where(r => r != null)
                .Select(r => r!)
                .ToList();

            if (providerResponses.Count == 0)
            {
                throw new InvalidOperationException("No providers responded successfully.");
            }

            return providerResponses;
        }

        private static HubSearchResponse AggregateResponses(List<HubSearchResponse> providerResponses)
        {
            var aggregatedResponse = new HubSearchResponse();

            foreach (var response in providerResponses)
            {
                aggregatedResponse.Rooms.AddRange(response.Rooms);
            }

            return aggregatedResponse;
        }
    }
}
