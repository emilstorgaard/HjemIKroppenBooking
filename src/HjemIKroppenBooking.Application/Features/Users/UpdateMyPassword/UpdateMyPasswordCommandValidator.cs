using FluentValidation;
using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Users.UpdateMyPassword;

public sealed class UpdateMyPasswordCommandValidator : AppValidator<UpdateMyPasswordCommand>
{
    public UpdateMyPasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty().WithErrorCode(ErrorCodes.ValidationRequired);
        RuleFor(x => x.NewPassword)
            .ValidNewPassword()
            .Must((command, newPassword) => !string.Equals(newPassword, command.CurrentPassword, StringComparison.Ordinal)).WithErrorCode(ErrorCodes.ValidationPasswordUnchanged);
    }
}
