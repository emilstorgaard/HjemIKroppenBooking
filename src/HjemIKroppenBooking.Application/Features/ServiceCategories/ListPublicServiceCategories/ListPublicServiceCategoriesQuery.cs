using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.ServiceCategories.ListPublicServiceCategories;

/// <summary>
/// All categories, oldest first. Names and slugs are public, so categories without active service types are included,
/// e.g. for the category picker in Umbraco. The frontend hides categories without service types where needed.
/// </summary>
public sealed record ListPublicServiceCategoriesQuery(int Page = PagingDefaults.Page, int PageSize = PagingDefaults.PageSize)
    : IQuery<PagedResponse<ServiceCategoryResponse>>, IPagedQuery;
