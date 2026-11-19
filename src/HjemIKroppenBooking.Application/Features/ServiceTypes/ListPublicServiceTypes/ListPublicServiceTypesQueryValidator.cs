using FluentValidation;
using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.ListPublicServiceTypes;

public sealed class ListPublicServiceTypesQueryValidator : AppValidator<ListPublicServiceTypesQuery>
{
    public ListPublicServiceTypesQueryValidator()
    {
        Include(new PagingValidator());
        RuleFor(x => x.Category).IsInEnum().WithErrorCode(ErrorCodes.ValidationEnumInvalid);
    }
}
