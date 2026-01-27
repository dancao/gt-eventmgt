using EventManagementAPI.Domain.Entities;

namespace EventManagementAPI.Repositories.Interfaces
{
    public interface IVenueRepository
    {
        Task<Venue?> GetByIdAsync(long id);
        Task<List<Venue>> GetAllAsync();
        Task AddAsync(Venue venue);
        Task UpdateAsync(Venue venue);
        Task DeleteAsync(Venue venue);
    }
}
