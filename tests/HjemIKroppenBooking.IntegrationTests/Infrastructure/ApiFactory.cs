using HjemIKroppenBooking.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Time.Testing;
using Microsoft.IdentityModel.Tokens;

namespace HjemIKroppenBooking.IntegrationTests.Infrastructure;

/// <summary>
/// The real API on a test server, against its own SQL Server database created from the migrations. Time is controlled
/// by <see cref="Time"/>; it only moves forward, so tests create their sessions relative to the current fake time.
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public static readonly DateTimeOffset StartTime = new(2030, 1, 7, 9, 0, 0, TimeSpan.Zero);
    private static readonly TimeSpan ClockSkew = TimeSpan.FromMinutes(1);
    private string _connectionString = string.Empty;

    public FakeTimeProvider Time { get; } = new(StartTime);

    public async ValueTask InitializeAsync()
    {
        _connectionString = await SqlServerContainer.CreateDatabaseConnectionStringAsync();
        await using var scope = Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync(TestContext.Current.CancellationToken);
    }

    public override async ValueTask DisposeAsync()
    {
        await using (var scope = Services.CreateAsyncScope())
        {
            await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureDeletedAsync();
        }

        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:Default", _connectionString);
        builder.UseSetting("Jwt:Key", "integration-tests-signing-key-that-is-long-enough-0123456789");
        builder.UseSetting("AllowedOrigins:0", "http://localhost:5173");
        builder.UseSetting("PasswordHashing:WorkFactor", "10");
        builder.UseSetting("RateLimiting:GlobalPermitLimitPerMinute", "100000");
        builder.UseSetting("RateLimiting:AuthPermitLimitPerMinute", "1000");
        builder.UseSetting("ForwardedHeaders:KnownProxies:0", "127.0.0.1");
        builder.UseSetting("Logging:LogLevel:Default", "Warning");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<TimeProvider>();
            services.AddSingleton<TimeProvider>(Time);

            // Tokens are issued with the fake time, so their lifetime must be validated against it too.
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options => options.TokenValidationParameters.LifetimeValidator = ValidateLifetime);
        });
    }

    private bool ValidateLifetime(DateTime? notBefore, DateTime? expires, SecurityToken token, TokenValidationParameters parameters)
    {
        var now = Time.GetUtcNow().UtcDateTime;
        return (notBefore is null || notBefore.Value <= now + ClockSkew) && (expires is null || expires.Value > now - ClockSkew);
    }
}
