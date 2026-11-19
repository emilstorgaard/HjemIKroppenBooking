using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Bookings;

namespace HjemIKroppenBooking.Application.Features.Bookings.ListBookings;

public sealed record ListBookingsQuery(
    Guid? UserId = null,
    Guid? SessionId = null,
    Guid? SeriesId = null,
    Guid? ServiceTypeId = null,
    BookingStatus? Status = null,
    bool? Upcoming = null,
    DateTime? From = null,
    DateTime? To = null,
    int Page = PagingDefaults.Page,
    int PageSize = PagingDefaults.PageSize) : IQuery<PagedResponse<AdminBookingResponse>>, IPagedQuery;
