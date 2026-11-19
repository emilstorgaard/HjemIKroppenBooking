using HjemIKroppenBooking.Domain.Bookings;
using HjemIKroppenBooking.Domain.ServiceTypes;

namespace HjemIKroppenBooking.Application.Contracts.Responses.Bookings;

/// <summary>One of the signed-in user's bookings with the session details the user needs.</summary>
public sealed record BookingResponse(
    Guid Id,
    Guid SessionId,
    Guid? SeriesId,
    string ServiceTypeName,
    ServiceCategory Category,
    DateTime StartsAt,
    DateTime EndsAt,
    string? Location,
    BookingStatus Status,
    string? Note,
    DateTime CreatedAt,
    DateTime? CancelledAt);
