using FluentValidation;
using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Users;

namespace HjemIKroppenBooking.Application.Features.Auth.Login;

public sealed class LoginCommandValidator : AppValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).RequiredText(User.EmailMaxLength);
        RuleFor(x => x.Password).NotEmpty().WithErrorCode(ErrorCodes.ValidationRequired);
    }
}
