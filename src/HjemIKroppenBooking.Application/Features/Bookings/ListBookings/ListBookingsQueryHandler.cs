using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Features.Bookings.Common;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Bookings.ListBookings;

public sealed class ListBookingsQueryHandler(IBookingQueries bookings, TimeProvider timeProvider)
    : IQueryHandler<ListBookingsQuery, PagedResponse<AdminBookingResponse>>
{
    public Task<PagedResponse<AdminBookingResponse>> Handle(ListBookingsQuery query, CancellationToken cancellationToken)
    {
        var filter = new BookingListFilter(
            timeProvider.GetUtcNow().UtcDateTime, query.UserId, query.SessionId, query.SeriesId, query.ServiceTypeId, query.Status, query.Upcoming, query.From, query.To);
        return bookings.ListAsync(filter, query.Page, query.PageSize, cancellationToken);
    }
}
