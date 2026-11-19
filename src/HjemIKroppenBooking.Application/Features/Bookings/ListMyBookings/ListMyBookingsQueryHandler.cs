using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Features.Bookings.Common;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Bookings.ListMyBookings;

public sealed class ListMyBookingsQueryHandler(ICurrentUser currentUser, IBookingQueries bookings, TimeProvider timeProvider)
    : IQueryHandler<ListMyBookingsQuery, PagedResponse<BookingResponse>>
{
    public Task<PagedResponse<BookingResponse>> Handle(ListMyBookingsQuery query, CancellationToken cancellationToken)
    {
        var filter = new BookingListFilter(timeProvider.GetUtcNow().UtcDateTime, UserId: currentUser.UserId, SeriesId: query.SeriesId, Status: query.Status, Upcoming: query.Upcoming);
        return bookings.ListOwnAsync(filter, query.Page, query.PageSize, cancellationToken);
    }
}
