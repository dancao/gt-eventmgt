using Newtonsoft.Json;

namespace EventManagementAPI.ViewModels
{
    public class PurchaseTicketDto
    {
        [JsonProperty("ticketTypeId")]
        public long TicketTypeId { get; set; }

        [JsonProperty("buyerName")]
        public string BuyerName { get; set; } = "";

        [JsonProperty("quantity")]
        public int Quantity { get; set; } = 0;

        [JsonProperty("totalCost")]
        public decimal TotalCost { get; set; } = 0;
    }
}
