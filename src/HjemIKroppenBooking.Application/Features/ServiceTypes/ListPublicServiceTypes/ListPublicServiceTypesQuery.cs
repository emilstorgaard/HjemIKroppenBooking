using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.ServiceTypes;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.ListPublicServiceTypes;

/// <summary>Active service types, sorted by sort order, name, then id.</summary>
public sealed record ListPublicServiceTypesQuery(ServiceCategory? Category = null, int Page = PagingDefaults.Page, int PageSize = PagingDefaults.PageSize)
    : IQuery<PagedResponse<ServiceTypeResponse>>, IPagedQuery;
