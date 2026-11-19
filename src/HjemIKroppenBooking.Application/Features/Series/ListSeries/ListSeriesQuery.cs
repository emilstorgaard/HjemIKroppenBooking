using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Series.ListSeries;

/// <summary>
/// All series for admins. <see cref="Upcoming"/>: true = has a non-cancelled session that has not ended.
/// Sorted by first session start (ascending for upcoming, otherwise descending), then id.
/// </summary>
public sealed record ListSeriesQuery(
    string? Search = null, Guid? ServiceTypeId = null, bool? Upcoming = null, bool? IsCancelled = null, int Page = PagingDefaults.Page, int PageSize = PagingDefaults.PageSize)
    : IQuery<PagedResponse<AdminSeriesListItemResponse>>, IPagedQuery;
