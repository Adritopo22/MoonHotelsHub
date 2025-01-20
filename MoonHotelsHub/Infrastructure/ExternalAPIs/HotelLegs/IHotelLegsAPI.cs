namespace MoonHotelsHub.Infrastructure.ExternalAPIs.HotelLegs
{
    public interface IHotelLegsAPI
    {
        Task<HotelLegsResponse> Search(HotelLegsRequest request);
    }
}
