using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HjemIKroppenBooking.Infrastructure.Persistence;

/// <summary>
/// Used only by the <c>dotnet ef</c> tools, so migrations can be generated with this project as the startup
/// project. Generating a migration or an idempotent script does not connect to the database; the connection string
/// is only used by commands such as <c>database update</c> and is read from <c>ConnectionStrings__Default</c>.
/// </summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string PlaceholderConnectionString = "Server=localhost;Database=HjemIKroppenBooking;Integrated Security=true;TrustServerCertificate=true";

    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default") ?? PlaceholderConnectionString;
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(connectionString).Options;
        return new AppDbContext(options, TimeProvider.System);
    }
}
