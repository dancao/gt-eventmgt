using EventManagementAPI.Commons;
using EventManagementAPI.Data;
using EventManagementAPI.Domain.Entities;
using EventManagementAPI.Services.Interfaces;
using EventManagementAPI.ViewModels;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace EventManagementAPI.Services
{
    public class ReportingService: IReportingService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<ReportingService> _logger;

        public ReportingService(AppDbContext context, ILogger<ReportingService> logger)
        {
            _dbContext = context;
            _logger = logger;
        }

        public async Task CreateJobAsync(string jobId)
        {
            await _dbContext.BackgroundJobs.AddAsync(new ReportRequest() { 
                Id = jobId,
                Status = JobStatus.Pending,
                CreatedAt = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ReportRequest> GetJobByIdAsync(string jobId)
        {
            var record = await _dbContext.BackgroundJobs.FindAsync(jobId);
            return record ?? null!;
        }

        public async Task<List<TicketSalesByEventReport>> GetTicketSalesByEventReportAsync(string requestId)
        {
            var record = await _dbContext.BackgroundJobs.FindAsync(requestId);
            if (record == null) throw new ArgumentException("requestId is not existed.");
            if(record.Status != JobStatus.Completed) throw new ArgumentException("Report is not ready yet.");

            return await _dbContext.TicketSalesByEventReports.Where(r => r.ReportRequestId == requestId)
                .OrderBy(r => r.EventName)
                .ToListAsync();
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task TicketSalesByEventReportAsync(ProcessReportRequest request)
        {
            _logger.LogInformation("Starting report for request {RequestId}", request.RequestId);

            if (string.Equals(request.ReportName, ReportNameConstants.TicketSalesByEventReport, StringComparison.OrdinalIgnoreCase))
            {
                var reportData = await _dbContext.Events.Where(x => x.CreatedOn >= DateTime.UtcNow.AddYears(-1))
                                    .Include(x => x.TicketTypes).ThenInclude(x => x.Tickets)
                                    .Select(evt => new TicketSalesByEventReport
                                    {
                                        EventId = evt.Id,
                                        EventName = evt.Name,
                                        TicketsCount = evt.TicketTypes.Sum(x => x.Tickets.Sum(t => t.Quanlity)),
                                        TicketsTotalCost = evt.TicketTypes.Sum(x => x.Tickets.Sum(t => t.TotalCost)),
                                        CreatedDate = DateTime.UtcNow,
                                        CreatedBy = nameof(TicketSalesByEventReportAsync),
                                        ReportRequestId = request.RequestId
                                    }).ToListAsync();

                await _dbContext.TicketSalesByEventReports.AddRangeAsync(reportData);
                await _dbContext.SaveChangesAsync();
            }

            // Update status in DB (for polling / websocket later)
            var record = await _dbContext.BackgroundJobs.FindAsync(request.RequestId);
            if (record != null)
            {
                record.Status = JobStatus.Completed;
                record.CompletedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }

            _logger.LogInformation("Report {RequestId} completed", request.RequestId);
        }
    }
}
