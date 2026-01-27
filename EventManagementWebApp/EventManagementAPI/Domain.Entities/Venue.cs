namespace EventManagementAPI.Domain.Entities
{
    public class Venue
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Address { get; set; } = "";
        public long Capacity { get; set; }
        public string CreatedBy { get; set; } = "System";
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public List<Event> Events { get; set; } = []; // one Venue can assign to many events as long as time is OK
    }
}
