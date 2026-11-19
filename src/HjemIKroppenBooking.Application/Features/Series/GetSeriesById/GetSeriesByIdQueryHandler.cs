using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Series.GetSeriesById;

public sealed class GetSeriesByIdQueryHandler(ISeriesQueries series) : IQueryHandler<GetSeriesByIdQuery, AdminSeriesResponse>
{
    public async Task<AdminSeriesResponse> Handle(GetSeriesByIdQuery query, CancellationToken cancellationToken) =>
        await series.GetByIdAsync(query.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.SeriesNotFound);
}
