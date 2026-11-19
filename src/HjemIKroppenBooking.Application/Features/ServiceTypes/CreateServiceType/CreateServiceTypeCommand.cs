using HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;
using HjemIKroppenBooking.Application.Features.ServiceTypes.Common;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.ServiceTypes;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.CreateServiceType;

public sealed record CreateServiceTypeCommand(
    ServiceCategory Category, string Name, string? Description, int DefaultDurationMinutes, int DefaultCapacity, int SortOrder, bool IsActive)
    : ICommand<AdminServiceTypeResponse>, IServiceTypeFields;
