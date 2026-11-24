using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Application.Features.ServiceCategories.Common;

namespace HjemIKroppenBooking.Application.Features.ServiceCategories.CreateServiceCategory;

public sealed class CreateServiceCategoryCommandValidator : AppValidator<CreateServiceCategoryCommand>
{
    public CreateServiceCategoryCommandValidator() => Include(new ServiceCategoryFieldsValidator());
}
