using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Bookings.GetMyBookingById;

/// <summary>Other users' bookings give the same response as missing ones, so ids cannot be probed.</summary>
public sealed class GetMyBookingByIdQueryHandler(ICurrentUser currentUser, IBookingQueries bookings) : IQueryHandler<GetMyBookingByIdQuery, BookingResponse>
{
    public async Task<BookingResponse> Handle(GetMyBookingByIdQuery query, CancellationToken cancellationToken) =>
        await bookings.GetOwnByIdAsync(query.Id, currentUser.UserId, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.BookingNotFound);
}
