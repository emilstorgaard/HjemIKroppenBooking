using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Common.Mapping;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;
using HjemIKroppenBooking.Application.Features.ServiceCategories.Common;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.ServiceCategories;

namespace HjemIKroppenBooking.Application.Features.ServiceCategories.CreateServiceCategory;

public sealed class CreateServiceCategoryCommandHandler(IServiceCategoryRepository categories, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateServiceCategoryCommand, AdminServiceCategoryResponse>
{
    public async Task<AdminServiceCategoryResponse> Handle(CreateServiceCategoryCommand command, CancellationToken cancellationToken)
    {
        await categories.EnsureNameIsFreeAsync(command.Name, null, cancellationToken);
        var slug = await categories.UniqueSlugAsync(command.Name, null, cancellationToken);
        var category = ServiceCategory.Create(command.Name, slug);

        categories.Add(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return category.ToAdminResponse(serviceTypeCount: 0);
    }
}
