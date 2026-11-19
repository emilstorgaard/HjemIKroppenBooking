using System.Globalization;
using System.Threading.RateLimiting;
using HjemIKroppenBooking.Api.ErrorHandling;
using HjemIKroppenBooking.Api.Options;
using HjemIKroppenBooking.Domain.Common;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace HjemIKroppenBooking.Api.Extensions;

/// <summary>
/// Fixed-window rate limits per client IP: a global limit for all endpoints and a stricter <see cref="AuthPolicy"/>
/// for login and registration. Rejected requests get 429 with <c>Retry-After</c> and a Danish problem response.
/// </summary>
public static class RateLimitingExtensions
{
    public const string AuthPolicy = "auth";
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<RateLimitingSettings>().Bind(configuration.GetSection(RateLimitingSettings.SectionName)).ValidateDataAnnotations().ValidateOnStart();
        services.AddRateLimiter(_ => { });
        services.AddOptions<RateLimiterOptions>().Configure<IOptions<RateLimitingSettings>>((options, settings) =>
        {
            var limits = settings.Value;
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context => FixedWindow(context, limits.GlobalPermitLimitPerMinute));
            options.AddPolicy(AuthPolicy, context => FixedWindow(context, limits.AuthPermitLimitPerMinute));
            options.OnRejected = WriteRejectedAsync;
        });
        return services;
    }

    /// <summary>Partitions by client IP. Behind a proxy this is correct only when the proxy is configured in <c>ForwardedHeaders</c>.</summary>
    private static RateLimitPartition<string> FixedWindow(HttpContext context, int permitLimit) =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions { PermitLimit = permitLimit, Window = Window, QueueLimit = 0 });

    private static async ValueTask WriteRejectedAsync(OnRejectedContext context, CancellationToken cancellationToken)
    {
        var httpContext = context.HttpContext;
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            httpContext.Response.Headers.RetryAfter = ((int)Math.Ceiling(retryAfter.TotalSeconds)).ToString(CultureInfo.InvariantCulture);
        }

        httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        var problem = Problems.Error(StatusCodes.Status429TooManyRequests, ErrorCodes.RequestRateLimited);
        await ProblemWriter.WriteAsync(httpContext, problem, cancellationToken);
    }
}
