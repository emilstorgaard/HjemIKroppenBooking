using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Bookings.Common;

/// <summary>
/// Creates bookings through the session aggregates, for users booking themselves and for admins booking a user in.
/// All capacity and duplicate rules live in the domain; concurrent bookings of the last spot are caught by the
/// session row version and retried by the dispatcher.
/// </summary>
public sealed class BookingCreator(ISessionRepository sessions, ISessionSeriesRepository seriesRepository, IUnitOfWork unitOfWork, TimeProvider timeProvider)
{
    /// <param name="includeInactiveServiceTypes">Admins may book sessions of hidden service types; users get "not found".</param>
    public async Task<Guid> BookSessionAsync(Guid userId, Guid sessionId, string? note, bool includeInactiveServiceTypes, CancellationToken cancellationToken)
    {
        var session = await sessions.GetByIdAsync(sessionId, cancellationToken);
        if (session is null || (!includeInactiveServiceTypes && !session.ServiceType.IsActive))
        {
            throw DomainException.NotFound(ErrorCodes.SessionNotFound);
        }

        var booking = session.Book(userId, note, timeProvider.GetUtcNow().UtcDateTime);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return booking.Id;
    }

    /// <summary>
    /// Books every session of the series that has not started, all or nothing, in one save (one transaction).
    /// A conflict on any session rolls back the whole series booking before the dispatcher retries.
    /// </summary>
    /// <param name="includeInactiveServiceTypes">Admins may book series of hidden service types; users get "not found".</param>
    public async Task<IReadOnlyList<Guid>> BookSeriesAsync(Guid userId, Guid seriesId, string? note, bool includeInactiveServiceTypes, CancellationToken cancellationToken)
    {
        var series = await seriesRepository.GetByIdAsync(seriesId, cancellationToken);
        if (series is null || (!includeInactiveServiceTypes && !series.ServiceType.IsActive))
        {
            throw DomainException.NotFound(ErrorCodes.SeriesNotFound);
        }

        var seriesSessions = await sessions.GetBySeriesAsync(series.Id, cancellationToken);
        var bookings = series.Book(seriesSessions, userId, note, timeProvider.GetUtcNow().UtcDateTime);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return bookings.Select(b => b.Id).ToList();
    }
}
