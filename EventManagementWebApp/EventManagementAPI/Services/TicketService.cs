using EventManagementAPI.Data;
using EventManagementAPI.Domain.Entities;
using EventManagementAPI.Repositories.Interfaces;
using EventManagementAPI.Services.Interfaces;
using EventManagementAPI.ViewModels;

namespace EventManagementAPI.Services
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContext _dbContext;
        private readonly ITicketRepository _ticketRepository;

        public TicketService(AppDbContext dbContext, ITicketRepository ticketRepository)
        {
            _dbContext = dbContext;
            _ticketRepository = ticketRepository;
        }

        public async Task<bool> PurchaseTicketAsync(PurchaseTicketDto purchaseTicketDto)
        {
            if (purchaseTicketDto == null || purchaseTicketDto.Quantity <= 0) throw new ArgumentOutOfRangeException(nameof(purchaseTicketDto));

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var updatedRows = await _ticketRepository.UpdateTicketTypeAsync(purchaseTicketDto.TicketTypeId, purchaseTicketDto.Quantity);

                if (updatedRows == 0)
                {
                    // Insufficient inventory or concurrent update - Oversell prevented
                    await transaction.RollbackAsync();
                    return false;
                }

                var ticket = new Ticket
                {
                    TicketTypeId = purchaseTicketDto.TicketTypeId,
                    BuyerName = purchaseTicketDto.BuyerName,
                    PurchasedDate = DateTime.UtcNow,
                    Quanlity = purchaseTicketDto.Quantity,
                    TotalCost = purchaseTicketDto.TotalCost
                };
                await _ticketRepository.AddTicketAsync(ticket);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
