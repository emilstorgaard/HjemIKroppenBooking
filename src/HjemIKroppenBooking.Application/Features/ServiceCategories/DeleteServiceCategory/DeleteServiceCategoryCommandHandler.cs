using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.ServiceCategories.DeleteServiceCategory;

public sealed class DeleteServiceCategoryCommandHandler(IServiceCategoryRepository categories, IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteServiceCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteServiceCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await categories.GetByIdAsync(command.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.ServiceCategoryNotFound);
        if (await categories.IsInUseAsync(category.Id, cancellationToken))
        {
            throw DomainException.Conflict(ErrorCodes.ServiceCategoryInUse);
        }

        categories.Remove(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
