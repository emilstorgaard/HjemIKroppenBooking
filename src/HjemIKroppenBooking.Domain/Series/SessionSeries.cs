using HjemIKroppenBooking.Domain.Bookings;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.ServiceTypes;
using HjemIKroppenBooking.Domain.Sessions;

namespace HjemIKroppenBooking.Domain.Series;

/// <summary>
/// Groups several sessions of a service type into one series that users can book in one go. The
/// sessions remain separate <see cref="Session"/> aggregates; every series operation goes through their methods,
/// so capacity and concurrency rules are the same as for single bookings. Series may overlap freely.
/// </summary>
public sealed class SessionSeries : Entity
{
    public const int TitleMaxLength = 200;
    public const int MaxSessions = 52;

    private readonly List<Session> _sessions = [];

    private SessionSeries()
    {
    }

    public Guid ServiceTypeId { get; private set; }

    /// <summary>Navigation for queries. Not required to be loaded for domain logic.</summary>
    public ServiceType ServiceType { get; private set; } = null!;

    public string Title { get; private set; } = string.Empty;

    public bool IsCancelled { get; private set; }

    public DateTime? CancelledAtUtc { get; private set; }

    /// <summary>Navigation for queries. Domain operations receive the sessions explicitly.</summary>
    public IReadOnlyCollection<Session> Sessions => _sessions;

    /// <summary>Creates a series for an active service type. The title defaults to the service type name.</summary>
    public static SessionSeries Create(ServiceType serviceType, string? title)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        if (!serviceType.IsActive)
        {
            throw DomainException.Conflict(ErrorCodes.ServiceTypeInactive);
        }

        return new SessionSeries
        {
            ServiceTypeId = serviceType.Id,
            ServiceType = serviceType,
            Title = Guard.OptionalText(title, TitleMaxLength, "title") ?? serviceType.Name
        };
    }

    public void Rename(string title) => Title = Guard.RequiredText(title, TitleMaxLength, "title");

    /// <summary>
    /// Books every session in the series that has not started and is not cancelled, all or nothing. Sessions the
    /// user already has an active booking on are skipped. If any remaining session is full, nothing is booked and
    /// the error identifies the full session.
    /// </summary>
    /// <remarks><paramref name="sessions"/> must contain all sessions of the series, each with its active bookings loaded.</remarks>
    public IReadOnlyList<Booking> Book(IReadOnlyCollection<Session> sessions, Guid userId, string? note, DateTime nowUtc)
    {
        EnsureOwns(sessions);
        Guard.Utc(nowUtc);
        if (IsCancelled)
        {
            throw DomainException.Conflict(ErrorCodes.SeriesCancelled);
        }

        var upcoming = sessions
            .Where(s => !s.IsCancelled && !s.HasStarted(nowUtc))
            .OrderBy(s => s.StartsAtUtc)
            .ThenBy(s => s.Id)
            .ToList();

        if (upcoming.Count == 0)
        {
            throw DomainException.Conflict(ErrorCodes.SeriesNoUpcomingSessions);
        }

        var toBook = upcoming.Where(s => !s.HasActiveBookingFor(userId)).ToList();
        if (toBook.Count == 0)
        {
            throw DomainException.Conflict(ErrorCodes.SeriesAlreadyBooked);
        }

        // Check every session before changing any, so a full session leaves the whole series untouched.
        foreach (var session in toBook)
        {
            session.EnsureBookable(userId, nowUtc);
        }

        return toBook.Select(s => s.Book(userId, note, nowUtc)).ToList();
    }

    /// <summary>Cancels the user's active bookings on sessions that have not started. Started sessions stay as history.</summary>
    /// <remarks><paramref name="sessions"/> must contain all sessions of the series, each with its active bookings loaded.</remarks>
    public IReadOnlyList<Booking> CancelBookingsFor(IReadOnlyCollection<Session> sessions, Guid userId, DateTime nowUtc)
    {
        EnsureOwns(sessions);
        Guard.Utc(nowUtc);

        var targets = sessions
            .Where(s => !s.IsCancelled && !s.HasStarted(nowUtc))
            .Select(s => (Session: s, Booking: s.FindActiveBookingFor(userId)))
            .Where(x => x.Booking is not null)
            .OrderBy(x => x.Session.StartsAtUtc)
            .ToList();

        if (targets.Count == 0)
        {
            throw DomainException.NotFound(ErrorCodes.SeriesNoActiveBookings);
        }

        return targets.Select(x => x.Session.CancelBooking(x.Booking!.Id, nowUtc)).ToList();
    }

    /// <summary>
    /// Cancels the series together with its sessions that have not started, including their active bookings.
    /// Sessions that have already started stay as history. Returns the number of sessions cancelled.
    /// </summary>
    /// <remarks><paramref name="sessions"/> must contain all sessions of the series, each with its active bookings loaded.</remarks>
    public int Cancel(IReadOnlyCollection<Session> sessions, DateTime nowUtc)
    {
        EnsureOwns(sessions);
        Guard.Utc(nowUtc);
        if (IsCancelled)
        {
            throw DomainException.Conflict(ErrorCodes.SeriesCancelled);
        }

        var toCancel = sessions.Where(s => !s.IsCancelled && !s.HasStarted(nowUtc)).ToList();
        foreach (var session in toCancel)
        {
            session.EnsureBookingsLoaded();
        }

        foreach (var session in toCancel)
        {
            session.Cancel(nowUtc);
        }

        IsCancelled = true;
        CancelledAtUtc = nowUtc;
        return toCancel.Count;
    }

    private void EnsureOwns(IReadOnlyCollection<Session> sessions)
    {
        ArgumentNullException.ThrowIfNull(sessions);
        if (sessions.Any(s => s.SeriesId != Id))
        {
            throw new ArgumentException("All sessions must belong to this series.", nameof(sessions));
        }
    }
}
