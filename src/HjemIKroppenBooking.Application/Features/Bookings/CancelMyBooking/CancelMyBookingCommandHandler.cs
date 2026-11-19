using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Features.Bookings.Common;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Bookings.CancelMyBooking;

public sealed class CancelMyBookingCommandHandler(ICurrentUser currentUser, BookingCanceller canceller, IBookingQueries bookings)
    : ICommandHandler<CancelMyBookingCommand, BookingResponse>
{
    public async Task<BookingResponse> Handle(CancelMyBookingCommand command, CancellationToken cancellationToken)
    {
        await canceller.CancelAsync(command.BookingId, currentUser.UserId, cancellationToken);
        return await bookings.GetOwnByIdAsync(command.BookingId, currentUser.UserId, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.BookingNotFound);
    }
}
