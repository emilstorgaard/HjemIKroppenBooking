using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.DeleteServiceType;

public sealed class DeleteServiceTypeCommandHandler(IServiceTypeRepository serviceTypes, IUnitOfWork unitOfWork) : ICommandHandler<DeleteServiceTypeCommand, Unit>
{
    public async Task<Unit> Handle(DeleteServiceTypeCommand command, CancellationToken cancellationToken)
    {
        var serviceType = await serviceTypes.GetByIdAsync(command.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.ServiceTypeNotFound);
        if (await serviceTypes.IsInUseAsync(serviceType.Id, cancellationToken))
        {
            throw DomainException.Conflict(ErrorCodes.ServiceTypeInUse);
        }

        serviceTypes.Remove(serviceType);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
