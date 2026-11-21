using System.Globalization;
using System.Security.Claims;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace HjemIKroppenBooking.Api.Security;

/// <summary>
/// Configures token validation from the same <see cref="JwtOptions"/> that issue the tokens. After the signature is
/// validated, the user must still exist and be active, and the role is taken from the database, so deactivating a
/// user or changing their role takes effect on the next request.
/// </summary>
internal sealed class ConfigureJwtBearerOptions(IOptions<JwtOptions> jwtOptions) : IConfigureNamedOptions<JwtBearerOptions>
{
    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name == JwtBearerDefaults.AuthenticationScheme)
        {
            Configure(options);
        }
    }

    public void Configure(JwtBearerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        var jwt = jwtOptions.Value;
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = JwtSigning.CreateKey(jwt),
            ValidAlgorithms = [JwtSigning.Algorithm],
            NameClaimType = JwtRegisteredClaimNames.Sub,
            RoleClaimType = JwtSigning.RoleClaimType,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
        options.Events = new JwtBearerEvents { OnTokenValidated = ValidateActiveUserAsync };
    }

    private static async Task ValidateActiveUserAsync(TokenValidatedContext context)
    {
        var subject = context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(subject, CultureInfo.InvariantCulture, out var userId))
        {
            context.Fail("The token has no valid subject.");
            return;
        }

        var users = context.HttpContext.RequestServices.GetRequiredService<IUserQueries>();
        var user = await users.GetByIdAsync(userId, context.HttpContext.RequestAborted);
        if (user is not { IsActive: true })
        {
            context.Fail("The user does not exist or is deactivated.");
            return;
        }

        // The role comes from the database, not the token. Remove any role claim in tokens issued before this change.
        var identity = (ClaimsIdentity)context.Principal!.Identity!;
        foreach (var claim in identity.FindAll(JwtSigning.RoleClaimType).ToList()) identity.RemoveClaim(claim);
        identity.AddClaim(new Claim(JwtSigning.RoleClaimType, user.Role.ToString()));
    }
}
