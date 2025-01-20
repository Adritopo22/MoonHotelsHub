namespace MoonHotelsHub.Domain.Models
{
    public class HubRoom
    {
        public int RoomId { get; set; }
        public List<HubRate> Rates { get; set; } = [];
    }
}
