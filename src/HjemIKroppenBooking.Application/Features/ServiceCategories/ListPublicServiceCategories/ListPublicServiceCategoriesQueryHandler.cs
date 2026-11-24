using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.ServiceCategories.ListPublicServiceCategories;

public sealed class ListPublicServiceCategoriesQueryHandler(IServiceCategoryQueries categories)
    : IQueryHandler<ListPublicServiceCategoriesQuery, PagedResponse<ServiceCategoryResponse>>
{
    public Task<PagedResponse<ServiceCategoryResponse>> Handle(ListPublicServiceCategoriesQuery query, CancellationToken cancellationToken) =>
        categories.ListPublicAsync(query, cancellationToken);
}
