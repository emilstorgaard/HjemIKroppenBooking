using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;
using HjemIKroppenBooking.Application.Features.ServiceCategories.ListPublicServiceCategories;
using HjemIKroppenBooking.Application.Features.ServiceCategories.ListServiceCategories;

namespace HjemIKroppenBooking.Application.Abstractions.Persistence;

/// <summary>Read side for service categories.</summary>
public interface IServiceCategoryQueries
{
    Task<PagedResponse<ServiceCategoryResponse>> ListPublicAsync(ListPublicServiceCategoriesQuery query, CancellationToken cancellationToken);

    Task<PagedResponse<AdminServiceCategoryResponse>> ListAsync(ListServiceCategoriesQuery query, CancellationToken cancellationToken);

    Task<AdminServiceCategoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
