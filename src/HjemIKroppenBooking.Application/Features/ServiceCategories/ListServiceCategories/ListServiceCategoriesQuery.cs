using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.ServiceCategories.ListServiceCategories;

/// <summary>All categories for admins, oldest first.</summary>
public sealed record ListServiceCategoriesQuery(int Page = PagingDefaults.Page, int PageSize = PagingDefaults.PageSize)
    : IQuery<PagedResponse<AdminServiceCategoryResponse>>, IPagedQuery;
