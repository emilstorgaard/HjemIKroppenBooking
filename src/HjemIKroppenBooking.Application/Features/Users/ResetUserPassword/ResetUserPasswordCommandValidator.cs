using HjemIKroppenBooking.Application.Common.Validation;

namespace HjemIKroppenBooking.Application.Features.Users.ResetUserPassword;

public sealed class ResetUserPasswordCommandValidator : AppValidator<ResetUserPasswordCommand>
{
    public ResetUserPasswordCommandValidator()
    {
        RuleFor(x => x.NewPassword).ValidNewPassword();
    }
}
