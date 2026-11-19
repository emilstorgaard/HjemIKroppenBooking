using HjemIKroppenBooking.Domain.Bookings;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Series;
using HjemIKroppenBooking.Domain.ServiceTypes;

namespace HjemIKroppenBooking.Domain.Sessions;

/// <summary>
/// A concrete, bookable time. This is the aggregate root for capacity: every booking and cancellation goes
/// through it and updates <see cref="ActiveBookingCount"/>, so the session row changes on every booking.
/// Together with the row version configured in persistence this makes concurrent bookings of the last spot
/// fail with a concurrency conflict instead of overbooking.
/// </summary>
/// <remarks>
/// Overlapping sessions are allowed, and a user may book sessions whose times overlap. The only booking
/// invariants are: active bookings never exceed <see cref="Capacity"/>, and a user has at most one active booking
/// per session.
/// </remarks>
public sealed class Session : Entity
{
    public const int MinCapacity = 1;
    public const int MaxCapacity = 500;
    public const int MinDurationMinutes = 15;
    public const int MaxDurationMinutes = 720;
    public const int LocationMaxLength = 200;
    public const int NotesMaxLength = 2000;

    private readonly List<Booking> _bookings = [];

    private Session()
    {
    }

    public Guid ServiceTypeId { get; private set; }

    /// <summary>Navigation for queries. Not required to be loaded for domain logic.</summary>
    public ServiceType ServiceType { get; private set; } = null!;

    public Guid? SeriesId { get; private set; }

    /// <summary>Navigation for queries. Not required to be loaded for domain logic.</summary>
    public SessionSeries? Series { get; private set; }

    public DateTime StartsAtUtc { get; private set; }

    public DateTime EndsAtUtc { get; private set; }

    public int Capacity { get; private set; }

    /// <summary>Denormalized number of active bookings. Changed only by this aggregate.</summary>
    public int ActiveBookingCount { get; private set; }

    public string? Location { get; private set; }

    /// <summary>Optional note for participants, e.g. what to bring.</summary>
    public string? Notes { get; private set; }

    public bool IsCancelled { get; private set; }

    public DateTime? CancelledAtUtc { get; private set; }

    public Guid CreatedByUserId { get; private set; }

    /// <summary>
    /// The loaded bookings. Operations that need them (booking, cancelling) require all active bookings to be
    /// loaded; cancelled bookings may be loaded too.
    /// </summary>
    public IReadOnlyCollection<Booking> Bookings => _bookings;

    public int AvailableSpots => Math.Max(0, Capacity - ActiveBookingCount);

    public bool IsFull => ActiveBookingCount >= Capacity;

    public bool HasStarted(DateTime nowUtc) => StartsAtUtc <= nowUtc;

    /// <summary>Creates a new session, optionally as part of a series. Overlaps with other sessions are allowed.</summary>
    public static Session Schedule(
        ServiceType serviceType,
        SessionSeries? series,
        DateTime startsAtUtc,
        DateTime endsAtUtc,
        int capacity,
        string? location,
        string? notes,
        Guid createdByUserId,
        DateTime nowUtc)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        Guard.Utc(nowUtc);
        if (createdByUserId == Guid.Empty)
        {
            throw new ArgumentException("A creating user is required.", nameof(createdByUserId));
        }

        if (!serviceType.IsActive)
        {
            throw DomainException.Conflict(ErrorCodes.ServiceTypeInactive);
        }

        if (series is not null)
        {
            if (series.ServiceTypeId != serviceType.Id)
            {
                throw DomainException.Validation(ErrorCodes.SessionServiceTypeMismatch, Guard.Field("serviceTypeId"));
            }

            if (series.IsCancelled)
            {
                throw DomainException.Conflict(ErrorCodes.SeriesCancelled);
            }
        }

        if (Guard.Utc(startsAtUtc) <= nowUtc)
        {
            throw DomainException.Validation(ErrorCodes.SessionStartInPast, Guard.Field("startsAt"));
        }

        ValidateSchedule(startsAtUtc, endsAtUtc);
        ValidateCapacity(capacity, activeBookingCount: 0);

