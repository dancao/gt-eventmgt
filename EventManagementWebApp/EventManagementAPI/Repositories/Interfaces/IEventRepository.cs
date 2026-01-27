using EventManagementAPI.Domain.Entities;

namespace EventManagementAPI.Repositories.Interfaces
{
    public interface IEventRepository : IDisposable
    {
        Task<Event> GetByIdAsync(int id);
        Task<List<Event>> GetAllAsync();
        Task<int> AddAsync(Event venue);
        Task<bool> UpdateAsync(Event venue);
        Task<bool> DeleteAsync(int id);
        Task<List<Event>> GetActiveEventsByVenueIdAsync(int venueId);
    }
}
