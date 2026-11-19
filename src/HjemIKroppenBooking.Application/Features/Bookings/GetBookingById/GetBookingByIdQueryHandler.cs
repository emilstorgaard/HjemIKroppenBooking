using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Bookings.GetBookingById;

public sealed class GetBookingByIdQueryHandler(IBookingQueries bookings) : IQueryHandler<GetBookingByIdQuery, AdminBookingResponse>
{
    public async Task<AdminBookingResponse> Handle(GetBookingByIdQuery query, CancellationToken cancellationToken) =>
        await bookings.GetByIdAsync(query.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.BookingNotFound);
}
