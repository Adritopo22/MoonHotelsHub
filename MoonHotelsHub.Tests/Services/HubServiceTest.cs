using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using MoonHotelsHub.Domain.Services;
using MoonHotelsHub.Domain.Models;
using MoonHotelsHub.Infrastructure.Connectors;

public class HubServiceTests
{
    private readonly Mock<IProviderConnector> _mockProvider1;
    private readonly Mock<IProviderConnector> _mockProvider2;
    private readonly Mock<ILogger<HubService>> _mockLogger;
    private readonly HubService _hubService;

    public HubServiceTests()
    {
        _mockProvider1 = new Mock<IProviderConnector>();
        _mockProvider2 = new Mock<IProviderConnector>();
        _mockLogger = new Mock<ILogger<HubService>>();

        _hubService = new HubService(new List<IProviderConnector> { _mockProvider1.Object, _mockProvider2.Object }, _mockLogger.Object);
    }

    [Fact]
    public async Task Search_ShouldReturnAggregatedResults_WhenProvidersRespond()
    {
        // Arrange
        var request = new HubSearchRequest
        {
            HotelId = 1,
            CheckIn = DateTime.Today,
            CheckOut = DateTime.Today.AddDays(5),
            NumberOfGuests = 2,
            NumberOfRooms = 1,
            Currency = "EUR"
        };

        var response1 = new HubSearchResponse
        {
            Rooms = new List<HubRoom>
            {
                new HubRoom { RoomId = 1, Rates = new List<HubRate> { new HubRate { MealPlanId = 1, IsCancellable = false, Price = 100m } } }
            }
        };

        var response2 = new HubSearchResponse
        {
            Rooms = new List<HubRoom>
            {
                new HubRoom { RoomId = 2, Rates = new List<HubRate> { new HubRate { MealPlanId = 2, IsCancellable = true, Price = 150m } } }
            }
        };

        _mockProvider1.Setup(p => p.Search(request)).ReturnsAsync(response1);
        _mockProvider2.Setup(p => p.Search(request)).ReturnsAsync(response2);

        // Act
        var aggregatedResponse = await _hubService.Search(request);

        // Assert
        Assert.NotNull(aggregatedResponse);
        Assert.Equal(2, aggregatedResponse.Rooms.Count);
    }

    [Fact]
    public async Task Search_ShouldHandleProviderFailure_AndContinueWithOthers()
    {
        // Arrange
        var request = new HubSearchRequest
        {
            HotelId = 1,
            CheckIn = DateTime.Today,
            CheckOut = DateTime.Today.AddDays(5),
            NumberOfGuests = 2,
            NumberOfRooms = 1,
            Currency = "EUR"
        };

        var response1 = new HubSearchResponse
        {
            Rooms = new List<HubRoom>
            {
                new HubRoom { RoomId = 1, Rates = new List<HubRate> { new HubRate { MealPlanId = 1, IsCancellable = false, Price = 100m } } }
            }
        };

        _mockProvider1.Setup(p => p.Search(request)).ReturnsAsync(response1);
        _mockProvider2.Setup(p => p.Search(request)).ThrowsAsync(new Exception("Provider failure"));

        // Act
        var aggregatedResponse = await _hubService.Search(request);

        // Assert
        Assert.NotNull(aggregatedResponse);
        Assert.Single(aggregatedResponse.Rooms);
    }

    [Fact]
    public async Task Search_ShouldThrowException_WhenAllProvidersFail()
    {
        // Arrange
        var request = new HubSearchRequest
        {
            HotelId = 1,
            CheckIn = DateTime.Today,
            CheckOut = DateTime.Today.AddDays(5),
            NumberOfGuests = 2,
            NumberOfRooms = 1,
            Currency = "EUR"
        };

        _mockProvider1.Setup(p => p.Search(request)).ThrowsAsync(new Exception("Provider 1 failure"));
        _mockProvider2.Setup(p => p.Search(request)).ThrowsAsync(new Exception("Provider 2 failure"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _hubService.Search(request));
    }
}
