namespace MoonHotelsHub.Infrastructure.ExternalAPIs.HotelLegs
{
    public class HotelLegsRoom
    {
        public int Room { get; set; }
        public int Meal { get; set; }
        public bool CanCancel { get; set; }
        public decimal Price { get; set; }
    }
}