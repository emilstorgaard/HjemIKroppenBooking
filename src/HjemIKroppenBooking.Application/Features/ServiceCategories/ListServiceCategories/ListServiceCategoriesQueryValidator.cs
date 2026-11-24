using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Common.Validation;

namespace HjemIKroppenBooking.Application.Features.ServiceCategories.ListServiceCategories;

public sealed class ListServiceCategoriesQueryValidator : AppValidator<ListServiceCategoriesQuery>
{
    public ListServiceCategoriesQueryValidator() => Include(new PagingValidator());
}
