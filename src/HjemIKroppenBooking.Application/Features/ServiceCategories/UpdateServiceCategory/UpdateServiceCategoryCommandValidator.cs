using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Application.Features.ServiceCategories.Common;

namespace HjemIKroppenBooking.Application.Features.ServiceCategories.UpdateServiceCategory;

public sealed class UpdateServiceCategoryCommandValidator : AppValidator<UpdateServiceCategoryCommand>
{
    public UpdateServiceCategoryCommandValidator() => Include(new ServiceCategoryFieldsValidator());
}
