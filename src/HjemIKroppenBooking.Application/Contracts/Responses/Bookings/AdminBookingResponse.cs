using HjemIKroppenBooking.Domain.Bookings;

namespace HjemIKroppenBooking.Application.Contracts.Responses.Bookings;

public sealed record AdminBookingResponse(
    Guid Id,
    Guid SessionId,
    Guid? SeriesId,
    Guid UserId,
    string UserFullName,
    string UserEmail,
    string ServiceTypeName,
    DateTime StartsAt,
    DateTime EndsAt,
    BookingStatus Status,
    string? Note,
    DateTime CreatedAt,
    DateTime? CancelledAt);
