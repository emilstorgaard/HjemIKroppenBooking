using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Users.Common;

internal static class OwnAccountGuard
{
    /// <summary>
    /// Admins may not change their own role, active status or password through the admin endpoints. This prevents
    /// accidental lock-out and stops a stolen token from resetting a password without knowing the current one.
    /// </summary>
    public static void EnsureNotSelf(ICurrentUser currentUser, Guid targetUserId)
    {
        if (currentUser.UserId == targetUserId)
        {
            throw DomainException.Conflict(ErrorCodes.UserCannotChangeOwnAccount);
        }
    }
}
