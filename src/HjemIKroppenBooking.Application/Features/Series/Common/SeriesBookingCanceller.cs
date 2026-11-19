using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Series.Common;

/// <summary>Cancels a user's bookings on a series' sessions that have not started, through the session aggregates.</summary>
public sealed class SeriesBookingCanceller(ISessionSeriesRepository seriesRepository, ISessionRepository sessions, IUnitOfWork unitOfWork, TimeProvider timeProvider)
{
    /// <summary>Returns the ids of the cancelled bookings.</summary>
    public async Task<IReadOnlyList<Guid>> CancelAsync(Guid seriesId, Guid userId, CancellationToken cancellationToken)
    {
        var series = await seriesRepository.GetByIdAsync(seriesId, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.SeriesNotFound);
        var seriesSessions = await sessions.GetBySeriesAsync(series.Id, cancellationToken);
        var cancelled = series.CancelBookingsFor(seriesSessions, userId, timeProvider.GetUtcNow().UtcDateTime);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return cancelled.Select(b => b.Id).ToList();
    }
}
