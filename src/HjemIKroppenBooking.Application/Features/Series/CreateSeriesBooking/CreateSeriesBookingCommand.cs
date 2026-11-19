using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Series.CreateSeriesBooking;

/// <summary>
/// Books every session of the series that has not started for the signed-in user, all or nothing. Sessions the user
/// already booked are skipped.
/// </summary>
public sealed record CreateSeriesBookingCommand(Guid SeriesId, string? Note) : ICommand<SeriesBookingResponse>;
