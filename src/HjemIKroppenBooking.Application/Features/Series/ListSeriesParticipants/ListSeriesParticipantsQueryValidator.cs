using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Common.Validation;

namespace HjemIKroppenBooking.Application.Features.Series.ListSeriesParticipants;

public sealed class ListSeriesParticipantsQueryValidator : AppValidator<ListSeriesParticipantsQuery>
{
    public ListSeriesParticipantsQueryValidator() => Include(new PagingValidator());
}
