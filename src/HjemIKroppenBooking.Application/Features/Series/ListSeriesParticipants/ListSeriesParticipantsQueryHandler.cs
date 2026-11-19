using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Series.ListSeriesParticipants;

public sealed class ListSeriesParticipantsQueryHandler(ISeriesQueries series)
    : IQueryHandler<ListSeriesParticipantsQuery, PagedResponse<SeriesParticipantResponse>>
{
    public async Task<PagedResponse<SeriesParticipantResponse>> Handle(ListSeriesParticipantsQuery query, CancellationToken cancellationToken)
    {
        if (!await series.ExistsAsync(query.SeriesId, cancellationToken))
        {
            throw DomainException.NotFound(ErrorCodes.SeriesNotFound);
        }

        return await series.ListParticipantsAsync(query, cancellationToken);
    }
}
