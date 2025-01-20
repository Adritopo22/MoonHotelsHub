using MoonHotelsHub.Domain.Exceptions;
using MoonHotelsHub.Domain.Models;
using MoonHotelsHub.Infrastructure.ExternalAPIs.HotelLegs;

namespace MoonHotelsHub.Infrastructure.Connectors
{
    public class HotelLegsConnector : IProviderConnector
    {
        private readonly IHotelLegsAPI _hotelLegsAPI;
        private readonly ILogger<HotelLegsConnector> _logger;

        public HotelLegsConnector(IHotelLegsAPI hotelLegsAPI, ILogger<HotelLegsConnector> logger)
        {
            _hotelLegsAPI = hotelLegsAPI;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<HubSearchResponse> Search(HubSearchRequest request)
        {
            try
            {
                var hotelLegsRequest = MapRequest(request);
                var hotelLegsResponse = await _hotelLegsAPI.Search(hotelLegsRequest);
                var hubSearchResponse = MapResponse(hotelLegsResponse);
                return hubSearchResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in HotelLegsConnector while calling HotelLegsAPI.");
                throw new ProviderException("HotelLegsConnector failed", ex);
            }
        }

        private static HubSearchResponse MapResponse(HotelLegsResponse hotelLegsResponse)
        {
            var hubSearchResponse = new HubSearchResponse
            {
                Rooms = []
            };

            foreach (var hotelLegsRoom in hotelLegsResponse.Results)
            {
                HubRoom? hubRoom = hubSearchResponse.Rooms.FirstOrDefault(r => r.RoomId == hotelLegsRoom.Room);
                if (hubRoom == null)
                {
                    hubRoom = new HubRoom { RoomId = hotelLegsRoom.Room, Rates = [] };
                    hubSearchResponse.Rooms.Add(hubRoom);
                }

                hubRoom.Rates.Add(
                    new HubRate
                    {
                        MealPlanId = hotelLegsRoom.Meal,
                        IsCancellable = hotelLegsRoom.CanCancel,
                        Price = hotelLegsRoom.Price
                    });
            }

            return hubSearchResponse;
        }

        private static HotelLegsRequest MapRequest(HubSearchRequest request)
        {
            var hotelLegsRequest = new HotelLegsRequest
            {
                Hotel = request.HotelId,
                CheckInDate = request.CheckIn,
                NumberOfNights = (int)(request.CheckOut - request.CheckIn).TotalDays,
                Guests = request.NumberOfGuests,
                Rooms = request.NumberOfRooms,
                Currency = request.Currency
            };
            return hotelLegsRequest;
        }
    }
}
