using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Features.Bookings.Common;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Bookings.CreateBookingForUser;

public sealed class CreateBookingForUserCommandHandler(IUserRepository users, BookingCreator creator, IBookingQueries bookings)
    : ICommandHandler<CreateBookingForUserCommand, AdminBookingResponse>
{
    public async Task<AdminBookingResponse> Handle(CreateBookingForUserCommand command, CancellationToken cancellationToken)
    {
        var user = await users.GetByIdAsync(command.UserId, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.UserNotFound);
        if (!user.IsActive)
        {
            throw DomainException.Conflict(ErrorCodes.UserInactive);
        }

        var bookingId = await creator.BookSessionAsync(user.Id, command.SessionId, command.Note, includeInactiveServiceTypes: true, cancellationToken);
        return await bookings.GetByIdAsync(bookingId, cancellationToken) ?? throw new InvalidOperationException("The created booking could not be read back.");
    }
}
