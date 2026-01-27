using EventManagementAPI.Commons;
using EventManagementAPI.Data;
using EventManagementAPI.Domain.Entities;
using EventManagementAPI.Repositories.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EventManagementAPI.Repositories
{
    public class EventRepository: IEventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Event>> GetActiveEventsByVenueIdAsync(int venueId)
        {
            return await _context.Events
                .AsNoTracking()
                .Where(x => x.Venue != null && x.Venue.Id == venueId && x.EventStatus == EventStatus.Finished)
                .OrderByDescending(t => t.CreatedOn)
                .ToListAsync();
        }
        public async Task<int> AddAsync(Event venue)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<List<Event>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Event> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Event venue)
        {
            throw new NotImplementedException();
        }
    }
}
