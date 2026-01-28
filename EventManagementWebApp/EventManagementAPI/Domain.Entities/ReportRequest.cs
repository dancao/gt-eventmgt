
using EventManagementAPI.Commons;

namespace EventManagementAPI.Domain.Entities
{
    public class ReportRequest
    {
        public string Id { get; set; } = "";

        public JobStatus Status { get; set; } = JobStatus.Pending;

        public string Result { get; set; } = "";

        public DateTime CreatedAt { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}
