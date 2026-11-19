using HjemIKroppenBooking.Domain.ServiceTypes;

namespace HjemIKroppenBooking.Api.Contracts.Requests.ServiceTypes;

/// <summary>Used to create and to fully replace a service type.</summary>
public sealed record ServiceTypeRequest(
    ServiceCategory Category, string Name, string? Description, int DefaultDurationMinutes, int DefaultCapacity, int SortOrder, bool IsActive);
