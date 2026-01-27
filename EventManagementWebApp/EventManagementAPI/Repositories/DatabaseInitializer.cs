using EventManagementAPI.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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
                await dbContext.Database.MigrateAsync(cancellationToken);
                _logger.LogInformation("Database migrated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database initialization failed.");
                throw;
            }

            //await using var conn = new SqliteConnection(_connectionString);
            //await conn.OpenAsync();

            //_logger.LogInformation("Initializing Venues table...");
            //await using var cmd = conn.CreateCommand();
            //cmd.CommandText = """
            //CREATE TABLE IF NOT EXISTS Venues (
            //    Id              INTEGER PRIMARY KEY AUTOINCREMENT,
            //    Name            TEXT NOT NULL,
            //    Description     TEXT NULL,
            //    Capacity        INTEGER NOT NULL DEFAULT 0,
            //    CreatedBy       TEXT NOT NULL,
            //    CreatedOn       TEXT NOT NULL DEFAULT (datetime('now'))
            //);
            //""";
            //await cmd.ExecuteNonQueryAsync();

            //_logger.LogInformation("Initializing Events table...");
            //await using var cmdEvents = conn.CreateCommand();
            //cmdEvents.CommandText = """
            //CREATE TABLE IF NOT EXISTS Events (
            //    Id                          INTEGER PRIMARY KEY AUTOINCREMENT,
            //    Name                        TEXT NOT NULL,
            //    Description                 TEXT NULL,
            //    TotalTicketsAvail           INTEGER NOT NULL DEFAULT 0,
            //    TotalSoldTicket             INTEGER NOT NULL DEFAULT 0,
            //    EventStartDate              TEXT NOT NULL,
            //    EventEndDate                TEXT NOT NULL,
            //    VenueId                     INTEGER NOT NULL,
            //    PricingTier                 INTEGER NOT NULL,
            //    EventStatus                 TEXT NOT NULL,
            //    CreatedBy                   TEXT NOT NULL,
            //    CreatedOn                   TEXT NOT NULL DEFAULT (datetime('now'))
            //);
            //""";
            //await cmdEvents.ExecuteNonQueryAsync();

            _logger.LogInformation("SQLite database initialized successfully.");
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
