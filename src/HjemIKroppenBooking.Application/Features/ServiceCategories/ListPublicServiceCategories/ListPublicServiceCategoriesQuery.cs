using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.ServiceCategories.ListPublicServiceCategories;

/// <summary>Categories with at least one active service type, oldest first.</summary>
public sealed record ListPublicServiceCategoriesQuery(int Page = PagingDefaults.Page, int PageSize = PagingDefaults.PageSize)
    : IQuery<PagedResponse<ServiceCategoryResponse>>, IPagedQuery;
