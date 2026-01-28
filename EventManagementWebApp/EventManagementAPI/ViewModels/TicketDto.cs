using Newtonsoft.Json;

namespace EventManagementAPI.ViewModels
{
    public class TicketDto
    {
        public long Id { get; set; }
        public string BuyerName { get; set; } = "";
        public int Quanlity { get; set; }
        public DateTime PurchasedDate { get; set; }
        public decimal TotalCost { get; set; } = 0;
    }
}
