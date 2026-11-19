using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Bookings.CancelMyBooking;

/// <summary>Cancels one of the signed-in user's bookings. Allowed right up to the session start.</summary>
public sealed record CancelMyBookingCommand(Guid BookingId) : ICommand<BookingResponse>;
