using HjemIKroppenBooking.Domain.Series;

namespace HjemIKroppenBooking.Application.Abstractions.Persistence;

/// <summary>Loads and stores the series aggregate (with its service type). Returned entities are tracked.</summary>
public interface ISessionSeriesRepository
{
    Task<SessionSeries?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    void Add(SessionSeries series);
}
