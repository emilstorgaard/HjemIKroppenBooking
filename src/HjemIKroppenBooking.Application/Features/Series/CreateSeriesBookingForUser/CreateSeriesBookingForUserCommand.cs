using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Series.CreateSeriesBookingForUser;

public sealed record CreateSeriesBookingForUserCommand(Guid SeriesId, Guid UserId, string? Note) : ICommand<AdminSeriesBookingResponse>;
