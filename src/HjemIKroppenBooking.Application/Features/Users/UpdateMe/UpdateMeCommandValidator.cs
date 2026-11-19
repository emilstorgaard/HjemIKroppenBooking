using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Users;

namespace HjemIKroppenBooking.Application.Features.Users.UpdateMe;

public sealed class UpdateMeCommandValidator : AppValidator<UpdateMeCommand>
{
    public UpdateMeCommandValidator()
    {
        RuleFor(x => x.FullName).RequiredText(User.FullNameMaxLength);
        RuleFor(x => x.Email).ValidEmail();
    }
}
