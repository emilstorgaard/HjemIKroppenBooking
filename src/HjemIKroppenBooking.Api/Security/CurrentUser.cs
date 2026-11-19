using System.Globalization;
using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Domain.Common;
using Microsoft.IdentityModel.JsonWebTokens;

namespace HjemIKroppenBooking.Api.Security;

/// <summary>Reads the user id from the <c>sub</c> claim of the validated access token.</summary>
internal sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid UserId
    {
        get
        {
            var subject = httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return Guid.TryParse(subject, CultureInfo.InvariantCulture, out var userId) ? userId : throw DomainException.Unauthorized(ErrorCodes.AuthUnauthorized);
        }
    }
}
