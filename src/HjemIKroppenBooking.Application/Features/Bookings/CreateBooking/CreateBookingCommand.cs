using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Bookings.CreateBooking;

/// <summary>Books a session for the signed-in user. Sessions that belong to a series can also be booked one by one.</summary>
public sealed record CreateBookingCommand(Guid SessionId, string? Note) : ICommand<BookingResponse>;
