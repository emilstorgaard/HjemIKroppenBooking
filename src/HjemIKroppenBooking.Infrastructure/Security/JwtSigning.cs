using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace HjemIKroppenBooking.Infrastructure.Security;

/// <summary>The single place that turns the configured key into a signing key, used for issuing and validating tokens.</summary>
public static class JwtSigning
{
    public const string Algorithm = SecurityAlgorithms.HmacSha256;

    /// <summary>Claim type that carries the user's role. The API maps it as the role claim when validating.</summary>
    public const string RoleClaimType = "role";

    public static SymmetricSecurityKey CreateKey(JwtOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));
    }
}
