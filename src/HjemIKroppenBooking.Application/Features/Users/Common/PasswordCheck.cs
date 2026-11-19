using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Users;

namespace HjemIKroppenBooking.Application.Features.Users.Common;

internal static class PasswordCheck
{
    /// <summary>
    /// Verifies the user's current password. A wrong password is reported as a validation error on the given field,
    /// not as 401, so the frontend does not mistake it for an expired token.
    /// </summary>
    public static void EnsureMatches(IPasswordHasher passwordHasher, User user, string password, string field)
    {
        if (!passwordHasher.Verify(password, user.PasswordHash))
        {
            throw DomainException.Validation(ErrorCodes.AuthWrongPassword, new Dictionary<string, object?> { ["field"] = field });
        }
    }
}
