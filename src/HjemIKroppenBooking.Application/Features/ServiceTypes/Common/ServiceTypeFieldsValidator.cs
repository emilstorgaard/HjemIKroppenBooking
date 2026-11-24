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
        RuleFor(x => x.CategoryId).RequiredId();
        RuleFor(x => x.Name).RequiredText(ServiceType.NameMaxLength);
        RuleFor(x => x.Description).OptionalText(ServiceType.DescriptionMaxLength);
        RuleFor(x => x.DefaultDurationMinutes)
            .InclusiveBetween(Session.MinDurationMinutes, Session.MaxDurationMinutes).WithErrorCode(ErrorCodes.ValidationDurationOutOfRange);

        // Whether the category allows more than one spot is checked by the domain, which knows the category.
        RuleFor(x => x.DefaultCapacity)
            .InclusiveBetween(Session.MinCapacity, Session.MaxCapacity).WithErrorCode(ErrorCodes.ValidationCapacityOutOfRange);
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.ValidationInvalidValue);
    }
}
