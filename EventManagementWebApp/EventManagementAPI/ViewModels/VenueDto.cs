using Newtonsoft.Json;

namespace EventManagementAPI.ViewModels
{
    public class VenueDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("description")]
        public string Description { get; set; } = "";

        [JsonProperty("address")]
        public string Address { get; set; } = "";

        [JsonProperty("capacity")]
        public int Capacity { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; } = "System";

        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [JsonProperty("isActive")]
        public bool IsActive { get; set; } = true;
    }
}
