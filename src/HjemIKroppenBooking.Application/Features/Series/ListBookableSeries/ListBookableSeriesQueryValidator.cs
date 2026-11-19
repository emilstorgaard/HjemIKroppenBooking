using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Common.Validation;

namespace HjemIKroppenBooking.Application.Features.Series.ListBookableSeries;

public sealed class ListBookableSeriesQueryValidator : AppValidator<ListBookableSeriesQuery>
{
    public ListBookableSeriesQueryValidator() => Include(new PagingValidator());
}
