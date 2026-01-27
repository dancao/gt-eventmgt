using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EventManagementAPI.Commons
{
    public class ApiResponse<T>
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("status")]
        public ApiResponseStatus Status { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; } = "";
    }
}
