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

        Task AddEventAsync(EventDto eventDto);
        Task<EventDto> GetEventByIdAsync(long id, bool includeVenue = false, bool includeTicketTypes = false);
        Task<List<EventDto>> GetAllEventsAsync();
        Task<List<VenueDto>> SearchVenuesAsync(string venueName, string desc, int minCapacity, int maxCapacity, bool isActive = true);
    }
}
