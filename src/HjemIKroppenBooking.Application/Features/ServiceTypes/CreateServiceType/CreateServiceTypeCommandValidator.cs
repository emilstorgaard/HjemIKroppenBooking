using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Application.Features.ServiceTypes.Common;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.CreateServiceType;

public sealed class CreateServiceTypeCommandValidator : AppValidator<CreateServiceTypeCommand>
{
    public CreateServiceTypeCommandValidator() => Include(new ServiceTypeFieldsValidator());
}
