using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Bookings.GetMyBookingById;

public sealed record GetMyBookingByIdQuery(Guid Id) : IQuery<BookingResponse>;
