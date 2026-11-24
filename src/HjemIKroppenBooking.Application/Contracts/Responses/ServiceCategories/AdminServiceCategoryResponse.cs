namespace HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;

/// <summary>A category for admins. <see cref="ServiceTypeCount"/> counts all its service types, active or not.</summary>
public sealed record AdminServiceCategoryResponse(
    Guid Id,
    string Name,
    string Slug,
    int ServiceTypeCount,
    DateTime CreatedAt,
    DateTime UpdatedAt);
