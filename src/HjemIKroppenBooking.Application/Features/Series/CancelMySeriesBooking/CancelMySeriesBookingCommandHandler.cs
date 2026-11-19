using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Features.Series.Common;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Series.CancelMySeriesBooking;

public sealed class CancelMySeriesBookingCommandHandler(ICurrentUser currentUser, SeriesBookingCanceller canceller, IBookingQueries bookings)
    : ICommandHandler<CancelMySeriesBookingCommand, SeriesBookingResponse>
{
    public async Task<SeriesBookingResponse> Handle(CancelMySeriesBookingCommand command, CancellationToken cancellationToken)
    {
        var bookingIds = await canceller.CancelAsync(command.SeriesId, currentUser.UserId, cancellationToken);
        return new SeriesBookingResponse(command.SeriesId, await bookings.GetOwnByIdsAsync(bookingIds, cancellationToken));
    }
}
