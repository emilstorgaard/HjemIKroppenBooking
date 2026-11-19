using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Bookings.GetBookingById;

public sealed record GetBookingByIdQuery(Guid Id) : IQuery<AdminBookingResponse>;
