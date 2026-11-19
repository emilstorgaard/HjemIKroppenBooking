using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Users;

namespace HjemIKroppenBooking.Application.Features.Users.UpdateUser;

public sealed class UpdateUserCommandValidator : AppValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.FullName).RequiredText(User.FullNameMaxLength);
        RuleFor(x => x.Email).ValidEmail();
    }
}
