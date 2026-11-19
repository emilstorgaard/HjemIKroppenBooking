using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Series.ListSeriesParticipants;

/// <summary>One row per user with bookings in the series, sorted by first booking time, then user id.</summary>
public sealed record ListSeriesParticipantsQuery(Guid SeriesId, bool IncludeCancelled = false, int Page = PagingDefaults.Page, int PageSize = PagingDefaults.PageSize)
    : IQuery<PagedResponse<SeriesParticipantResponse>>, IPagedQuery;
