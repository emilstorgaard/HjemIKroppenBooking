using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Features.Bookings.Common;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Bookings.CreateBooking;

public sealed class CreateBookingCommandHandler(ICurrentUser currentUser, BookingCreator creator, IBookingQueries bookings)
    : ICommandHandler<CreateBookingCommand, BookingResponse>
{
    public async Task<BookingResponse> Handle(CreateBookingCommand command, CancellationToken cancellationToken)
    {
        var bookingId = await creator.BookSessionAsync(currentUser.UserId, command.SessionId, command.Note, includeInactiveServiceTypes: false, cancellationToken);
        return await bookings.GetOwnByIdAsync(bookingId, currentUser.UserId, cancellationToken) ?? throw new InvalidOperationException("The created booking could not be read back.");
    }
}
