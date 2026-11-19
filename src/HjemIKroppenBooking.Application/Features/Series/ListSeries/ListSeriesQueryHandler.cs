using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Series.ListSeries;

public sealed class ListSeriesQueryHandler(ISeriesQueries series, TimeProvider timeProvider)
    : IQueryHandler<ListSeriesQuery, PagedResponse<AdminSeriesListItemResponse>>
{
    public Task<PagedResponse<AdminSeriesListItemResponse>> Handle(ListSeriesQuery query, CancellationToken cancellationToken) =>
        series.ListAsync(query, timeProvider.GetUtcNow().UtcDateTime, cancellationToken);
}
