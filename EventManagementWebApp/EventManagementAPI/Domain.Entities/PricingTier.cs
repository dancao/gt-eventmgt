namespace EventManagementAPI.Domain.Entities
{
    public class PricingTier
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }

        public List<Event> Events { get; set; } = [];
    }
}
