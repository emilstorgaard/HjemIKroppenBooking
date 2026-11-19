using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Series.CancelSeriesBookingForUser;

public sealed record CancelSeriesBookingForUserCommand(Guid SeriesId, Guid UserId) : ICommand<AdminSeriesBookingResponse>;
