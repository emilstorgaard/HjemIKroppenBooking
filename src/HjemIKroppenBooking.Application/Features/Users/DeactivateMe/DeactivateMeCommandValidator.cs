using FluentValidation;
using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Users.DeactivateMe;

public sealed class DeactivateMeCommandValidator : AppValidator<DeactivateMeCommand>
{
    public DeactivateMeCommandValidator()
    {
        RuleFor(x => x.Password).NotEmpty().WithErrorCode(ErrorCodes.ValidationRequired);
    }
}
