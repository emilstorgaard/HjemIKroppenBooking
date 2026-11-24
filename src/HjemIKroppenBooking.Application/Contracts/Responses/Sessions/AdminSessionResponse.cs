using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;

namespace HjemIKroppenBooking.Application.Contracts.Responses.Sessions;

public sealed record AdminSessionResponse(
    Guid Id,
    Guid ServiceTypeId,
    string ServiceTypeName,
    ServiceCategorySummary Category,
    Guid? SeriesId,
    string? SeriesTitle,
    DateTime StartsAt,
    DateTime EndsAt,
    string? Location,
    string? Notes,
    int Capacity,
    int ActiveBookingCount,
    int AvailableSpots,
    bool IsCancelled,
    DateTime? CancelledAt,
    Guid CreatedByUserId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
