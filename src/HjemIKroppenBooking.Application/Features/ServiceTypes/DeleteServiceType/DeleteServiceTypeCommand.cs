using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.DeleteServiceType;

/// <summary>Only unused service types can be deleted. Used ones should be deactivated instead.</summary>
public sealed record DeleteServiceTypeCommand(Guid Id) : ICommand<Unit>;
