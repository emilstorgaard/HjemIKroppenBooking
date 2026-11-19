using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.ServiceTypes;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.ListServiceTypes;

/// <summary>All service types for admins, sorted by sort order, name, then id.</summary>
public sealed record ListServiceTypesQuery(
    ServiceCategory? Category = null, bool? IsActive = null, int Page = PagingDefaults.Page, int PageSize = PagingDefaults.PageSize)
    : IQuery<PagedResponse<AdminServiceTypeResponse>>, IPagedQuery;
