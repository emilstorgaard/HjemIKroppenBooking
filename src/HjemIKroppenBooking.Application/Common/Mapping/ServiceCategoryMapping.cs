using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;
using HjemIKroppenBooking.Domain.ServiceCategories;

namespace HjemIKroppenBooking.Application.Common.Mapping;

public static class ServiceCategoryMapping
{
    public static ServiceCategorySummary ToSummary(this ServiceCategory category) => new(category.Id, category.Name, category.Slug);

    public static AdminServiceCategoryResponse ToAdminResponse(this ServiceCategory category, int serviceTypeCount) => new(
        category.Id,
        category.Name,
        category.Slug,
        serviceTypeCount,
        category.CreatedAtUtc,
        category.UpdatedAtUtc);
}
