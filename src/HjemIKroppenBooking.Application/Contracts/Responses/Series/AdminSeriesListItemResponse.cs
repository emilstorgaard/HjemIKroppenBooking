using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;

namespace HjemIKroppenBooking.Application.Contracts.Responses.Series;

public sealed record AdminSeriesListItemResponse(
    Guid Id,
    string Title,
    Guid ServiceTypeId,
    string ServiceTypeName,
    ServiceCategorySummary Category,
    bool IsCancelled,
    int SessionCount,
    int ParticipantCount,
    DateTime? FirstStartsAt,
    DateTime? LastEndsAt,
    DateTime CreatedAt,
    DateTime UpdatedAt);
