using EventManagementAPI.Commons;

namespace EventManagementAPI.ViewModels
{
    public class TicketAvailabilityDto
    {
        public long EventId { get; set; }
        public string EventName { get; set; } = "";
        public DateTime EventDate { get; set; }
        public string VenueName { get; set; } = "";
        public EventStatus EventStatus { get; set; } = EventStatus.Active;
        public List<TicketTypeLiteDto> TicketTypes { get; set; } = [];
    }

    public class TicketAvailabilityPayLoad
    {
        public List<TicketAvailabilityDto> Events { get; set; } = [];
        public int TotalCount { get; set; } = 0;
    }
}
