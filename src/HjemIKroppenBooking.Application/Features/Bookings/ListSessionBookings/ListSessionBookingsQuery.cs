using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Bookings.ListSessionBookings;

/// <summary>A session's participants, in booking order. Cancelled bookings are excluded unless requested.</summary>
public sealed record ListSessionBookingsQuery(Guid SessionId, bool IncludeCancelled = false, int Page = PagingDefaults.Page, int PageSize = PagingDefaults.PageSize)
    : IQuery<PagedResponse<AdminBookingResponse>>, IPagedQuery;
