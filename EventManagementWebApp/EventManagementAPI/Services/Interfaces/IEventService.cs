using EventManagementAPI.ViewModels;

namespace EventManagementAPI.Services.Interfaces
{
    public interface IEventService
    {
        Task AddVenueAsync(VenueDto venue);
        Task<VenueDto> GetVenueByIdAsync(int id);
        Task<bool> UpdateVenueAsync(VenueDto venue);
        Task<bool> DeleteVenueAsync(int id);
        Task<List<VenueDto>> GetVenuesAsync();
        Task<List<VenueDto>> SearchVenuesAsync(string venueName, string desc, int minCapacity, int maxCapacity, bool isActive = true);

        Task AddEventAsync(EventDto eventDto);
        Task<bool> UpdateEventAsync(EventDto eventDto);
        Task<bool> DeleteEventAsync(long id);
        Task<EventDto> GetEventByIdAsync(long id, bool includeVenue = false, bool includeTicketTypes = false);
        Task<List<EventDto>> GetAllEventsAsync();
        Task<(List<TicketAvailabilityDto> ticketAvailabilities, int totalCount)> GetTicketAvailabilityAsync(int pageNumber = 1, int pageSize = 10);
    }
}
