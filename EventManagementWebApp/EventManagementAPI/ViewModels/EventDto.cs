using EventManagementAPI.Domain.Entities;

namespace EventManagementAPI.ViewModels
{
    public class EventDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public long TotalTicketsAvail { get; set; }
        public long TotalSoldTicket { get; set; }
        public DateTime EventDate { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public TimeSpan Duration => EndTime - StartTime;
        public Venue? Venue { get; set; }
        public string CreatedBy { get; set; } = "System";
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
