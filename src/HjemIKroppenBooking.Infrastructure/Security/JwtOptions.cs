using System.ComponentModel.DataAnnotations;

namespace HjemIKroppenBooking.Infrastructure.Security;

/// <summary>JWT settings, bound from the <c>Jwt</c> section (environment variables <c>Jwt__Key</c> etc.).</summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    /// <summary>HMAC-SHA256 signing key. Must be at least 32 bytes (256 bits) as UTF-8.</summary>
    [Required]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Range(1, 24)]
    public int ExpiryHours { get; set; } = 12;
}
