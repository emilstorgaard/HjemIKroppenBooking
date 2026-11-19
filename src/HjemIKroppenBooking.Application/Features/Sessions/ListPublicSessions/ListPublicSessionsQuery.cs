using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Sessions;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.ServiceTypes;

namespace HjemIKroppenBooking.Application.Features.Sessions.ListPublicSessions;

/// <summary>Sessions users can book: not cancelled, not started, active service type. Sorted by start, then id.</summary>
public sealed record ListPublicSessionsQuery(
    Guid? ServiceTypeId = null,
    ServiceCategory? Category = null,
    DateTime? From = null,
    DateTime? To = null,
    bool OnlyAvailable = false,
    int Page = PagingDefaults.Page,
    int PageSize = PagingDefaults.PageSize) : IQuery<PagedResponse<SessionResponse>>, IPagedQuery;
