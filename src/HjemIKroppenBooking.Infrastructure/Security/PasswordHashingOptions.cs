using System.ComponentModel.DataAnnotations;

namespace HjemIKroppenBooking.Infrastructure.Security;

/// <summary>Bound from the <c>PasswordHashing</c> section. Each step of the work factor doubles the hashing time.</summary>
public sealed class PasswordHashingOptions
{
    public const string SectionName = "PasswordHashing";

    [Range(10, 16)]
    public int WorkFactor { get; set; } = 12;
}
