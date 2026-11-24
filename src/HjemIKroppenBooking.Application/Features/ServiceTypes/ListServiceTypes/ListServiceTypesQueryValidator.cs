using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Common.Validation;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.ListServiceTypes;

public sealed class ListServiceTypesQueryValidator : AppValidator<ListServiceTypesQuery>
{
    public ListServiceTypesQueryValidator() => Include(new PagingValidator());
}
