using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Series.ListBookableSeries;

/// <summary>
/// Series users can book: not cancelled, active service type and at least one session that has not started.
/// <see cref="OnlyAvailable"/> keeps only series where none of the remaining sessions is full. Sorted by the next
/// session start, then id.
/// </summary>
public sealed record ListBookableSeriesQuery(Guid? ServiceTypeId = null, bool OnlyAvailable = false, int Page = PagingDefaults.Page, int PageSize = PagingDefaults.PageSize)
    : IQuery<PagedResponse<SeriesResponse>>, IPagedQuery;
