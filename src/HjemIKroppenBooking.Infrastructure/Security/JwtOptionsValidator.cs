using System.Text;
using Microsoft.Extensions.Options;

namespace HjemIKroppenBooking.Infrastructure.Security;

/// <summary>Stops the application at startup if the signing key is too short or still a placeholder.</summary>
public sealed class JwtOptionsValidator : IValidateOptions<JwtOptions>
{
    public const int MinimumKeyBytes = 32;

    /// <summary>The placeholder used in <c>.env.example</c>. Must never reach a running system.</summary>
    public const string PlaceholderMarker = "CHANGE_ME";

    public ValidateOptionsResult Validate(string? name, JwtOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (Encoding.UTF8.GetByteCount(options.Key) < MinimumKeyBytes)
        {
            return ValidateOptionsResult.Fail("Jwt:Key must be at least " + MinimumKeyBytes + " bytes.");
        }

        if (options.Key.Contains(PlaceholderMarker, StringComparison.OrdinalIgnoreCase))
        {
            return ValidateOptionsResult.Fail("Jwt:Key is still the placeholder value. Set a random secret.");
        }

        return ValidateOptionsResult.Success;
    }
}
