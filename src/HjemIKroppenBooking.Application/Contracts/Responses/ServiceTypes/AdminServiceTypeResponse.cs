using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;

namespace HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;

public sealed record AdminServiceTypeResponse(
    Guid Id,
    ServiceCategorySummary Category,
    string Name,
    string? Description,
    int DefaultDurationMinutes,
    int DefaultCapacity,
    int SortOrder,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
