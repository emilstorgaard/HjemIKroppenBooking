using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.ActivateServiceType;

public sealed record ActivateServiceTypeCommand(Guid Id) : ICommand<Unit>;
