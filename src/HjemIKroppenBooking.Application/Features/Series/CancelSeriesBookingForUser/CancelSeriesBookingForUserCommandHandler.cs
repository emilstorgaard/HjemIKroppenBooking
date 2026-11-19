using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Features.Series.Common;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Series.CancelSeriesBookingForUser;

public sealed class CancelSeriesBookingForUserCommandHandler(SeriesBookingCanceller canceller, IBookingQueries bookings)
    : ICommandHandler<CancelSeriesBookingForUserCommand, AdminSeriesBookingResponse>
{
    public async Task<AdminSeriesBookingResponse> Handle(CancelSeriesBookingForUserCommand command, CancellationToken cancellationToken)
    {
        var bookingIds = await canceller.CancelAsync(command.SeriesId, command.UserId, cancellationToken);
        return new AdminSeriesBookingResponse(command.SeriesId, command.UserId, await bookings.GetByIdsAsync(bookingIds, cancellationToken));
    }
}
