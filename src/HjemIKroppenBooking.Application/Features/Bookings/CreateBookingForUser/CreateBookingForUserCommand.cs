using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Bookings.CreateBookingForUser;

/// <summary>An admin books a user in. Same rules as user bookings, except that hidden service types can be booked.</summary>
public sealed record CreateBookingForUserCommand(Guid UserId, Guid SessionId, string? Note) : ICommand<AdminBookingResponse>;
