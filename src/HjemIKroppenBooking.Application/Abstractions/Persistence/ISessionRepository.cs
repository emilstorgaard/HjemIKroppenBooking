using HjemIKroppenBooking.Domain.Sessions;

namespace HjemIKroppenBooking.Application.Abstractions.Persistence;

/// <summary>
/// Loads and stores the session aggregate. Every load includes the service type and all active bookings in a single
/// query, which the aggregate requires for booking and cancelling. Returned entities are tracked.
/// </summary>
public interface ISessionRepository
{
    Task<Session?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>All sessions of the series, including cancelled and started ones.</summary>
    Task<IReadOnlyList<Session>> GetBySeriesAsync(Guid seriesId, CancellationToken cancellationToken);

    /// <summary>Non-cancelled sessions that have not started and on which the user has an active booking.</summary>
    Task<IReadOnlyList<Session>> GetUpcomingBookedByUserAsync(Guid userId, DateTime nowUtc, CancellationToken cancellationToken);

    void Add(Session session);

    void Remove(Session session);
}
