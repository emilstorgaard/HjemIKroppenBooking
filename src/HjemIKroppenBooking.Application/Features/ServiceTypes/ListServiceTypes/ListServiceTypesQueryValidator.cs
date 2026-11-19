using FluentValidation;
using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.ListServiceTypes;

public sealed class ListServiceTypesQueryValidator : AppValidator<ListServiceTypesQuery>
{
    public ListServiceTypesQueryValidator()
    {
        Include(new PagingValidator());
        RuleFor(x => x.Category).IsInEnum().WithErrorCode(ErrorCodes.ValidationEnumInvalid);
    }
}
