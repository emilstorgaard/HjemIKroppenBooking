using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Domain.Series;
using Microsoft.EntityFrameworkCore;

namespace HjemIKroppenBooking.Infrastructure.Persistence.Repositories;

internal sealed class SessionSeriesRepository(AppDbContext db) : ISessionSeriesRepository
{
    public Task<SessionSeries?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.SessionSeries.Include(s => s.ServiceType).SingleOrDefaultAsync(s => s.Id == id, cancellationToken);

    public void Add(SessionSeries series) => db.SessionSeries.Add(series);
}
