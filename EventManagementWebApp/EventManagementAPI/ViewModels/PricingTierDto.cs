using Newtonsoft.Json;

namespace EventManagementAPI.ViewModels
{
    public class PricingTierDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("description")]
        public string Description { get; set; } = "";

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; } = true;

        [JsonIgnore]
        public List<EventDto> Events { get; set; } = [];
    }
}
