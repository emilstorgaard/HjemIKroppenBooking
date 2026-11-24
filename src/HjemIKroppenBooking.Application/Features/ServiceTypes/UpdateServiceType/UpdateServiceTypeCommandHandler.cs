using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Common.Mapping;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.UpdateServiceType;

public sealed class UpdateServiceTypeCommandHandler(IServiceTypeRepository serviceTypes, IServiceCategoryRepository categories, IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateServiceTypeCommand, AdminServiceTypeResponse>
{
    public async Task<AdminServiceTypeResponse> Handle(UpdateServiceTypeCommand command, CancellationToken cancellationToken)
    {
        var serviceType = await serviceTypes.GetByIdAsync(command.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.ServiceTypeNotFound);
        var category = await categories.GetByIdAsync(command.CategoryId, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.ServiceCategoryNotFound);
        serviceType.Update(
            category, command.Name, command.Description, command.DefaultDurationMinutes, command.DefaultCapacity, command.IsActive);

        if (await serviceTypes.NameExistsAsync(serviceType.CategoryId, serviceType.Name, serviceType.Id, cancellationToken))
        {
            throw DomainException.Conflict(ErrorCodes.ServiceTypeNameAlreadyExists);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return serviceType.ToAdminResponse();
    }
}
