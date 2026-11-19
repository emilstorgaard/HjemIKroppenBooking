using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Series.ListBookableSeries;

public sealed class ListBookableSeriesQueryHandler(ISeriesQueries series, TimeProvider timeProvider)
    : IQueryHandler<ListBookableSeriesQuery, PagedResponse<SeriesResponse>>
{
    public Task<PagedResponse<SeriesResponse>> Handle(ListBookableSeriesQuery query, CancellationToken cancellationToken) =>
        series.ListBookableAsync(query, timeProvider.GetUtcNow().UtcDateTime, cancellationToken);
}
