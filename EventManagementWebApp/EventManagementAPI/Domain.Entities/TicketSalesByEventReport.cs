namespace EventManagementAPI.Domain.Entities
{
    /// <summary>
    /// Enhancement: we can use generic to store Reports so we don't need to create new Report Type 
    /// </summary>
    public class TicketSalesByEventReport
    {
        public long Id { get; set; }
        public long EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public long TicketsCount { get; set; }
        public decimal TicketsTotalCost { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = "";
        public string ReportRequestId { get; set; } = "";
    }
}
