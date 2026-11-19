using FluentValidation;
using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Users.ListUsers;

public sealed class ListUsersQueryValidator : AppValidator<ListUsersQuery>
{
    public ListUsersQueryValidator()
    {
        Include(new PagingValidator());
        RuleFor(x => x.Search).OptionalText(ValidationRules.SearchMaxLength);
        RuleFor(x => x.Role).IsInEnum().WithErrorCode(ErrorCodes.ValidationEnumInvalid);
    }
}
