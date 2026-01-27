using EventManagementAPI.Data;
using EventManagementAPI.Domain.Entities;
using EventManagementAPI.Repositories.Interfaces;
using EventManagementAPI.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EventManagementAPI.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Event evt)
        {
            await _context.Events.AddAsync(evt);
        }

        public async Task DeleteAsync(Event evt)
        {
            _context.Events.Remove(evt);
        }

        public Task<List<Event>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Event> GetByIdAsync(long id)
        {
            return await _context.Events.FirstOrDefaultAsync(x => x.Id == id) ?? throw new Exception();
        }

        public async Task<bool> IsVenueAvailable(Event eventItem)
        {
            var eventEndDate = eventItem.EventDate.AddDays(eventItem.Duration);

            var result = await _context.Events.AnyAsync(x => x.IsActive && x.VenueId == eventItem.VenueId &&
                    x.EventStatus == Commons.EventStatus.Active &&
                    ((eventItem.EventDate >= x.EventDate && eventItem.EventDate < x.EventDate.AddDays(x.Duration)) ||
                     (eventEndDate >= x.EventDate && eventEndDate < x.EventDate.AddDays(x.Duration)))
                    );
            return !result;
        }

        public Task UpdateAsync(Event evt)
        {
            _context.Events.Update(evt);
            return Task.CompletedTask;
        }
    }
}
