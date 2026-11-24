namespace HjemIKroppenBooking.Api.Contracts.Requests.ServiceTypes;

/// <summary>Used to create and to fully replace a service type.</summary>
public sealed record ServiceTypeRequest(
    Guid CategoryId, string Name, string? Description, int DefaultDurationMinutes, int DefaultCapacity, int SortOrder, bool IsActive);
