using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Application.Features.ServiceTypes.Common;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.UpdateServiceType;

public sealed class UpdateServiceTypeCommandValidator : AppValidator<UpdateServiceTypeCommand>
{
    public UpdateServiceTypeCommandValidator() => Include(new ServiceTypeFieldsValidator());
}
