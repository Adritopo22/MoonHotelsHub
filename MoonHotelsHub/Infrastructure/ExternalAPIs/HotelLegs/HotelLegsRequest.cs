namespace MoonHotelsHub.Infrastructure.ExternalAPIs.HotelLegs
{
    public class HotelLegsRequest
    {
        public required int Hotel { get; set; }
        public required DateTime CheckInDate { get; set; }
        public required int NumberOfNights { get; set; }
        public required int Guests { get; set; }
        public required int Rooms { get; set; }
        public required string Currency { get; set; }
    }
}
