namespace MoonHotelsHub.Domain.Models
{
    public class HubRate
    {
        public int MealPlanId { get; set; }
        public bool IsCancellable { get; set; }
        public decimal Price { get; set; }
    }
}
