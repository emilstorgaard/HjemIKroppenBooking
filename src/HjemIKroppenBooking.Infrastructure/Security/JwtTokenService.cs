using System.Globalization;
using System.Security.Claims;
using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Domain.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace HjemIKroppenBooking.Infrastructure.Security;

/// <summary>Issues signed access tokens that carry only the user id (<c>sub</c>). The API reads the role from the database on every request.</summary>
internal sealed class JwtTokenService(IOptions<JwtOptions> options, TimeProvider timeProvider) : IJwtTokenService
{
    private static readonly JsonWebTokenHandler Handler = new();

    public AccessToken Create(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        var jwt = options.Value;
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var expires = now.AddHours(jwt.ExpiryHours);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = jwt.Issuer,
            Audience = jwt.Audience,
            IssuedAt = now,
            NotBefore = now,
            Expires = expires,
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString("D", CultureInfo.InvariantCulture)),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture))
            ]),
            SigningCredentials = new SigningCredentials(JwtSigning.CreateKey(jwt), JwtSigning.Algorithm)
        };

        return new AccessToken(Handler.CreateToken(descriptor), expires);
    }
}
