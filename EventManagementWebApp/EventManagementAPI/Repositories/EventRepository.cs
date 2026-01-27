using EventManagementAPI.Data;
using EventManagementAPI.Domain.Entities;
using EventManagementAPI.Repositories.Interfaces;
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

        public async Task<List<Event>> GetAllAsync()
        {
            return await _context.Events
                .AsNoTracking()
                .Include(x => x.Venue)
                .Include(x => x.TicketTypes)
                .ToListAsync();
        }

        public async Task<(List<Event> events, int totalCount)> GetTicketAvailabilityAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if(pageSize > 20) pageSize = 10;

            var query = _context.Events
                .AsNoTracking()
                .Include(x => x.Venue)
                .Include(x => x.TicketTypes)
                .AsQueryable();

            query = query.Where(evt => evt.IsActive && evt.EventStatus == Commons.EventStatus.Active);

            query = query.OrderBy(evt => evt.CreatedOn).ThenBy(evt => evt.Name);

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Event> GetByIdAsync(long id)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(x => x.Venue)
                .Include(x => x.TicketTypes).ThenInclude(tt => tt.PricingTier)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new Exception();
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
