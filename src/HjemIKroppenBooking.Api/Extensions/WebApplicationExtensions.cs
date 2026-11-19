using HjemIKroppenBooking.Api.Middleware;
using HjemIKroppenBooking.Api.Options;
using HjemIKroppenBooking.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

namespace HjemIKroppenBooking.Api.Extensions;

public static partial class WebApplicationExtensions
{
    public static WebApplication UseApi(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);
        WarnIfNoProxyIsTrusted(app);

        // Order matters: the client IP must be known before logging and rate limiting, and errors from every later
        // step must reach the exception handler. HTTPS is terminated by the reverse proxy, so there is no HTTPS redirection or HSTS.
        app.UseForwardedHeaders();
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<SecurityHeadersMiddleware>();
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseRateLimiter();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHealthEndpoints();
        app.MapApiDocumentation();
        return app;
    }

    /// <summary>Liveness checks only the process; readiness also checks that SQL Server answers.</summary>
    private static void MapHealthEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false }).AllowAnonymous().DisableRateLimiting();
        app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains(DependencyInjection.ReadyHealthCheckTag) })
            .AllowAnonymous()
            .DisableRateLimiting();
    }

    /// <summary>OpenAPI at <c>/openapi/v1.json</c> and Scalar at <c>/scalar</c>, in Development or when <c>OpenApi:Enabled</c> is true.</summary>
    private static void MapApiDocumentation(this WebApplication app)
    {
        var enabled = app.Environment.IsDevelopment() || app.Services.GetRequiredService<IOptions<OpenApiSettings>>().Value.Enabled;
        if (!enabled)
        {
            return;
        }

        app.MapOpenApi().AllowAnonymous();
        app.MapScalarApiReference().AllowAnonymous();
    }

    private static void WarnIfNoProxyIsTrusted(WebApplication app)
    {
        if (!app.Services.GetRequiredService<IOptions<ForwardedHeadersSettings>>().Value.IsConfigured)
        {
            LogNoTrustedProxy(app.Logger);
        }
    }

    [LoggerMessage(EventId = 5100, Level = LogLevel.Warning, Message = "No trusted proxies are configured (ForwardedHeaders:KnownProxies/KnownNetworks). Behind a reverse proxy, client IPs and rate limits will use the proxy's address.")]
    private static partial void LogNoTrustedProxy(ILogger logger);
}
