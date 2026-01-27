using Newtonsoft.Json;

namespace EventManagementAPI.Domain.Entities
{
    public class TicketType
    {
        public long Id { get; set; }
        public string Name { get; set; } = "General Admission";

        [JsonProperty("totalAvailable")]
        public int TotalAvailable { get; set; }  // Initial inventory

        [JsonProperty("remaining")]
        public int Remaining { get; set; }

        public long EventId { get; set; }
        public Event Event { get; set; } = null!;

        [JsonProperty("PricingTier")]
        public PricingTier PricingTier { get; set; } = null!;

        [JsonProperty("pricingTierId")]
        public long PricingTierId { get; set; }
    }
}
