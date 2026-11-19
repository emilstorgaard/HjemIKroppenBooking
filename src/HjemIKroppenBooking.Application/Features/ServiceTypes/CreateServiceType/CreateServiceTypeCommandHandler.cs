using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Common.Mapping;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.ServiceTypes;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.CreateServiceType;

public sealed class CreateServiceTypeCommandHandler(IServiceTypeRepository serviceTypes, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateServiceTypeCommand, AdminServiceTypeResponse>
{
    public async Task<AdminServiceTypeResponse> Handle(CreateServiceTypeCommand command, CancellationToken cancellationToken)
    {
        var serviceType = ServiceType.Create(
            command.Category, command.Name, command.Description, command.DefaultDurationMinutes, command.DefaultCapacity, command.SortOrder, command.IsActive);

        if (await serviceTypes.NameExistsAsync(serviceType.Category, serviceType.Name, null, cancellationToken))
        {
            throw DomainException.Conflict(ErrorCodes.ServiceTypeNameAlreadyExists);
        }

        serviceTypes.Add(serviceType);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return serviceType.ToAdminResponse();
    }
}
