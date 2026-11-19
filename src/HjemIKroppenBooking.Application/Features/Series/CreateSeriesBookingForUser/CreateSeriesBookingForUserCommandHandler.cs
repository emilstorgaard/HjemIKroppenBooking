using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Features.Bookings.Common;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Series.CreateSeriesBookingForUser;

public sealed class CreateSeriesBookingForUserCommandHandler(IUserRepository users, BookingCreator creator, IBookingQueries bookings)
    : ICommandHandler<CreateSeriesBookingForUserCommand, AdminSeriesBookingResponse>
{
    public async Task<AdminSeriesBookingResponse> Handle(CreateSeriesBookingForUserCommand command, CancellationToken cancellationToken)
    {
        var user = await users.GetByIdAsync(command.UserId, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.UserNotFound);
        if (!user.IsActive)
        {
            throw DomainException.Conflict(ErrorCodes.UserInactive);
        }

        var bookingIds = await creator.BookSeriesAsync(user.Id, command.SeriesId, command.Note, includeInactiveServiceTypes: true, cancellationToken);
        return new AdminSeriesBookingResponse(command.SeriesId, user.Id, await bookings.GetByIdsAsync(bookingIds, cancellationToken));
    }
}
