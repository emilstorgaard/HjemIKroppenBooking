using HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;
using HjemIKroppenBooking.Application.Features.ServiceTypes.Common;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.CreateServiceType;

public sealed record CreateServiceTypeCommand(
    Guid CategoryId, string Name, string? Description, int DefaultDurationMinutes, int DefaultCapacity, bool IsActive)
    : ICommand<AdminServiceTypeResponse>, IServiceTypeFields;
