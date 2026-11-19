using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Domain.Series;

namespace HjemIKroppenBooking.UnitTests.Application.Fakes;

internal sealed class InMemorySeriesRepository : ISessionSeriesRepository
{
    public List<SessionSeries> Series { get; } = [];

    public Task<SessionSeries?> GetByIdAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult(Series.Find(s => s.Id == id));

    public void Add(SessionSeries series) => Series.Add(series);
}
