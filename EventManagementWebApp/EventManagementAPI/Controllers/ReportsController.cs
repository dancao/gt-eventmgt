using EventManagementAPI.Commons;
using EventManagementAPI.Helpers;
using EventManagementAPI.Services.Interfaces;
using EventManagementAPI.ViewModels;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportingService _reportingService;
        private readonly IBackgroundJobClient _jobs;

        public ReportsController(IReportingService reportingService, IBackgroundJobClient jobs)
        {
            _reportingService = reportingService;
            _jobs = jobs;
        }

        [HttpPost]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status201Created)]
        public async Task<IActionResult> StartProcessing([FromQuery] string reportName)
        {
            var requestId = Guid.NewGuid().ToString();
            await _reportingService.CreateJobAsync(requestId);

            _jobs.Enqueue<IReportingService>(job => 
                job.TicketSalesByEventReportAsync(new ProcessReportRequest() { RequestId = requestId, ReportName = reportName }));

            return AcceptedAtAction(nameof(GetStatusByJobId), new { id = requestId }, null);
        }

        [HttpGet("status/{id}", Name = "GetStatusByJobId")]
        public async Task<IActionResult> GetStatusByJobId(string id)
        {
            var job = await _reportingService.GetJobByIdAsync(id);

            if (job == null) return NotFound();

            if (job.Status == JobStatus.Completed)
            {
                return Ok(new { Status = "Completed", ResultUrl = $"results/{id}" });
            }

            return Ok(new { Status = job.Status.ToString(), EstimatedTimeSeconds = 30 });
        }

        [HttpGet("results/{id}", Name = "GetResults")]
        public async Task<IActionResult> GetResults(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest("ID is required.");

            var reportData = await _reportingService.GetTicketSalesByEventReportAsync(id);
            var apiResponse = EventMgtSingleton.Instance.GetApiResponse(reportData, ApiResponseStatus.Success, "");
            return Ok(apiResponse);
        }
    }
}
