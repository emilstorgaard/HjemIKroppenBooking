using HjemIKroppenBooking.Domain.Bookings;

namespace HjemIKroppenBooking.Application.Abstractions.Persistence;

/// <summary>The identifying facts of a booking, used to find the session aggregate that owns it.</summary>
public sealed record BookingKey(Guid Id, Guid SessionId, Guid UserId, BookingStatus Status);
