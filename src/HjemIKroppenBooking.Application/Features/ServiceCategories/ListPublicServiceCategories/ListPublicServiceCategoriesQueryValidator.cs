using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Common.Validation;

namespace HjemIKroppenBooking.Application.Features.ServiceCategories.ListPublicServiceCategories;

public sealed class ListPublicServiceCategoriesQueryValidator : AppValidator<ListPublicServiceCategoriesQuery>
{
    public ListPublicServiceCategoriesQueryValidator() => Include(new PagingValidator());
}
