using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;

namespace HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;

/// <summary>An active service type as shown to users.</summary>
public sealed record ServiceTypeResponse(
    Guid Id, ServiceCategorySummary Category, string Name, string? Description, int DefaultDurationMinutes, int DefaultCapacity, int SortOrder);
