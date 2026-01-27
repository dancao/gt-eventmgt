using Newtonsoft.Json;

namespace EventManagementAPI.Domain.Entities
{
    public class TicketType
    {
        public long Id { get; set; }
        public string Name { get; set; } = "General Admission";

        public int TotalAvailable { get; set; }  // Initial inventory

        public int Remaining { get; set; }

        public long EventId { get; set; }
        public Event Event { get; set; } = null!;

        public PricingTier PricingTier { get; set; } = null!;

        public long PricingTierId { get; set; }

        public List<Ticket> Tickets { get; set; } = [];
    }
}
