using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Series.CancelSeries;

/// <summary>Cancels the series, its sessions that have not started and their active bookings.</summary>
public sealed record CancelSeriesCommand(Guid Id) : ICommand<AdminSeriesResponse>;
