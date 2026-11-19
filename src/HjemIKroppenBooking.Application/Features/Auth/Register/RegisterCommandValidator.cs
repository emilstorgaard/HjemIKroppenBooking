using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Users;

namespace HjemIKroppenBooking.Application.Features.Auth.Register;

public sealed class RegisterCommandValidator : AppValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email).ValidEmail();
        RuleFor(x => x.FullName).RequiredText(User.FullNameMaxLength);
        RuleFor(x => x.Password).ValidNewPassword();
    }
}
