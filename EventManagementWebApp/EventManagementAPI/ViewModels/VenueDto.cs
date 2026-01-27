using Newtonsoft.Json;

namespace EventManagementAPI.ViewModels
{
    public class VenueDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; } = "";
        [JsonProperty("description")]
        public string Description { get; set; } = "";
        [JsonProperty("capacity")]
        public int Capacity { get; set; }
        public string CreatedBy { get; set; } = "System";
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
