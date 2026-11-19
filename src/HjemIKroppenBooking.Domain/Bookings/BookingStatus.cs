namespace HjemIKroppenBooking.Domain.Bookings;

/// <summary>Status of a <see cref="Booking"/>.</summary>
public enum BookingStatus
{
    /// <summary>The booking holds a spot on the session.</summary>
    Active = 0,

    /// <summary>The booking was cancelled and its spot released. Kept for history.</summary>
    Cancelled = 1
}
