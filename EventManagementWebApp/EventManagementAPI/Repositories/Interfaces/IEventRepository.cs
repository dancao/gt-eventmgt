using EventManagementAPI.Domain.Entities;

namespace EventManagementAPI.Repositories.Interfaces
{
    public interface IEventRepository
    {
        Task<Event> GetByIdAsync(long id);
        Task<List<Event>> GetAllAsync();
        Task<(List<Event> events, int totalCount)> GetTicketAvailabilityAsync(int pageNumber = 1, int pageSize = 10);
        Task AddAsync(Event evt);
        Task UpdateAsync(Event evt);
        Task DeleteAsync(Event evt);
        Task<bool> IsVenueAvailable(Event eventItem);
    }
}
