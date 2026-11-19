using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.DeactivateServiceType;

/// <summary>Hides the service type and its sessions and series from users. Existing bookings are kept.</summary>
public sealed record DeactivateServiceTypeCommand(Guid Id) : ICommand<Unit>;
