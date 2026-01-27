using EventManagementAPI.Commons;

namespace EventManagementAPI.Domain.Entities
{
    public class Event
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public long TotalTicketsAvail { get; set; }
        public long TotalSoldTicket { get; set; }
        public DateTime EventDate { get; set; }
        public int Duration { get; set; }
        public EventStatus EventStatus { get; set; } = EventStatus.Active;
        public string CreatedBy { get; set; } = "System";
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public long VenueId { get; set; }
        public Venue? Venue { get; set; }

        public PricingTier? PricingTier { get; set; }
        public long PricingTierId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
