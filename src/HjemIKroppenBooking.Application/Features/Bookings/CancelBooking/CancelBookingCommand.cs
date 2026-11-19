using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Bookings.CancelBooking;

/// <summary>An admin cancels any booking. The same rule as for users applies: not after the session has started.</summary>
public sealed record CancelBookingCommand(Guid BookingId) : ICommand<AdminBookingResponse>;
