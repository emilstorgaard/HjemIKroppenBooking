using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Sessions;
using HjemIKroppenBooking.Application.Features.Sessions.Common;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Sessions.ListPublicSessions;

public sealed class ListPublicSessionsQueryHandler(ISessionQueries sessions, TimeProvider timeProvider)
    : IQueryHandler<ListPublicSessionsQuery, PagedResponse<SessionResponse>>
{
    public Task<PagedResponse<SessionResponse>> Handle(ListPublicSessionsQuery query, CancellationToken cancellationToken)
    {
        var filter = new SessionListFilter(
            timeProvider.GetUtcNow().UtcDateTime,
            ServiceTypeId: query.ServiceTypeId,
            CategoryId: query.CategoryId,
            FromUtc: query.From,
            ToUtc: query.To,
            OnlyAvailable: query.OnlyAvailable);
        return sessions.ListPublicAsync(filter, query.Page, query.PageSize, cancellationToken);
    }
}
