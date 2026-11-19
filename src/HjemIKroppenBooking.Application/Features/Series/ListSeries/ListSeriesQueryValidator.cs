using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Common.Validation;

namespace HjemIKroppenBooking.Application.Features.Series.ListSeries;

public sealed class ListSeriesQueryValidator : AppValidator<ListSeriesQuery>
{
    public ListSeriesQueryValidator()
    {
        Include(new PagingValidator());
        RuleFor(x => x.Search).OptionalText(ValidationRules.SearchMaxLength);
    }
}
