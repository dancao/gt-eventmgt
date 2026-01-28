using EventManagementAPI.Data;
using EventManagementAPI.Domain.Entities;
using EventManagementAPI.Services.Interfaces;
using EventManagementAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EventManagementAPI.Services
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContext _dbContext;

        public TicketService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> PurchaseTicketAsync(PurchaseTicketDto purchaseTicketDto)
        {
            if (purchaseTicketDto == null || purchaseTicketDto.Quantity <= 0) throw new ArgumentOutOfRangeException(nameof(purchaseTicketDto));

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var updatedRows = await _dbContext.TicketTypes
                    .Where(tt => tt.Id == purchaseTicketDto.TicketTypeId && tt.Remaining >= purchaseTicketDto.Quantity)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(tt => tt.Remaining, tt => tt.Remaining - purchaseTicketDto.Quantity));

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
                _dbContext.Tickets.Add(ticket);

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
