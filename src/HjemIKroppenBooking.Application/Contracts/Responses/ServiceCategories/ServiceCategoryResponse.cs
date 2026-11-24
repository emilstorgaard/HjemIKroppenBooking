namespace HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;

/// <summary>A category as shown to users. <see cref="Slug"/> is used in links such as <c>/book?kategori=danseterapi</c>.</summary>
public sealed record ServiceCategoryResponse(Guid Id, string Name, string Slug);
