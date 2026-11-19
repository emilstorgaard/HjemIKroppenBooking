using System.ComponentModel.DataAnnotations;

namespace HjemIKroppenBooking.Api.Options;

/// <summary>Requests per client IP per minute, bound from the <c>RateLimiting</c> section.</summary>
public sealed class RateLimitingSettings
{
    public const string SectionName = "RateLimiting";

    /// <summary>Limit for all endpoints together, except health checks.</summary>
    [Range(1, 100_000)]
    public int GlobalPermitLimitPerMinute { get; set; } = 300;

    /// <summary>Stricter limit for login and registration, to slow down password guessing.</summary>
    [Range(1, 1_000)]
    public int AuthPermitLimitPerMinute { get; set; } = 10;
}
