using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Users;

namespace HjemIKroppenBooking.Application.Features.Users.Common;

/// <summary>Shared by "update me" and the admin "update user": checks that a changed email is free.</summary>
internal static class UserProfileUpdater
{
    public static async Task UpdateAsync(IUserRepository users, User user, string fullName, string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = User.NormalizeEmail(email);
        if (!string.Equals(normalizedEmail, user.Email, StringComparison.Ordinal) && await users.EmailExistsAsync(normalizedEmail, user.Id, cancellationToken))
        {
            throw DomainException.Conflict(ErrorCodes.UserEmailAlreadyExists);
        }

        user.UpdateProfile(fullName, normalizedEmail);
    }
}
