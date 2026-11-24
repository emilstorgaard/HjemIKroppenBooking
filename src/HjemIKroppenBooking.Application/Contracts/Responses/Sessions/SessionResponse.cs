using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;

namespace HjemIKroppenBooking.Application.Contracts.Responses.Sessions;

/// <summary>A bookable session as shown to users. Contains no participant data.</summary>
public sealed record SessionResponse(
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
    int AvailableSpots
);
