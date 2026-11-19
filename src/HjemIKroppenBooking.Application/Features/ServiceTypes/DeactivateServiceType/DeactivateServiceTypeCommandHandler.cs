using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.DeactivateServiceType;

public sealed class DeactivateServiceTypeCommandHandler(IServiceTypeRepository serviceTypes, IUnitOfWork unitOfWork)
    : ICommandHandler<DeactivateServiceTypeCommand, Unit>
{
    public async Task<Unit> Handle(DeactivateServiceTypeCommand command, CancellationToken cancellationToken)
    {
        var serviceType = await serviceTypes.GetByIdAsync(command.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.ServiceTypeNotFound);
        serviceType.Deactivate();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
