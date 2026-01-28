using EventManagementAPI.Data;
using EventManagementAPI.Domain.Entities;
using EventManagementAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventManagementAPI.Repositories
{
    public class TicketRepository: ITicketRepository
    {
        private readonly AppDbContext _dbContext;

        public TicketRepository(AppDbContext context)
        {
            _dbContext = context;
        }

        public async Task AddTicketAsync(Ticket ticket)
        {
            await _dbContext.Tickets.AddAsync(ticket);
        }

        public async Task<int> UpdateTicketTypeAsync(long ticketTypeId, int Quantity)
        {
            return await _dbContext.TicketTypes
                    .Where(tt => tt.Id == ticketTypeId && tt.Remaining >= Quantity)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(tt => tt.Remaining, tt => tt.Remaining - Quantity));
        }
    }
}
