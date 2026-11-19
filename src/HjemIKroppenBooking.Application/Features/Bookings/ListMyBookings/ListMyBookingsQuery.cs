using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Bookings;

namespace HjemIKroppenBooking.Application.Features.Bookings.ListMyBookings;

/// <summary>The signed-in user's bookings. The user id always comes from the token, never from the request.</summary>
public sealed record ListMyBookingsQuery(BookingStatus? Status = null, bool? Upcoming = null, Guid? SeriesId = null, int Page = PagingDefaults.Page, int PageSize = PagingDefaults.PageSize)
    : IQuery<PagedResponse<BookingResponse>>, IPagedQuery;
