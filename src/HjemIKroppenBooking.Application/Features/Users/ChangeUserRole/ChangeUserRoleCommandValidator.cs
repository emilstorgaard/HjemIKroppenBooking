using FluentValidation;
using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Users.ChangeUserRole;

public sealed class ChangeUserRoleCommandValidator : AppValidator<ChangeUserRoleCommand>
{
    public ChangeUserRoleCommandValidator()
    {
        RuleFor(x => x.Role).IsInEnum().WithErrorCode(ErrorCodes.ValidationEnumInvalid);
    }
}
