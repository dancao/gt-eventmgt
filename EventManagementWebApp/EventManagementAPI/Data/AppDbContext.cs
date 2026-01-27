using EventManagementAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventManagementAPI.Data
{
    public class AppDbContext: DbContext
    {
        public DbSet<Venue> Venues => Set<Venue>();
        public DbSet<PricingTier> PricingTiers => Set<PricingTier>();
        public DbSet<Event> Events => Set<Event>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Venue>().HasKey(t => t.Id);
            //modelBuilder.Entity<Venue>().Property(t => t.Name).IsRequired().HasMaxLength(50);

            //modelBuilder.Entity<PricingTier>().HasKey(t => t.Id);
            //modelBuilder.Entity<PricingTier>().Property(t => t.Name).IsRequired().HasMaxLength(50);

            //modelBuilder.Entity<Event>().HasKey(t => t.Id);
            //modelBuilder.Entity<Event>().Property(t => t.Name).IsRequired().HasMaxLength(50);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Venue)
                .WithMany(v => v.Events)
                .HasForeignKey(e => e.VenueId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.PricingTier)
                .WithMany(p => p.Events)
                .HasForeignKey(e => e.PricingTierId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
