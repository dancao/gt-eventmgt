
using EventManagementAPI.Commons;
using EventManagementAPI.Domain.Entities;
using EventManagementAPI.ViewModels;

namespace EventManagementAPI.Services.Interfaces
{
    public interface IReportingService
    {
        Task CreateJobAsync(string jobId);
        Task TicketSalesByEventReportAsync(ProcessReportRequest request);
        Task<ReportRequest> GetJobByIdAsync(string jobId);
        Task<List<TicketSalesByEventReport>> GetTicketSalesByEventReportAsync(string requestId);
    }
}
