using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Series.CancelMySeriesBooking;

/// <summary>Cancels the signed-in user's bookings on the series' sessions that have not started.</summary>
public sealed record CancelMySeriesBookingCommand(Guid SeriesId) : ICommand<SeriesBookingResponse>;
