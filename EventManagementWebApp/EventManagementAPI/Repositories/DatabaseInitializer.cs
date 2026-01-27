using EventManagementAPI.Data;
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
                //await dbContext.Database.MigrateAsync(cancellationToken); // for Production migration
                _logger.LogInformation("Database migrated successfully.");
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
