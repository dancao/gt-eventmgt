using EventManagementAPI.Domain.Entities;
using Newtonsoft.Json;

namespace EventManagementAPI.ViewModels
{
    public class TicketTypeDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = "General Admission";

        [JsonProperty("totalAvailable")]
        public int TotalAvailable { get; set; }  // Initial inventory

        [JsonProperty("remaining")]
        public int Remaining { get; set; }

        [JsonProperty("eventId")]
        public long EventId { get; set; }

        [JsonProperty("event")]
        public EventDto? Event { get; set; }

        [JsonProperty("PricingTier")]
        public PricingTierDto? PricingTier { get; set; }

        [JsonProperty("pricingTierId")]
        public long PricingTierId { get; set; }

        public List<TicketDto> Tickets { get; set; } = [];
    }

    public class TicketTypeLiteDto
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "General Admission";

        [JsonProperty("totalAvailable")]
        public int TotalAvailable { get; set; }  // Initial inventory

        [JsonProperty("remaining")]
        public int Remaining { get; set; }
    }
}
