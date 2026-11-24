using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;
using HjemIKroppenBooking.Application.Features.ServiceCategories.Common;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.ServiceCategories.UpdateServiceCategory;

public sealed class UpdateServiceCategoryCommandHandler(IServiceCategoryRepository categories, IServiceCategoryQueries categoryQueries, IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateServiceCategoryCommand, AdminServiceCategoryResponse>
{
    public async Task<AdminServiceCategoryResponse> Handle(UpdateServiceCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await categories.GetByIdAsync(command.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.ServiceCategoryNotFound);

        await categories.EnsureNameIsFreeAsync(command.Name, category.Id, cancellationToken);
        var slug = await categories.UniqueSlugAsync(command.Name, category.Id, cancellationToken);
        category.Rename(command.Name, slug);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await categoryQueries.GetByIdAsync(category.Id, cancellationToken) ?? throw new InvalidOperationException("The updated category could not be read back.");
    }
}
