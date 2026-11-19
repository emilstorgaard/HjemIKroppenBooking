using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Features.Bookings.Common;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Bookings.CancelBooking;

public sealed class CancelBookingCommandHandler(BookingCanceller canceller, IBookingQueries bookings) : ICommandHandler<CancelBookingCommand, AdminBookingResponse>
{
    public async Task<AdminBookingResponse> Handle(CancelBookingCommand command, CancellationToken cancellationToken)
    {
        await canceller.CancelAsync(command.BookingId, ownerUserId: null, cancellationToken);
        return await bookings.GetByIdAsync(command.BookingId, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.BookingNotFound);
    }
}
