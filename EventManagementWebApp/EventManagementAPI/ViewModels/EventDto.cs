using EventManagementAPI.Commons;
using EventManagementAPI.Domain.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EventManagementAPI.ViewModels
{
    public class EventDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("description")]
        public string Description { get; set; } = "";

        [JsonIgnore]
        public long TotalTicketsAvail { get; set; }

        [JsonIgnore]
        public long TotalSoldTicket { get; set; }

        [JsonProperty("eventDate")]
        public DateTime? EventDate { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonIgnore]
        public EventStatus EventStatus { get; set; } = EventStatus.Active;

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; } = "System";

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [JsonProperty("venueId")]
        public long VenueId { get; set; }

        [JsonProperty("Venue")]
        public Venue? Venue { get; set; }

        [JsonProperty("PricingTier")]
        public PricingTier? PricingTier { get; set; }

        [JsonProperty("pricingTierId")]
        public long PricingTierId { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; } = true;
    }
}
