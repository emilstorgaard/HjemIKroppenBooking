using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.ServiceCategories.ListServiceCategories;

public sealed class ListServiceCategoriesQueryHandler(IServiceCategoryQueries categories)
    : IQueryHandler<ListServiceCategoriesQuery, PagedResponse<AdminServiceCategoryResponse>>
{
    public Task<PagedResponse<AdminServiceCategoryResponse>> Handle(ListServiceCategoriesQuery query, CancellationToken cancellationToken) =>
        categories.ListAsync(query, cancellationToken);
}
