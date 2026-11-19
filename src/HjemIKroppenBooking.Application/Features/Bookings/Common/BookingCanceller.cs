using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Domain.Bookings;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Bookings.Common;

/// <summary>Cancels a single booking through its session aggregate.</summary>
public sealed class BookingCanceller(IBookingRepository bookings, ISessionRepository sessions, IUnitOfWork unitOfWork, TimeProvider timeProvider)
{
    /// <param name="ownerUserId">When set, bookings of other users are reported as not found.</param>
    public async Task CancelAsync(Guid bookingId, Guid? ownerUserId, CancellationToken cancellationToken)
    {
        var key = await bookings.FindKeyAsync(bookingId, cancellationToken);
        if (key is null || (ownerUserId is { } owner && key.UserId != owner))
        {
            throw DomainException.NotFound(ErrorCodes.BookingNotFound);
        }

        if (key.Status == BookingStatus.Cancelled)
        {
            throw DomainException.Conflict(ErrorCodes.BookingAlreadyCancelled);
        }

        var session = await sessions.GetByIdAsync(key.SessionId, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.BookingNotFound);
        session.CancelBooking(key.Id, timeProvider.GetUtcNow().UtcDateTime);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
