using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TradingEngine.Infrastructure.Persistence;

namespace TradingPlatform.IntegrationTests;

public class TradingPlatformFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;
    public bool UseSqlite { get; set; } = false; // Toggle this to switch between Real DB and SQLite

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (UseSqlite)
        {
            // Create and open the connection to ensure the in-memory database persists
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            builder.ConfigureServices(services =>
            {
                // Aggressively remove all possible DbContext-related services
                var descriptors = services.Where(d =>
                    d.ServiceType == typeof(TradingDbContext) ||
                    d.ServiceType == typeof(DbContextOptions<TradingDbContext>) ||
                    d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true)
                    .ToList();

                foreach (var d in descriptors)
                {
                    services.Remove(d);
                }

                // Add SQLite using the persistent connection
                services.AddDbContext<TradingDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                });
            });
        }
        else
        {
            // Use Real Database from appsettings.json
            // We don't remove anything, so the default SQL Server registration stays active
            builder.UseEnvironment("Development");
        }
    }

    public async Task InitializeDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TradingDbContext>();

        if (UseSqlite)
        {
            await context.Database.EnsureCreatedAsync();
        }
        else
        {
            // For real DB, we use Migrate to ensure schema is up to date
            await context.Database.MigrateAsync();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection?.Dispose();
        }
        base.Dispose(disposing);
    }
}
