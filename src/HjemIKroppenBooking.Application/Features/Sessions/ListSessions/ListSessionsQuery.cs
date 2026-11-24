using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Sessions;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Sessions.ListSessions;

public sealed record ListSessionsQuery(
    Guid? ServiceTypeId = null,
    Guid? CategoryId = null,
    Guid? SeriesId = null,
    bool? IsCancelled = null,
    bool? Upcoming = null,
    DateTime? From = null,
    DateTime? To = null,
    bool OnlyAvailable = false,
    int Page = PagingDefaults.Page,
    int PageSize = PagingDefaults.PageSize) : IQuery<PagedResponse<AdminSessionResponse>>, IPagedQuery;
