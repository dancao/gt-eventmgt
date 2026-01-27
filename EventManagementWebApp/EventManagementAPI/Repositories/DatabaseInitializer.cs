using EventManagementAPI.Data;
using EventManagementAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventManagementAPI.Repositories
{
    public class DatabaseInitializer: IHostedService
    {
        private readonly ILogger<DatabaseInitializer> _logger;
        private readonly IServiceProvider _serviceProvider;

        public DatabaseInitializer(IServiceProvider serviceProvider, ILogger<DatabaseInitializer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Initializing SQLite database...");

            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            try
            {
                await dbContext.Database.EnsureCreatedAsync();
                _logger.LogInformation("Database created successfully.");
                // for Production migration
                //await dbContext.Database.MigrateAsync(cancellationToken); 
                //_logger.LogInformation("Database migrated successfully.");

                // Optional seed
                if (!await dbContext.PricingTiers.AnyAsync(cancellationToken))
                {
                    _logger.LogInformation("seeding PricingTier ...");
                    dbContext.PricingTiers.Add(new PricingTier()
                    {
                        Name = "Tier 1",
                        Price = 5
                    });
                    dbContext.PricingTiers.Add(new PricingTier()
                    {
                        Name = "Tier 2",
                        Price = (decimal)8.3
                    });
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
                if (!await dbContext.Venues.AnyAsync(cancellationToken))
                {
                    _logger.LogInformation("seeding Venue ...");
                    dbContext.Venues.Add(new Venue()
                    {
                        Name = "down town hall",
                        Address = "123 Main St.",
                        Capacity = 100
                    });
                    dbContext.Venues.Add(new Venue()
                    {
                        Name = "Chicago Stadium",
                        Address = "123 Main St.",
                        Capacity = 300
                    });
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database initialization failed.");
                throw;
            }

            _logger.LogInformation("SQLite database initialized successfully.");
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
