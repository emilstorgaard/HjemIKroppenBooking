namespace HjemIKroppenBooking.Application.Abstractions.Persistence;

/// <summary>
/// Bookings are changed only through the session aggregate, so this repository only locates bookings.
/// </summary>
public interface IBookingRepository
{
    Task<BookingKey?> FindKeyAsync(Guid bookingId, CancellationToken cancellationToken);
}
