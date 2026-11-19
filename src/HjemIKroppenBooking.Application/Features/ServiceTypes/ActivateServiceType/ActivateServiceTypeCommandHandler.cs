using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.ActivateServiceType;

public sealed class ActivateServiceTypeCommandHandler(IServiceTypeRepository serviceTypes, IUnitOfWork unitOfWork)
    : ICommandHandler<ActivateServiceTypeCommand, Unit>
{
    public async Task<Unit> Handle(ActivateServiceTypeCommand command, CancellationToken cancellationToken)
    {
        var serviceType = await serviceTypes.GetByIdAsync(command.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.ServiceTypeNotFound);
        serviceType.Activate();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
