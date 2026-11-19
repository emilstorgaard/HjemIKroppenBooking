using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Features.Bookings.Common;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Series.CreateSeriesBooking;

public sealed class CreateSeriesBookingCommandHandler(ICurrentUser currentUser, BookingCreator creator, IBookingQueries bookings)
    : ICommandHandler<CreateSeriesBookingCommand, SeriesBookingResponse>
{
    public async Task<SeriesBookingResponse> Handle(CreateSeriesBookingCommand command, CancellationToken cancellationToken)
    {
        var bookingIds = await creator.BookSeriesAsync(currentUser.UserId, command.SeriesId, command.Note, includeInactiveServiceTypes: false, cancellationToken);
        return new SeriesBookingResponse(command.SeriesId, await bookings.GetOwnByIdsAsync(bookingIds, cancellationToken));
    }
}
