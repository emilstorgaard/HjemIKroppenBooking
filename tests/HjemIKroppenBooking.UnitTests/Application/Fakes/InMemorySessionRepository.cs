using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Domain.Sessions;

namespace HjemIKroppenBooking.UnitTests.Application.Fakes;

/// <summary>Keeps aggregates in memory with all their bookings, like a tracking DbContext would.</summary>
internal sealed class InMemorySessionRepository : ISessionRepository, IBookingRepository
{
    public List<Session> Sessions { get; } = [];

    public List<Session> Removed { get; } = [];

    public Task<Session?> GetByIdAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult(Sessions.Find(s => s.Id == id));

    public Task<IReadOnlyList<Session>> GetBySeriesAsync(Guid seriesId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Session>>(Sessions.Where(s => s.SeriesId == seriesId).OrderBy(s => s.StartsAtUtc).ToList());

    public Task<IReadOnlyList<Session>> GetUpcomingBookedByUserAsync(Guid userId, DateTime nowUtc, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Session>>(Sessions.Where(s => !s.IsCancelled && s.StartsAtUtc > nowUtc && s.HasActiveBookingFor(userId)).ToList());

    public void Add(Session session) => Sessions.Add(session);

    public void Remove(Session session)
    {
        Sessions.Remove(session);
        Removed.Add(session);
    }

    public Task<BookingKey?> FindKeyAsync(Guid bookingId, CancellationToken cancellationToken)
    {
        var booking = Sessions.SelectMany(s => s.Bookings).FirstOrDefault(b => b.Id == bookingId);
        return Task.FromResult<BookingKey?>(booking is null ? null : new BookingKey(booking.Id, booking.SessionId, booking.UserId, booking.Status));
    }
}
