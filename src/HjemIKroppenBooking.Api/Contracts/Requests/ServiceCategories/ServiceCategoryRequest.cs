namespace HjemIKroppenBooking.Api.Contracts.Requests.ServiceCategories;

/// <summary>Used to create and to rename a category. The slug used in frontend links is made from the name.</summary>
public sealed record ServiceCategoryRequest(string Name);
