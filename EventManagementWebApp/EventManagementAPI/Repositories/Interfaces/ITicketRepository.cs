using EventManagementAPI.Domain.Entities;

namespace EventManagementAPI.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        Task<int> UpdateTicketTypeAsync(long ticketTypeId,  int Quantity);
        Task AddTicketAsync(Ticket ticket);
    }
}
