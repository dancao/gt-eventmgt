using EventManagementAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventManagementAPI.Data
{
    public class AppDbContext: DbContext
    {
        public DbSet<Venue> Venues => Set<Venue>();
        public DbSet<PricingTier> PricingTiers => Set<PricingTier>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<TicketType> TicketTypes => Set<TicketType>();
        public DbSet<Ticket> Tickets => Set<Ticket>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Venue)
                .WithMany(v => v.Events)
                .HasForeignKey(e => e.VenueId)
                .OnDelete(DeleteBehavior.Cascade);   // Delete Events if Venue deleted

            modelBuilder.Entity<TicketType>()
            .HasOne(tt => tt.Event)
            .WithMany(e => e.TicketTypes)
            .HasForeignKey(tt => tt.EventId)
            .OnDelete(DeleteBehavior.Cascade);  // Delete ticket types if event deleted

            modelBuilder.Entity<TicketType>()
            .HasOne(tt => tt.PricingTier)
            .WithMany(pt => pt.TicketTypes)
            .HasForeignKey(tt => tt.PricingTierId)
            .OnDelete(DeleteBehavior.Cascade);   // Delete ticket types if pricing tier deleted

            modelBuilder.Entity<TicketType>()
            .HasMany(tt => tt.Tickets)
            .WithOne(t => t.TicketType)
            .HasForeignKey(t => t.TicketTypeId);
        }
    }
}
