using EventManagementAPI.Data;
using EventManagementAPI.Domain.Entities;
using EventManagementAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventManagementAPI.Repositories
{
    public class VenueRepository : IVenueRepository
    {
        private readonly AppDbContext _context;

        public VenueRepository(AppDbContext context) 
        {
            _context = context;
        }

        public async Task AddAsync(Venue venue)
        {
            await _context.Venues.AddAsync(venue);
        }

        public Task DeleteAsync(Venue venue)
        {
            _context.Venues.Remove(venue);
            return Task.CompletedTask;
        }

        public async Task<List<Venue>> GetAllAsync()
        {
            return await _context.Venues
                .AsNoTracking()
                .OrderByDescending(t => t.CreatedOn)
                .ToListAsync();
        }

        public async Task<Venue?> GetByIdAsync(long id)
        {
            return await _context.Venues
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public Task UpdateAsync(Venue venue)
        {
            _context.Venues.Update(venue);
            return Task.CompletedTask;
        }
    }
}
