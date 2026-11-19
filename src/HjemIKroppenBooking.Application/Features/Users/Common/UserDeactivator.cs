using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Domain.Users;

namespace HjemIKroppenBooking.Application.Features.Users.Common;

/// <summary>
/// Deactivates a user and cancels their bookings on sessions that have not started, through the session aggregate.
/// Used both when users deactivate themselves and when an admin deactivates them.
/// </summary>
public sealed class UserDeactivator(ISessionRepository sessions, IUnitOfWork unitOfWork, TimeProvider timeProvider)
{
    /// <summary>Returns the number of cancelled bookings. Calling it again on a deactivated user cleans up leftovers.</summary>
    public async Task<int> DeactivateAsync(User user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var upcoming = await sessions.GetUpcomingBookedByUserAsync(user.Id, now, cancellationToken);

        foreach (var session in upcoming)
        {
            var booking = session.FindActiveBookingFor(user.Id);
            if (booking is not null)
            {
                session.CancelBooking(booking.Id, now);
            }
        }

        user.Deactivate();

        // One save, so the deactivation and all cancellations succeed or fail together.
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return upcoming.Count;
    }
}
