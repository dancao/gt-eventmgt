namespace EventManagementAPI.Domain.Entities
{
    public class Ticket
    {
        public long Id { get; set; }
        public string BuyerName { get; set; } = "";
        public int Quanlity { get; set; }
        public decimal TotalCost { get; set; } = 0;
        public DateTime PurchasedDate { get; set; }

        public long TicketTypeId { get; set; }
        public TicketType TicketType { get; set; } = null!;
    }
}
