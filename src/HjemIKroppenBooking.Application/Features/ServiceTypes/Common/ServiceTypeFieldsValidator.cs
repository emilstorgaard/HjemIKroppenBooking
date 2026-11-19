using FluentValidation;
using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.ServiceTypes;
using HjemIKroppenBooking.Domain.Sessions;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.Common;

public sealed class ServiceTypeFieldsValidator : AppValidator<IServiceTypeFields>
{
    public ServiceTypeFieldsValidator()
    {
        RuleFor(x => x.Category).IsInEnum().WithErrorCode(ErrorCodes.ValidationEnumInvalid);
        RuleFor(x => x.Name).RequiredText(ServiceType.NameMaxLength);
        RuleFor(x => x.Description).OptionalText(ServiceType.DescriptionMaxLength);
        RuleFor(x => x.DefaultDurationMinutes)
            .InclusiveBetween(Session.MinDurationMinutes, Session.MaxDurationMinutes).WithErrorCode(ErrorCodes.ValidationDurationOutOfRange);
        RuleFor(x => x.DefaultCapacity)
            .InclusiveBetween(Session.MinCapacity, Session.MaxCapacity).WithErrorCode(ErrorCodes.ValidationCapacityOutOfRange)
            .Equal(ServiceType.IndividualCapacity).WithErrorCode(ErrorCodes.ValidationIndividualCapacityMustBeOne)
            .When(x => x.Category != ServiceCategory.Group, ApplyConditionTo.CurrentValidator);
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.ValidationInvalidValue);
    }
}
