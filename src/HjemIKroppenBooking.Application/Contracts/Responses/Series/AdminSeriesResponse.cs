using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;

namespace HjemIKroppenBooking.Application.Contracts.Responses.Series;

/// <summary>A series with all its sessions. <see cref="ParticipantCount"/> counts distinct users with an active booking.</summary>
public sealed record AdminSeriesResponse(
    Guid Id,
    string Title,
    Guid ServiceTypeId,
    string ServiceTypeName,
    ServiceCategorySummary Category,
    bool IsCancelled,
    DateTime? CancelledAt,
    int ParticipantCount,
    DateTime? FirstStartsAt,
    DateTime? LastEndsAt,
    IReadOnlyList<AdminSeriesSessionResponse> Sessions,
    DateTime CreatedAt,
    DateTime UpdatedAt);
