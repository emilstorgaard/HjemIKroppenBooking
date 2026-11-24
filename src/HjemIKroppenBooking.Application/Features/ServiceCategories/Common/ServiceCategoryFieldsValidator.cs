using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.ServiceCategories;

namespace HjemIKroppenBooking.Application.Features.ServiceCategories.Common;

public sealed class ServiceCategoryFieldsValidator : AppValidator<IServiceCategoryFields>
{
    public ServiceCategoryFieldsValidator() => RuleFor(x => x.Name).RequiredText(ServiceCategory.NameMaxLength);
}
