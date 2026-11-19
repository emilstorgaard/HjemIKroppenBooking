using HjemIKroppenBooking.Domain.ServiceTypes;

namespace HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;

/// <summary>An active service type as shown to users.</summary>
public sealed record ServiceTypeResponse(
    Guid Id, ServiceCategory Category, string Name, string? Description, int DefaultDurationMinutes, int DefaultCapacity, int SortOrder);
