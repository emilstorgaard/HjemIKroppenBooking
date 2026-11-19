using HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;
using HjemIKroppenBooking.Application.Features.ServiceTypes.Common;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.ServiceTypes;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.UpdateServiceType;

public sealed record UpdateServiceTypeCommand(
    Guid Id, ServiceCategory Category, string Name, string? Description, int DefaultDurationMinutes, int DefaultCapacity, int SortOrder, bool IsActive)
    : ICommand<AdminServiceTypeResponse>, IServiceTypeFields;
