using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Common.Mapping;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.UpdateServiceType;

public sealed class UpdateServiceTypeCommandHandler(IServiceTypeRepository serviceTypes, IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateServiceTypeCommand, AdminServiceTypeResponse>
{
    public async Task<AdminServiceTypeResponse> Handle(UpdateServiceTypeCommand command, CancellationToken cancellationToken)
    {
        var serviceType = await serviceTypes.GetByIdAsync(command.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.ServiceTypeNotFound);

        // Changing the category would change the meaning of existing sessions and series.
        if (serviceType.Category != command.Category && await serviceTypes.IsInUseAsync(serviceType.Id, cancellationToken))
        {
            throw DomainException.Conflict(ErrorCodes.ServiceTypeInUse);
        }

        serviceType.Update(
            command.Category, command.Name, command.Description, command.DefaultDurationMinutes, command.DefaultCapacity, command.SortOrder, command.IsActive);

        if (await serviceTypes.NameExistsAsync(serviceType.Category, serviceType.Name, serviceType.Id, cancellationToken))
        {
            throw DomainException.Conflict(ErrorCodes.ServiceTypeNameAlreadyExists);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return serviceType.ToAdminResponse();
    }
}
