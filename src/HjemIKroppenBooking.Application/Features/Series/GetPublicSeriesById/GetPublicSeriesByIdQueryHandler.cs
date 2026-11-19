using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Series.GetPublicSeriesById;

/// <summary>Cancelled series and series of inactive service types give the same response as missing ones.</summary>
public sealed class GetPublicSeriesByIdQueryHandler(ISeriesQueries series, TimeProvider timeProvider) : IQueryHandler<GetPublicSeriesByIdQuery, SeriesResponse>
{
    public async Task<SeriesResponse> Handle(GetPublicSeriesByIdQuery query, CancellationToken cancellationToken) =>
        await series.GetPublicByIdAsync(query.Id, timeProvider.GetUtcNow().UtcDateTime, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.SeriesNotFound);
}
