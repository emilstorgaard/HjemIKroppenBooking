namespace HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;

/// <summary>The category embedded in service type, session, series and booking responses.</summary>
public sealed record ServiceCategorySummary(Guid Id, string Name, string Slug);
