using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Common.Validation;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.ListPublicServiceTypes;

public sealed class ListPublicServiceTypesQueryValidator : AppValidator<ListPublicServiceTypesQuery>
{
    public ListPublicServiceTypesQueryValidator() => Include(new PagingValidator());
}
