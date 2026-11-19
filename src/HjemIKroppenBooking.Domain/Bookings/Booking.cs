using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Sessions;
using HjemIKroppenBooking.Domain.Users;

namespace HjemIKroppenBooking.Domain.Bookings;

/// <summary>
/// A user's booking of a <see cref="Sessions.Session"/>. Bookings are created and cancelled only through the
/// session aggregate, which keeps <see cref="Session.ActiveBookingCount"/> in sync. Cancelled bookings are kept
/// for history.
/// </summary>
public sealed class Booking : Entity
{
    public const int NoteMaxLength = 1000;

    private Booking()
    {
    }

    internal Booking(Guid sessionId, Guid userId, string? note)
    {
        SessionId = sessionId;
        UserId = userId;
        Status = BookingStatus.Active;
        Note = Guard.OptionalText(note, NoteMaxLength, "note");
    }

    public Guid SessionId { get; private set; }

    /// <summary>Navigation for queries. Not required to be loaded for domain logic.</summary>
    public Session Session { get; private set; } = null!;

    public Guid UserId { get; private set; }

    /// <summary>Navigation for queries. Not required to be loaded for domain logic.</summary>
    public User User { get; private set; } = null!;

    public BookingStatus Status { get; private set; }

    /// <summary>Optional message from the user to the admin.</summary>
    public string? Note { get; private set; }

    public DateTime? CancelledAtUtc { get; private set; }

    public bool IsActive => Status == BookingStatus.Active;

    internal void Cancel(DateTime nowUtc)
    {
        Status = BookingStatus.Cancelled;
        CancelledAtUtc = nowUtc;
    }
}
