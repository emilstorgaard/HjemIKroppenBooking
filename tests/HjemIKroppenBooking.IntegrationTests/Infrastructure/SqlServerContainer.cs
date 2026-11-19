using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace HjemIKroppenBooking.IntegrationTests.Infrastructure;

/// <summary>
/// One SQL Server container for the whole test run, started on first use. Each test class gets its own database in it,
/// so classes can run in parallel without seeing each other's data. Testcontainers removes the container when the run ends.
/// </summary>
internal static class SqlServerContainer
{
    private const string Image = "mcr.microsoft.com/mssql/server:2022-latest";
    private static readonly Lazy<Task<MsSqlContainer>> Container = new(StartAsync);

    /// <summary>A connection string to a new, not yet created database with a unique name.</summary>
    public static async Task<string> CreateDatabaseConnectionStringAsync()
    {
        var container = await Container.Value;
        var builder = new SqlConnectionStringBuilder(container.GetConnectionString()) { InitialCatalog = "booking_tests_" + Guid.NewGuid().ToString("N") };
        return builder.ConnectionString;
    }

    private static async Task<MsSqlContainer> StartAsync()
    {
        var container = new MsSqlBuilder(Image).Build();
        await container.StartAsync();
        return container;
    }
}
