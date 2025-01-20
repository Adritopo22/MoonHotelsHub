namespace MoonHotelsHub.Domain.Models
{
    public class HubSearchRequest
    {
        public required int HotelId { get; set; }
        public required DateTime CheckIn { get; set; }
        public required DateTime CheckOut { get; set; }
        public required int NumberOfGuests { get; set; }
        public required int NumberOfRooms { get; set; }
        public required string Currency { get; set; }
    }
}
