using EventManagementAPI.ViewModels;

namespace EventManagementAPI.Services.Interfaces
{
    public interface ITicketService
    {
        Task<bool> PurchaseTicketAsync(PurchaseTicketDto purchaseTicketDto);
    }
}
