using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Domain.Bookings;
using HjemIKroppenBooking.Domain.Sessions;
using Microsoft.EntityFrameworkCore;

namespace HjemIKroppenBooking.Infrastructure.Persistence.Repositories;

/// <summary>
/// Loads sessions as aggregates: service type plus all active bookings, in one query (no split query), so the
/// active bookings and <see cref="Session.ActiveBookingCount"/> come from the same statement.
/// </summary>
internal sealed class SessionRepository(AppDbContext db) : ISessionRepository
{
    public Task<Session?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Aggregates().SingleOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Session>> GetBySeriesAsync(Guid seriesId, CancellationToken cancellationToken) =>
        await Aggregates().Where(s => s.SeriesId == seriesId).OrderBy(s => s.StartsAtUtc).ThenBy(s => s.Id).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Session>> GetUpcomingBookedByUserAsync(Guid userId, DateTime nowUtc, CancellationToken cancellationToken) =>
        await Aggregates()
            .Where(s => !s.IsCancelled && s.StartsAtUtc > nowUtc && s.Bookings.Any(b => b.UserId == userId && b.Status == BookingStatus.Active))
            .ToListAsync(cancellationToken);

    public void Add(Session session) => db.Sessions.Add(session);

    public void Remove(Session session) => db.Sessions.Remove(session);

    private IQueryable<Session> Aggregates() =>
        db.Sessions
            .Include(s => s.ServiceType)
            .Include(s => s.Bookings.Where(b => b.Status == BookingStatus.Active))
            .AsSingleQuery();
}
