using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.ListServiceTypes;

/// <summary>All service types for admins, sorted by sort order, name, then id.</summary>
public sealed record ListServiceTypesQuery(
    Guid? CategoryId = null, bool? IsActive = null, int Page = PagingDefaults.Page, int PageSize = PagingDefaults.PageSize)
    : IQuery<PagedResponse<AdminServiceTypeResponse>>, IPagedQuery;