        return new Session
        {
            ServiceTypeId = serviceType.Id,
            ServiceType = serviceType,
            SeriesId = series?.Id,
            Series = series,
            StartsAtUtc = startsAtUtc,
            EndsAtUtc = endsAtUtc,
            Capacity = capacity,
            Location = Guard.OptionalText(location, LocationMaxLength, "location"),
            Notes = Guard.OptionalText(notes, NotesMaxLength, "notes"),
            CreatedByUserId = createdByUserId
        };
    }

    /// <summary>
    /// Changes time, capacity and details. Allowed until the session has ended. A changed start must lie in the
    /// future. Capacity can never go below the number of active bookings.
    /// </summary>
    public void Update(DateTime startsAtUtc, DateTime endsAtUtc, int capacity, string? location, string? notes, DateTime nowUtc)
    {
        Guard.Utc(nowUtc);
        if (IsCancelled)
        {
            throw DomainException.Conflict(ErrorCodes.SessionCancelled, SessionParameters());
        }

        if (EndsAtUtc <= nowUtc)
        {
            throw DomainException.Conflict(ErrorCodes.SessionAlreadyEnded, SessionParameters());
        }

        if (Guard.Utc(startsAtUtc) != StartsAtUtc && startsAtUtc <= nowUtc)
        {
            throw DomainException.Validation(ErrorCodes.SessionStartInPast, Guard.Field("startsAt"));
        }

        ValidateSchedule(startsAtUtc, endsAtUtc);
        ValidateCapacity(capacity, ActiveBookingCount);
        var validLocation = Guard.OptionalText(location, LocationMaxLength, "location");
        var validNotes = Guard.OptionalText(notes, NotesMaxLength, "notes");

        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
        Capacity = capacity;
        Location = validLocation;
        Notes = validNotes;
    }

    /// <summary>Books a spot for the user. Bookings of overlapping sessions are allowed.</summary>
    public Booking Book(Guid userId, string? note, DateTime nowUtc)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("A user is required.", nameof(userId));
        }

        Guard.Utc(nowUtc);
        EnsureBookable(userId, nowUtc);

        var booking = new Booking(Id, userId, note);
        _bookings.Add(booking);
        ActiveBookingCount++;
        return booking;
    }

    /// <summary>Cancels an active booking. Allowed right up to the session start.</summary>
    public Booking CancelBooking(Guid bookingId, DateTime nowUtc)
    {
        Guard.Utc(nowUtc);
        EnsureBookingsLoaded();

        var booking = _bookings.Find(b => b.Id == bookingId) ?? throw DomainException.NotFound(ErrorCodes.BookingNotFound);
        if (!booking.IsActive)
        {
            throw DomainException.Conflict(ErrorCodes.BookingAlreadyCancelled);
        }

        if (HasStarted(nowUtc))
        {
            throw DomainException.Conflict(ErrorCodes.BookingCancellationClosed, SessionParameters());
        }

        booking.Cancel(nowUtc);
        ActiveBookingCount--;
        return booking;
    }

    public bool HasActiveBookingFor(Guid userId) => FindActiveBookingFor(userId) is not null;

    public Booking? FindActiveBookingFor(Guid userId)
    {
        EnsureBookingsLoaded();
        return _bookings.Find(b => b.UserId == userId && b.IsActive);
    }

    /// <summary>Cancels the session and all its active bookings. Only allowed before the session starts.</summary>
    public void Cancel(DateTime nowUtc)
    {
        Guard.Utc(nowUtc);
        EnsureBookingsLoaded();
        if (IsCancelled)
        {
            throw DomainException.Conflict(ErrorCodes.SessionCancelled, SessionParameters());
        }

        if (HasStarted(nowUtc))
        {
            throw DomainException.Conflict(ErrorCodes.SessionAlreadyStarted, SessionParameters());
        }

        foreach (var booking in _bookings.Where(b => b.IsActive))
        {
            booking.Cancel(nowUtc);
        }

        ActiveBookingCount = 0;
        IsCancelled = true;
        CancelledAtUtc = nowUtc;
    }

    /// <summary>A session can only be deleted when it has no active bookings.</summary>
    public void EnsureCanBeDeleted()
    {
        if (ActiveBookingCount > 0)
        {
            throw DomainException.Conflict(ErrorCodes.SessionHasActiveBookings, SessionParameters());
        }
    }

    /// <summary>Throws the error that <see cref="Book"/> would throw, without changing anything.</summary>
    internal void EnsureBookable(Guid userId, DateTime nowUtc)
    {
        EnsureBookingsLoaded();
        if (IsCancelled)
        {
            throw DomainException.Conflict(ErrorCodes.SessionCancelled, SessionParameters());
        }

        if (HasStarted(nowUtc))
        {
            throw DomainException.Conflict(ErrorCodes.SessionAlreadyStarted, SessionParameters());
        }

        if (HasActiveBookingFor(userId))
        {
            throw DomainException.Conflict(ErrorCodes.BookingAlreadyExists, SessionParameters());
        }

        if (IsFull)
        {
            throw DomainException.Conflict(ErrorCodes.SessionFull, SessionParameters());
        }
    }

    /// <summary>
    /// The loaded active bookings must match the counter. A mismatch means either the active bookings were not
    /// loaded or the row changed while it was being read; both are treated as a concurrency conflict so that no
    /// decision is made on stale data.
    /// </summary>
    internal void EnsureBookingsLoaded()
    {
        if (_bookings.Count(b => b.IsActive) != ActiveBookingCount)
        {
            throw DomainException.Conflict(ErrorCodes.SessionConcurrencyConflict, SessionParameters());
        }
    }

    private static void ValidateSchedule(DateTime startsAtUtc, DateTime endsAtUtc)
    {
        Guard.Utc(endsAtUtc);
        if (endsAtUtc <= startsAtUtc)
        {
            throw DomainException.Validation(ErrorCodes.ValidationEndBeforeStart, Guard.Field("endsAt"));
        }

        var duration = endsAtUtc - startsAtUtc;
        if (duration < TimeSpan.FromMinutes(MinDurationMinutes) || duration > TimeSpan.FromMinutes(MaxDurationMinutes))
        {
            throw DomainException.Validation(
                ErrorCodes.ValidationDurationOutOfRange, Guard.Field("endsAt", ("min", MinDurationMinutes), ("max", MaxDurationMinutes)));
        }
    }

    private static void ValidateCapacity(int capacity, int activeBookingCount)
    {
        Guard.InRange(capacity, MinCapacity, MaxCapacity, ErrorCodes.ValidationCapacityOutOfRange, "capacity");
        if (capacity < activeBookingCount)
        {
            throw DomainException.Conflict(
                ErrorCodes.SessionCapacityBelowActiveBookings, new Dictionary<string, object?> { ["activeBookingCount"] = activeBookingCount });
        }
    }

    private Dictionary<string, object?> SessionParameters() => new() { ["sessionId"] = Id, ["startsAt"] = StartsAtUtc };
}
