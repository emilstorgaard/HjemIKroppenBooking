using HjemIKroppenBooking.Application.Common.Paging;

namespace HjemIKroppenBooking.Api.Contracts.Requests;

/// <summary>Query parameters shared by all list endpoints. <c>page</c> is 1-based; <c>pageSize</c> is 1-100.</summary>
public abstract class PagedRequest
{
    public int Page { get; init; } = PagingDefaults.Page;

    public int PageSize { get; init; } = PagingDefaults.PageSize;
}
