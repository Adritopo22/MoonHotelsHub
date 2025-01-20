using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using MoonHotelsHub.Infrastructure.Connectors;
using MoonHotelsHub.Infrastructure.ExternalAPIs;
using MoonHotelsHub.Domain.Models;
using MoonHotelsHub.Domain.Exceptions;
using MoonHotelsHub.Infrastructure.ExternalAPIs.HotelLegs;

public class HotelLegsConnectorTests
{
    private readonly Mock<IHotelLegsAPI> _mockHotelLegsAPI;
    private readonly Mock<ILogger<HotelLegsConnector>> _mockLogger;
    private readonly HotelLegsConnector _connector;

    public HotelLegsConnectorTests()
    {
        _mockHotelLegsAPI = new Mock<IHotelLegsAPI>();
        _mockLogger = new Mock<ILogger<HotelLegsConnector>>();
        _connector = new HotelLegsConnector(_mockHotelLegsAPI.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Search_ShouldReturnValidResponse_WhenHotelLegsAPIResponds()
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

        var fakeResponse = new HotelLegsResponse
        {
            Results = new List<HotelLegsRoom>
            {
                new HotelLegsRoom { Room = 1, Meal = 1, CanCancel = false, Price = 120.50m }
            }
        };

        _mockHotelLegsAPI.Setup(api => api.Search(It.IsAny<HotelLegsRequest>()))
            .ReturnsAsync(fakeResponse);

        // Act
        var response = await _connector.Search(request);

        // Assert
        Assert.NotNull(response);
        Assert.Single(response.Rooms);
        Assert.Equal(120.50m, response.Rooms[0].Rates[0].Price);
    }

    [Fact]
    public async Task Search_ShouldThrowProviderException_WhenHotelLegsAPIFails()
    {
        // Arrange
        _mockHotelLegsAPI.Setup(api => api.Search(It.IsAny<HotelLegsRequest>()))
            .ThrowsAsync(new Exception("API failure"));

        var request = new HubSearchRequest
        {
            HotelId = 1,
            CheckIn = DateTime.Today,
            CheckOut = DateTime.Today.AddDays(5),
            NumberOfGuests = 2,
            NumberOfRooms = 1,
            Currency = "EUR"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ProviderException>(() => _connector.Search(request));
    }
}
