using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;
using HjemIKroppenBooking.Domain.Bookings;

namespace HjemIKroppenBooking.Application.Contracts.Responses.Bookings;

/// <summary>One of the signed-in user's bookings with the session details the user needs.</summary>
public sealed record BookingResponse(
    Guid Id,
    Guid SessionId,
    Guid? SeriesId,
    string ServiceTypeName,
    ServiceCategorySummary Category,
    DateTime StartsAt,
    DateTime EndsAt,
    string? Location,
    BookingStatus Status,
    string? Note,
    DateTime CreatedAt,
    DateTime? CancelledAt);
