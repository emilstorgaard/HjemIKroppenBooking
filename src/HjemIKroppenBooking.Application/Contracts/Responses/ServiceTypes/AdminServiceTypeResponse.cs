using HjemIKroppenBooking.Domain.ServiceTypes;

namespace HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;

public sealed record AdminServiceTypeResponse(
    Guid Id,
    ServiceCategory Category,
    string Name,
    string? Description,
    int DefaultDurationMinutes,
    int DefaultCapacity,
    int SortOrder,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);
