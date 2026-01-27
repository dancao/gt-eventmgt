using EventManagementAPI.ViewModels;

namespace EventManagementAPI.Services.Interfaces
{
    public interface IEventService
    {
        Task AddVenueAsync(VenueDto venue);
        Task<VenueDto> GetVenueByIdAsync(int id);
        Task<bool> UpdateVenueAsync(VenueDto venue);
        Task<bool> DeleteVenueAsync(int id);
    }
}
