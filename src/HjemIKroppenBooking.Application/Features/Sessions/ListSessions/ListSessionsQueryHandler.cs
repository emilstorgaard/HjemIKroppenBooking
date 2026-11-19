using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Sessions;
using HjemIKroppenBooking.Application.Features.Sessions.Common;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Sessions.ListSessions;

public sealed class ListSessionsQueryHandler(ISessionQueries sessions, TimeProvider timeProvider)
    : IQueryHandler<ListSessionsQuery, PagedResponse<AdminSessionResponse>>
{
    public Task<PagedResponse<AdminSessionResponse>> Handle(ListSessionsQuery query, CancellationToken cancellationToken)
    {
        var filter = new SessionListFilter(
            timeProvider.GetUtcNow().UtcDateTime,
            query.ServiceTypeId,
            query.Category,
            query.SeriesId,
            query.IsCancelled,
            query.Upcoming,
            query.From,
            query.To,
            query.OnlyAvailable);
        return sessions.ListAsync(filter, query.Page, query.PageSize, cancellationToken);
    }
}
