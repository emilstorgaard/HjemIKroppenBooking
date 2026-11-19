using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Features.Bookings.Common;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Bookings;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Bookings.ListSessionBookings;

public sealed class ListSessionBookingsQueryHandler(ISessionQueries sessions, IBookingQueries bookings, TimeProvider timeProvider)
    : IQueryHandler<ListSessionBookingsQuery, PagedResponse<AdminBookingResponse>>
{
    public async Task<PagedResponse<AdminBookingResponse>> Handle(ListSessionBookingsQuery query, CancellationToken cancellationToken)
    {
        // Without this check a missing session would return an empty list instead of 404.
        if (!await sessions.ExistsAsync(query.SessionId, cancellationToken))
        {
            throw DomainException.NotFound(ErrorCodes.SessionNotFound);
        }

        var filter = new BookingListFilter(
            timeProvider.GetUtcNow().UtcDateTime, SessionId: query.SessionId, Status: query.IncludeCancelled ? null : BookingStatus.Active);
        return await bookings.ListAsync(filter, query.Page, query.PageSize, cancellationToken);
    }
}
